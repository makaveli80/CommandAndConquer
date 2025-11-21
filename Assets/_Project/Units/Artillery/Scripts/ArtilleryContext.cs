using CommandAndConquer.Core;
using CommandAndConquer.Grid;
using CommandAndConquer.Units._Project.Units.Common.Vehicle;

namespace CommandAndConquer.Units.Artillery
{
    /// <summary>
    /// Contexte partagé entre tous les composants de l'Artillery.
    /// Hérite de VehicleContext pour éviter la duplication de code.
    /// Fournit un typage fort pour ArtilleryData.
    /// </summary>
    public class ArtilleryContext : VehicleContext
    {
        /// <summary>
        /// Configuration de l'Artillery (ScriptableObject typé).
        /// </summary>
        public new ArtilleryData Data => base.Data as ArtilleryData;

        /// <summary>
        /// Initialise le contexte avec les références nécessaires.
        /// À appeler dans ArtilleryController.Initialize().
        /// </summary>
        /// <param name="unit">L'unité propriétaire</param>
        /// <param name="data">Configuration de l'Artillery</param>
        /// <param name="initialPosition">Position initiale sur la grille</param>
        public void Initialize(UnitBase unit, ArtilleryData data, GridPosition initialPosition)
        {
            base.Initialize(unit, data, initialPosition);
        }
    }
}