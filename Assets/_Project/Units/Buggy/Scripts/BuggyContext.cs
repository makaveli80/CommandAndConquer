using CommandAndConquer.Core;
using CommandAndConquer.Grid;
using CommandAndConquer.Units._Project.Units.Common.Vehicle;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// Contexte partagé entre tous les composants du Buggy.
    /// Hérite de VehicleContext pour éviter la duplication de code.
    /// Fournit un typage fort pour BuggyData.
    /// </summary>
    public class BuggyContext : VehicleContext
    {
        /// <summary>
        /// Configuration du Buggy (ScriptableObject typé).
        /// </summary>
        public new BuggyData Data => base.Data as BuggyData;

        /// <summary>
        /// Initialise le contexte avec les références nécessaires.
        /// À appeler dans BuggyController.Initialize().
        /// </summary>
        /// <param name="unit">L'unité propriétaire</param>
        /// <param name="data">Configuration du Buggy</param>
        /// <param name="initialPosition">Position initiale sur la grille</param>
        public void Initialize(UnitBase unit, BuggyData data, GridPosition initialPosition)
        {
            base.Initialize(unit, data, initialPosition);
        }
    }
}
