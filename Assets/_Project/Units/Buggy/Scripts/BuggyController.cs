using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// Contrôleur principal de l'unité Buggy.
    /// Gère l'initialisation et coordonne les différents composants.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class BuggyController : UnitBase, IMovable, ISelectable
    {
        [Header("Buggy Configuration")]
        [SerializeField] private BuggyData buggyData;

        // Composants (seront ajoutés plus tard)
        private BuggyMovement movement;
        private GridManager gridManager;

        // État
        private GridPosition currentGridPosition;

        // IMovable properties
        public bool IsMoving => movement != null && movement.IsMoving;
        public float MoveSpeed => buggyData != null ? buggyData.moveSpeed : 0f;

        protected override void Awake()
        {
            base.Awake();

            // Récupérer les composants
            movement = GetComponent<BuggyMovement>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Récupérer le GridManager
            gridManager = FindFirstObjectByType<GridManager>();

            if (gridManager == null)
            {
                Debug.LogError("[BuggyController] GridManager not found in scene!");
                return;
            }

            // Initialiser la position sur la grille
            currentGridPosition = gridManager.GetGridPosition(transform.position);

            // S'enregistrer auprès du GridManager (gère automatiquement l'occupation)
            if (!gridManager.RegisterUnit(this, currentGridPosition))
            {
                Debug.LogError($"[BuggyController] Failed to register at {currentGridPosition}");
            }

            // Configurer le nom de l'unité
            if (buggyData != null)
            {
                unitName = buggyData.unitName;
            }

            Debug.Log($"[BuggyController] {unitName} initialized at {currentGridPosition}");
        }

        private void OnDestroy()
        {
            // Se désenregistrer du GridManager (libère automatiquement la cellule)
            gridManager?.UnregisterUnit(this);
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
                Debug.LogWarning("[BuggyController] BuggyMovement component not found!");
            }
        }

        // ISelectable implementation (héritées de UnitBase)
        // OnSelected() et OnDeselected() sont déjà implémentées

        // Getters
        public GridPosition CurrentGridPosition => currentGridPosition;
        public BuggyData Data => buggyData;

        // Sera appelé par BuggyMovement quand la position change
        public void UpdateGridPosition(GridPosition newPosition)
        {
            currentGridPosition = newPosition;
        }
    }
}
