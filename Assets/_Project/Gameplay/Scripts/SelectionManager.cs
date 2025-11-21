using UnityEngine;
using UnityEngine.InputSystem;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Gameplay
{
    /// <summary>
    /// Gère la sélection des unités et les commandes de mouvement via la souris.
    /// Singleton pour un accès global.
    /// </summary>
    public class SelectionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        [Tooltip("Référence au GridManager de la scène")]
        private GridManager gridManager;

        [SerializeField]
        [Tooltip("Référence au CursorManager de la scène")]
        private CursorManager cursorManager;

        [Header("Raycast Settings")]
        [SerializeField]
        [Tooltip("Layer utilisé pour les unités (devra être configuré)")]
        private LayerMask unitLayerMask = ~0; // Par défaut: tous les layers

        [SerializeField]
        [Tooltip("Distance maximale du raycast")]
        private float raycastDistance = 100f;

        // Unité actuellement sélectionnée
        private ISelectable currentSelection;

        // Unité actuellement survolée (pour le curseur)
        private ISelectable currentHoveredUnit;

        // Camera de la scène
        private Camera mainCamera;

        // Input System
        private Mouse mouse;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[SelectionManager] Main camera not found!");
            }

            mouse = Mouse.current;
            if (mouse == null)
            {
                Debug.LogError("[SelectionManager] Mouse input not available!");
            }
        }

        private void Start()
        {
            // Trouver le GridManager si pas assigné dans l'inspecteur
            if (gridManager == null)
            {
                gridManager = FindFirstObjectByType<GridManager>();
                if (gridManager == null)
                {
                    Debug.LogError("[SelectionManager] GridManager not found in scene!");
                }
            }

            // Trouver le CursorManager si pas assigné dans l'inspecteur
            if (cursorManager == null)
            {
                cursorManager = FindFirstObjectByType<CursorManager>();
                if (cursorManager == null)
                {
                    Debug.LogWarning("[SelectionManager] CursorManager not found in scene! Cursor feedback will be disabled.");
                }
            }
        }

        private void Update()
        {
            if (mouse == null) return;

            // Clic gauche: Sélectionner une unité
            if (mouse.leftButton.wasPressedThisFrame)
            {
                HandleLeftClick();
            }

            // Clic droit: Donner un ordre de mouvement
            if (mouse.rightButton.wasPressedThisFrame)
            {
                HandleRightClick();
            }

            // Détection de survol pour le curseur (priorité: unité > destination > défaut)
            bool isHoveringUnit = HandleUnitHover();

            // Afficher le curseur de destination seulement si pas en train de survoler une unité
            if (!isHoveringUnit)
            {
                HandleDestinationHover();
            }
        }

        /// <summary>
        /// Gère le clic gauche pour sélectionner une unité.
        /// </summary>
        private void HandleLeftClick()
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, raycastDistance, unitLayerMask);

            if (hit.collider != null)
            {
                // Vérifier si l'objet touché est une unité sélectionnable
                ISelectable selectable = hit.collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    SelectUnit(selectable);
                    Debug.Log($"[SelectionManager] Unit selected: {hit.collider.name}");
                }
            }
            else
            {
                // Clic sur le vide: Désélectionner
                DeselectCurrentUnit();
            }
        }

        /// <summary>
        /// Gère le clic droit pour donner un ordre de mouvement.
        /// </summary>
        private void HandleRightClick()
        {
            if (currentSelection == null) return;

            // Vérifier que l'unité sélectionnée peut bouger
            IMovable movable = currentSelection as IMovable;
            if (movable == null) return;

            // Obtenir la position de la souris dans le monde
            Vector2 mousePosition = mouse.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0; // 2D

            // Convertir en position de grille
            if (gridManager != null)
            {
                GridPosition targetGridPosition = gridManager.GetGridPosition(worldPosition);

                // Vérifier que la position est valide
                if (gridManager.IsValidGridPosition(targetGridPosition))
                {
                    movable.MoveTo(targetGridPosition);
                    Debug.Log($"[SelectionManager] Move command to {targetGridPosition}");
                }
                else
                {
                    Debug.LogWarning($"[SelectionManager] Invalid grid position: {targetGridPosition}");
                }
            }
        }

        /// <summary>
        /// Sélectionne une unité.
        /// </summary>
        private void SelectUnit(ISelectable selectable)
        {
            // Désélectionner l'unité précédente si elle existe
            if (currentSelection != null && currentSelection != selectable)
            {
                currentSelection.OnDeselected();
            }

            // Sélectionner la nouvelle unité
            currentSelection = selectable;
            currentSelection.OnSelected();
        }

        /// <summary>
        /// Désélectionne l'unité actuelle.
        /// </summary>
        private void DeselectCurrentUnit()
        {
            if (currentSelection != null)
            {
                currentSelection.OnDeselected();
                currentSelection = null;
                Debug.Log("[SelectionManager] Unit deselected");
            }
        }

        /// <summary>
        /// Détecte le survol d'unités et met à jour le curseur.
        /// Retourne true si une unité est actuellement survolée.
        /// </summary>
        private bool HandleUnitHover()
        {
            if (cursorManager == null) return false;

            Vector2 mousePosition = mouse.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, raycastDistance, unitLayerMask);

            if (hit.collider != null)
            {
                // Vérifier si l'objet touché est une unité sélectionnable
                ISelectable hoveredUnit = hit.collider.GetComponent<ISelectable>();

                if (hoveredUnit != null)
                {
                    if (hoveredUnit != currentHoveredUnit)
                    {
                        // Nouvelle unité survolée
                        currentHoveredUnit = hoveredUnit;
                        cursorManager.SetCursor(CursorType.Hover);
                    }
                    return true; // Une unité est survolée
                }
            }

            // Plus d'unité survolée
            if (currentHoveredUnit != null)
            {
                currentHoveredUnit = null;
                cursorManager.ResetCursor();
            }

            return false; // Aucune unité survolée
        }

        /// <summary>
        /// Détecte le survol de destinations valides et met à jour le curseur.
        /// N'affiche le curseur de mouvement que si une unité est sélectionnée ET la destination est valide.
        /// </summary>
        private void HandleDestinationHover()
        {
            if (cursorManager == null || gridManager == null) return;

            // Ne montrer le curseur de destination que si une unité est sélectionnée
            if (currentSelection == null)
            {
                cursorManager.ResetCursor();
                return;
            }

            // Vérifier que l'unité peut bouger
            IMovable movable = currentSelection as IMovable;
            if (movable == null)
            {
                cursorManager.ResetCursor();
                return;
            }

            // Obtenir la position de la souris dans le monde
            Vector2 mousePosition = mouse.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0; // 2D

            // Convertir en position de grille
            GridPosition targetGridPosition = gridManager.GetGridPosition(worldPosition);

            // Vérifier que la position est valide ET disponible
            if (gridManager.IsValidGridPosition(targetGridPosition))
            {
                // Obtenir l'unité sélectionnée comme UnitBase pour la vérification de disponibilité
                UnitBase selectedUnit = currentSelection as UnitBase;

                if (selectedUnit != null && gridManager.IsCellAvailableFor(targetGridPosition, selectedUnit))
                {
                    // Destination valide: afficher le curseur de mouvement
                    cursorManager.SetCursor(CursorType.Move);
                }
                else
                {
                    // Cellule occupée: curseur par défaut
                    cursorManager.ResetCursor();
                }
            }
            else
            {
                // Position invalide (hors grille): curseur par défaut
                cursorManager.ResetCursor();
            }
        }

        /// <summary>
        /// Obtient l'unité actuellement sélectionnée.
        /// </summary>
        public ISelectable CurrentSelection => currentSelection;

        private void OnDrawGizmos()
        {
            // Debug: Afficher la position de la souris dans le monde
            if (Application.isPlaying && mainCamera != null && mouse != null)
            {
                Vector2 mousePosition = mouse.position.ReadValue();
                Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0;

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(worldPosition, 0.2f);
            }
        }
    }
}
