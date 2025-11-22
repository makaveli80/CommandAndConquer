using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Gameplay
{
    /// <summary>
    /// Gère la sélection des unités (simple et multi-sélection par drag box) et les commandes de mouvement via la souris.
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

        [SerializeField]
        [Tooltip("Référence au DragBoxVisual de la scène (optionnel)")]
        private DragBoxVisual dragBoxVisual;

        [Header("Raycast Settings")]
        [SerializeField]
        [Tooltip("Layer utilisé pour les unités (devra être configuré)")]
        private LayerMask unitLayerMask = ~0; // Par défaut: tous les layers

        [SerializeField]
        [Tooltip("Distance maximale du raycast")]
        private float raycastDistance = 100f;

        [Header("Drag Box Selection")]
        [SerializeField]
        [Tooltip("Distance minimale (en pixels) pour considérer un drag (évite sélection accidentelle)")]
        private float dragThreshold = 5f;

        // Unités actuellement sélectionnées (multi-sélection)
        private HashSet<ISelectable> currentSelections = new HashSet<ISelectable>();

        // Unité actuellement survolée (pour le curseur)
        private ISelectable currentHoveredUnit;

        // État du drag box
        private bool isDragging = false;
        private Vector2 dragStartPosition;
        private Vector2 dragCurrentPosition;

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

            // === DRAG BOX DETECTION ===
            // Mouse DOWN: Commencer potentiel drag
            if (mouse.leftButton.wasPressedThisFrame)
            {
                dragStartPosition = mouse.position.ReadValue();
                isDragging = false; // Pas encore un drag, juste un clic pour l'instant
            }

            // Mouse HELD: Vérifier si on dépasse le threshold pour commencer le drag
            if (mouse.leftButton.isPressed && !isDragging)
            {
                dragCurrentPosition = mouse.position.ReadValue();
                float distance = Vector2.Distance(dragStartPosition, dragCurrentPosition);

                if (distance > dragThreshold)
                {
                    isDragging = true; // Transition: clic simple → drag box
                    Debug.Log($"[SelectionManager] Drag box started (distance: {distance:F1}px)");
                }
            }

            // Update position pendant le drag
            if (isDragging)
            {
                dragCurrentPosition = mouse.position.ReadValue();

                // Afficher le rectangle visuel du drag box
                if (dragBoxVisual != null)
                {
                    dragBoxVisual.ShowDragBox(dragStartPosition, dragCurrentPosition);
                }
            }

            // Mouse UP: Finaliser sélection (drag box ou clic simple)
            if (mouse.leftButton.wasReleasedThisFrame)
            {
                if (isDragging)
                {
                    // Drag box terminé
                    HandleDragBoxSelection();
                    isDragging = false;

                    // Cacher le rectangle visuel
                    if (dragBoxVisual != null)
                    {
                        dragBoxVisual.HideDragBox();
                    }
                }
                else
                {
                    // Clic simple (distance < threshold)
                    HandleSingleClickSelection();
                }
            }

            // === MOVEMENT COMMANDS ===
            // Clic droit: Donner un ordre de mouvement
            if (mouse.rightButton.wasPressedThisFrame)
            {
                HandleRightClick();
            }

            // === CURSOR FEEDBACK ===
            // Détection de survol pour le curseur (priorité: unité > destination > défaut)
            bool isHoveringUnit = HandleUnitHover();

            // Afficher le curseur de destination seulement si pas en train de survoler une unité
            if (!isHoveringUnit)
            {
                HandleDestinationHover();
            }
        }

        /// <summary>
        /// Gère le clic simple pour sélectionner une unité (distance < threshold).
        /// </summary>
        private void HandleSingleClickSelection()
        {
            Vector2 mousePosition = dragStartPosition; // Utiliser la position initiale du clic
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
                // Clic sur le vide: Désélectionner tout
                DeselectCurrentUnit();
            }
        }

        /// <summary>
        /// Gère la sélection par drag box (distance >= threshold).
        /// Sélectionne toutes les unités dans le rectangle défini par le drag.
        /// </summary>
        private void HandleDragBoxSelection()
        {
            // Convertir les positions écran en world space
            Vector3 startWorld = mainCamera.ScreenToWorldPoint(dragStartPosition);
            Vector3 endWorld = mainCamera.ScreenToWorldPoint(dragCurrentPosition);

            // Calculer les bounds du rectangle (min/max en 2D)
            Vector2 min = new Vector2(
                Mathf.Min(startWorld.x, endWorld.x),
                Mathf.Min(startWorld.y, endWorld.y)
            );
            Vector2 max = new Vector2(
                Mathf.Max(startWorld.x, endWorld.x),
                Mathf.Max(startWorld.y, endWorld.y)
            );

            // Trouver toutes les unités dans la zone
            Collider2D[] hitColliders = Physics2D.OverlapAreaAll(min, max, unitLayerMask);

            // Collecter les unités sélectionnables
            List<ISelectable> unitsInBox = new List<ISelectable>();
            foreach (Collider2D collider in hitColliders)
            {
                ISelectable selectable = collider.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    unitsInBox.Add(selectable);
                }
            }

            // Gérer la sélection (toujours remplacement, pas d'additif sans modificateurs)
            if (unitsInBox.Count > 0)
            {
                // Remplacer la sélection par les unités dans le box
                ClearSelection();
                foreach (ISelectable unit in unitsInBox)
                {
                    AddToSelection(unit);
                }
                Debug.Log($"[SelectionManager] Drag box selected {unitsInBox.Count} unit(s)");
            }
            else
            {
                // Aucune unité dans le box: désélectionner tout
                ClearSelection();
                Debug.Log("[SelectionManager] Drag box selected 0 units (deselecting all)");
            }
        }

        /// <summary>
        /// Gère le clic droit pour donner un ordre de mouvement à toutes les unités sélectionnées.
        /// </summary>
        private void HandleRightClick()
        {
            if (currentSelections.Count == 0) return;

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
                    // Déplacer toutes les unités sélectionnées qui peuvent bouger
                    int movedUnitsCount = 0;
                    foreach (ISelectable selectable in currentSelections)
                    {
                        IMovable movable = selectable as IMovable;
                        if (movable != null)
                        {
                            movable.MoveTo(targetGridPosition);
                            movedUnitsCount++;
                        }
                    }

                    if (movedUnitsCount > 0)
                    {
                        Debug.Log($"[SelectionManager] Move command to {targetGridPosition} for {movedUnitsCount} unit(s)");
                    }
                }
                else
                {
                    Debug.LogWarning($"[SelectionManager] Invalid grid position: {targetGridPosition}");
                }
            }
        }

        /// <summary>
        /// Sélectionne une unité (remplace la sélection actuelle).
        /// </summary>
        private void SelectUnit(ISelectable selectable)
        {
            SetSelection(selectable);
        }

        /// <summary>
        /// Désélectionne toutes les unités.
        /// </summary>
        private void DeselectCurrentUnit()
        {
            ClearSelection();
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
        /// N'affiche le curseur de mouvement que si au moins une unité sélectionnée peut bouger ET la destination est valide.
        /// </summary>
        private void HandleDestinationHover()
        {
            if (cursorManager == null || gridManager == null) return;

            // Ne montrer le curseur de destination que si au moins une unité est sélectionnée
            if (currentSelections.Count == 0)
            {
                cursorManager.ResetCursor();
                return;
            }

            // Vérifier qu'au moins une unité peut bouger
            bool hasMovableUnit = false;
            UnitBase firstMovableUnit = null;
            foreach (ISelectable selectable in currentSelections)
            {
                if (selectable is IMovable)
                {
                    hasMovableUnit = true;
                    firstMovableUnit = selectable as UnitBase;
                    break;
                }
            }

            if (!hasMovableUnit)
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
                // Utiliser la première unité movable pour la vérification de disponibilité
                if (firstMovableUnit != null && gridManager.IsCellAvailableFor(targetGridPosition, firstMovableUnit))
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
        public ISelectable CurrentSelection => currentSelections.Count > 0 ? null : null; // Deprecated - use CurrentSelections

        /// <summary>
        /// Obtient toutes les unités actuellement sélectionnées.
        /// </summary>
        public IReadOnlyCollection<ISelectable> CurrentSelections => currentSelections;

        #region Selection Helper Methods

        /// <summary>
        /// Ajoute une unité à la sélection.
        /// </summary>
        private void AddToSelection(ISelectable selectable)
        {
            if (selectable == null) return;

            // Ajouter à la collection (HashSet évite les duplicates automatiquement)
            if (currentSelections.Add(selectable))
            {
                // Déclencher l'événement de sélection
                selectable.OnSelected();
                Debug.Log($"[SelectionManager] Unit added to selection: {(selectable as MonoBehaviour)?.name}");
            }
        }

        /// <summary>
        /// Retire une unité de la sélection.
        /// </summary>
        private void RemoveFromSelection(ISelectable selectable)
        {
            if (selectable == null) return;

            // Retirer de la collection
            if (currentSelections.Remove(selectable))
            {
                // Déclencher l'événement de désélection
                selectable.OnDeselected();
                Debug.Log($"[SelectionManager] Unit removed from selection: {(selectable as MonoBehaviour)?.name}");
            }
        }

        /// <summary>
        /// Remplace la sélection entière par une seule unité.
        /// </summary>
        private void SetSelection(ISelectable selectable)
        {
            if (selectable == null)
            {
                ClearSelection();
                return;
            }

            // Désélectionner toutes les unités sauf celle-ci
            ClearSelection();

            // Sélectionner la nouvelle unité
            AddToSelection(selectable);
        }

        /// <summary>
        /// Désélectionne toutes les unités.
        /// </summary>
        private void ClearSelection()
        {
            // Désélectionner toutes les unités
            foreach (ISelectable selectable in currentSelections)
            {
                selectable.OnDeselected();
            }

            currentSelections.Clear();
            Debug.Log("[SelectionManager] All units deselected");
        }

        /// <summary>
        /// Vérifie si une unité est actuellement sélectionnée.
        /// </summary>
        private bool IsSelected(ISelectable selectable)
        {
            return currentSelections.Contains(selectable);
        }

        #endregion

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
