using System;
using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Units.Artillery
{
    /// <summary>
    /// Contrôleur principal de l'unité Artillery.
    /// OBSOLÈTE: Utilisez le composant Unit générique à la place.
    /// </summary>
    [Obsolete("ArtilleryController is obsolete. Use the generic Unit component instead.", false)]
    public class ArtilleryController : UnitBase, IMovable, ISelectable
    {
        [Header("Artillery Configuration")]
        [SerializeField] private ArtilleryData artilleryData;

        // Composants
        private ArtilleryMovement movement;

        // Contexte partagé
        private ArtilleryContext context;

        // IMovable properties
        public bool IsMoving => movement != null && movement.IsMoving;
        public float MoveSpeed => artilleryData != null ? artilleryData.moveSpeed : 0f;

        protected override void Awake()
        {
            base.Awake();

            // Récupérer les composants
            movement = GetComponent<ArtilleryMovement>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialiser le contexte partagé
            context = new ArtilleryContext();

            // Trouver le GridManager temporairement pour obtenir la position initiale
            GridManager gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("[ArtilleryController] GridManager not found in scene!");
                return;
            }

            GridPosition initialPosition = gridManager.GetGridPosition(transform.position);

            // Initialiser le contexte avec toutes les références
            context.Initialize(this, artilleryData, initialPosition);

            // Vérifier que le contexte est valide
            if (!context.IsValid())
            {
                Debug.LogError("[ArtilleryController] Context initialization failed!");
                return;
            }

            // S'enregistrer auprès du GridManager (gère automatiquement l'occupation)
            if (!context.GridManager.RegisterUnit(this, context.CurrentGridPosition))
            {
                Debug.LogError($"[ArtilleryController] Failed to register at {context.CurrentGridPosition}");
            }

            // Configurer le nom de l'unité
            if (artilleryData != null)
            {
                unitName = artilleryData.unitName;
            }

            Debug.Log($"[ArtilleryController] {unitName} initialized at {context.CurrentGridPosition}");
        }

        private void OnDestroy()
        {
            // Se désenregistrer du GridManager (libère automatiquement la cellule)
            context?.GridManager?.UnregisterUnit(this);
        }

        // IMovable implementation
        public void MoveTo(GridPosition targetPosition)
        {
            if (movement != null)
            {
                movement.MoveTo(targetPosition);
            }
            else
            {
                Debug.LogWarning("[ArtilleryController] ArtilleryMovement component not found!");
            }
        }

        // ISelectable implementation (feedback visuel géré par SelectableComponent)
        public override void OnSelected()
        {
            base.OnSelected();
            Debug.Log($"[ArtilleryController] {unitName} selected");
        }

        public override void OnDeselected()
        {
            base.OnDeselected();
            Debug.Log($"[ArtilleryController] {unitName} deselected");
        }

        // Getters
        public GridPosition CurrentGridPosition => context.CurrentGridPosition;
        public ArtilleryData Data => artilleryData;
        public ArtilleryContext Context => context;

        // Sera appelé par ArtilleryMovement quand la position change
        public void UpdateGridPosition(GridPosition newPosition)
        {
            context.UpdateGridPosition(newPosition);
        }
    }
}