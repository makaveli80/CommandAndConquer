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

        [Header("Raycast Settings")]
        [SerializeField]
        [Tooltip("Layer utilisé pour les unités (devra être configuré)")]
        private LayerMask unitLayerMask = ~0; // Par défaut: tous les layers

        [SerializeField]
        [Tooltip("Distance maximale du raycast")]
        private float raycastDistance = 100f;

        // Unité actuellement sélectionnée
        private ISelectable currentSelection;

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