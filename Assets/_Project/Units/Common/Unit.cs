using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Composant générique pour toutes les unités du jeu.
    /// Centralise les données, événements et l'état de l'unité.
    /// Implémente IMovable et ISelectable en déléguant aux composants appropriés.
    /// Remplace UnitBase et tous les Controllers spécifiques (BuggyController, ArtilleryController, etc.).
    /// </summary>
    public class Unit : MonoBehaviour, IMovable, ISelectable
    {
        #region Configuration

        [Header("Unit Configuration")]
        [SerializeField]
        [Tooltip("Données de configuration de l'unité (ScriptableObject)")]
        private UnitData unitData;

        #endregion

        #region Runtime State

        private GridPosition currentGridPosition;
        private GridManager gridManager;
        private bool isSelected;

        // Composants (auto-découverts)
        private IMovementComponent movementComponent;

        #endregion

        #region Events

        /// <summary>
        /// Événement déclenché quand l'unité est sélectionnée.
        /// Utilisé par SelectableComponent pour afficher le feedback visuel.
        /// </summary>
        public event System.Action OnSelectedEvent;

        /// <summary>
        /// Événement déclenché quand l'unité est désélectionnée.
        /// Utilisé par SelectableComponent pour masquer le feedback visuel.
        /// </summary>
        public event System.Action OnDeselectedEvent;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Auto-découverte des composants
            // Cherche n'importe quel composant implémentant IMovementComponent
            // (VehicleMovement, InfantryMovement, AircraftMovement, etc.)
            movementComponent = GetComponent<IMovementComponent>();
            gridManager = FindFirstObjectByType<GridManager>();

            if (gridManager == null)
            {
                Debug.LogError($"[Unit] GridManager not found in scene!");
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            // Se désenregistrer du GridManager (libère automatiquement la cellule)
            gridManager?.UnregisterUnit(this);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialise l'unité au démarrage.
        /// Calcule la position initiale et s'enregistre auprès du GridManager.
        /// </summary>
        private void Initialize()
        {
            if (gridManager == null)
            {
                Debug.LogError($"[Unit] Cannot initialize without GridManager!");
                return;
            }

            // Calculer la position initiale sur la grille
            currentGridPosition = gridManager.GetGridPosition(transform.position);

            // S'enregistrer auprès du GridManager (gère automatiquement l'occupation de cellule)
            if (!gridManager.RegisterUnit(this, currentGridPosition))
            {
                Debug.LogError($"[Unit] Failed to register unit '{UnitName}' at {currentGridPosition}");
                return;
            }

            Debug.Log($"[Unit] '{UnitName}' initialized at {currentGridPosition}");
        }

        #endregion

        #region IMovable Implementation

        /// <summary>
        /// Déplace l'unité vers une position cible sur la grille.
        /// Délègue au composant de mouvement (VehicleMovement, InfantryMovement, etc.).
        /// </summary>
        public void MoveTo(GridPosition targetPosition)
        {
            if (movementComponent != null && unitData != null && unitData.canMove)
            {
                movementComponent.MoveTo(targetPosition);
            }
            else if (movementComponent == null)
            {
                Debug.LogWarning($"[Unit] '{UnitName}' has no movement component!");
            }
            else if (!unitData.canMove)
            {
                Debug.LogWarning($"[Unit] '{UnitName}' cannot move (canMove = false in UnitData)");
            }
        }

        /// <summary>
        /// Indique si l'unité est actuellement en mouvement.
        /// </summary>
        public bool IsMoving => movementComponent != null && movementComponent.IsMoving;

        /// <summary>
        /// Vitesse de déplacement de l'unité (depuis UnitData).
        /// </summary>
        public float MoveSpeed => unitData != null ? unitData.moveSpeed : 0f;

        #endregion

        #region ISelectable Implementation

        /// <summary>
        /// Appelé quand l'unité est sélectionnée par le joueur.
        /// Déclenche l'événement OnSelectedEvent pour le feedback visuel.
        /// </summary>
        public void OnSelected()
        {
            if (isSelected) return;

            isSelected = true;
            OnSelectedEvent?.Invoke();

            Debug.Log($"[Unit] '{UnitName}' selected");
        }

        /// <summary>
        /// Appelé quand l'unité est désélectionnée par le joueur.
        /// Déclenche l'événement OnDeselectedEvent pour retirer le feedback visuel.
        /// </summary>
        public void OnDeselected()
        {
            if (!isSelected) return;

            isSelected = false;
            OnDeselectedEvent?.Invoke();

            Debug.Log($"[Unit] '{UnitName}' deselected");
        }

        /// <summary>
        /// Indique si l'unité est actuellement sélectionnée.
        /// </summary>
        public bool IsSelected => isSelected;

        #endregion

        #region Public API

        /// <summary>
        /// Position actuelle de l'unité sur la grille.
        /// Source de vérité unique pour tous les composants.
        /// </summary>
        public GridPosition CurrentGridPosition => currentGridPosition;

        /// <summary>
        /// Données de configuration de l'unité (ScriptableObject).
        /// </summary>
        public UnitData Data => unitData;

        /// <summary>
        /// Référence au GridManager (pour les composants qui en ont besoin).
        /// </summary>
        public GridManager GridManager => gridManager;

        /// <summary>
        /// Nom de l'unité (depuis UnitData).
        /// </summary>
        public string UnitName => unitData != null ? unitData.unitName : "Unknown";

        /// <summary>
        /// Met à jour la position actuelle de l'unité sur la grille.
        /// Appelé par le composant de mouvement (VehicleMovement, etc.) quand l'unité se déplace.
        /// </summary>
        public void UpdateGridPosition(GridPosition newPosition)
        {
            currentGridPosition = newPosition;
        }

        #endregion
    }
}
