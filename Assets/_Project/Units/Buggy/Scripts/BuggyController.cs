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

        [Header("Selection Visual Feedback")]
        [SerializeField]
        [Tooltip("Couleur du sprite quand l'unité est sélectionnée")]
        private Color selectedColor = new Color(0.5f, 1f, 0.5f, 1f); // Vert clair

        // Composants
        private BuggyMovement movement;
        private SpriteRenderer spriteRenderer;

        // Contexte partagé
        private BuggyContext context;

        // Couleur d'origine du sprite
        private Color originalColor;

        // IMovable properties
        public bool IsMoving => movement != null && movement.IsMoving;
        public float MoveSpeed => buggyData != null ? buggyData.moveSpeed : 0f;

        protected override void Awake()
        {
            base.Awake();

            // Récupérer les composants
            movement = GetComponent<BuggyMovement>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Sauvegarder la couleur d'origine
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Initialiser le contexte partagé
            context = new BuggyContext();

            // Trouver le GridManager temporairement pour obtenir la position initiale
            GridManager gridManager = FindFirstObjectByType<GridManager>();
            if (gridManager == null)
            {
                Debug.LogError("[BuggyController] GridManager not found in scene!");
                return;
            }

            GridPosition initialPosition = gridManager.GetGridPosition(transform.position);

            // Initialiser le contexte avec toutes les références
            context.Initialize(this, buggyData, initialPosition);

            // Vérifier que le contexte est valide
            if (!context.IsValid())
            {
                Debug.LogError("[BuggyController] Context initialization failed!");
                return;
            }

            // S'enregistrer auprès du GridManager (gère automatiquement l'occupation)
            if (!context.GridManager.RegisterUnit(this, context.CurrentGridPosition))
            {
                Debug.LogError($"[BuggyController] Failed to register at {context.CurrentGridPosition}");
            }

            // Configurer le nom de l'unité
            if (buggyData != null)
            {
                unitName = buggyData.unitName;
            }

            Debug.Log($"[BuggyController] {unitName} initialized at {context.CurrentGridPosition}");
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
                Debug.LogWarning("[BuggyController] BuggyMovement component not found!");
            }
        }

        // ISelectable implementation avec feedback visuel
        public override void OnSelected()
        {
            base.OnSelected();

            // Changer la couleur du sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.color = selectedColor;
            }

            Debug.Log($"[BuggyController] {unitName} selected");
        }

        public override void OnDeselected()
        {
            base.OnDeselected();

            // Restaurer la couleur d'origine
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }

            Debug.Log($"[BuggyController] {unitName} deselected");
        }

        // Getters
        public GridPosition CurrentGridPosition => context.CurrentGridPosition;
        public BuggyData Data => buggyData;
        public BuggyContext Context => context;

        // Sera appelé par BuggyMovement quand la position change
        public void UpdateGridPosition(GridPosition newPosition)
        {
            context.UpdateGridPosition(newPosition);
        }
    }
}
