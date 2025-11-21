using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// Contexte partagé entre tous les composants du Buggy.
    /// Centralise les références externes et l'état interne pour éviter la duplication.
    /// </summary>
    public class BuggyContext
    {
        #region External References

        /// <summary>
        /// Référence au GridManager (initialisée une seule fois).
        /// </summary>
        public GridManager GridManager { get; private set; }

        /// <summary>
        /// Configuration du Buggy (ScriptableObject).
        /// </summary>
        public BuggyData Data { get; private set; }

        /// <summary>
        /// Référence à l'unité elle-même (pour TryMoveUnitTo, etc.).
        /// </summary>
        public UnitBase Unit { get; private set; }

        #endregion

        #region Internal State

        /// <summary>
        /// Position actuelle du Buggy sur la grille.
        /// Source de vérité unique pour tous les composants.
        /// </summary>
        public GridPosition CurrentGridPosition { get; private set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialise le contexte avec les références nécessaires.
        /// À appeler dans BuggyController.Initialize().
        /// </summary>
        /// <param name="unit">L'unité propriétaire</param>
        /// <param name="data">Configuration du Buggy</param>
        /// <param name="initialPosition">Position initiale sur la grille</param>
        public void Initialize(UnitBase unit, BuggyData data, GridPosition initialPosition)
        {
            Unit = unit;
            Data = data;
            CurrentGridPosition = initialPosition;

            // Récupérer le GridManager une seule fois pour tous les composants
            GridManager = Object.FindFirstObjectByType<GridManager>();

            if (GridManager == null)
            {
                Debug.LogError("[BuggyContext] GridManager not found in scene!");
            }
        }

        #endregion

        #region State Management

        /// <summary>
        /// Met à jour la position actuelle du Buggy sur la grille.
        /// Appelé par BuggyMovement quand l'unité atteint une nouvelle cellule.
        /// </summary>
        public void UpdateGridPosition(GridPosition newPosition)
        {
            CurrentGridPosition = newPosition;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Vérifie que le contexte est correctement initialisé.
        /// </summary>
        public bool IsValid()
        {
            return GridManager != null && Data != null && Unit != null;
        }

        #endregion
    }
}
