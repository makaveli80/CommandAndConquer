using System;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;
using CommandAndConquer.Units._Project.Units.Common.Vehicle;

namespace CommandAndConquer.Units.Artillery
{
    /// <summary>
    /// Contexte partagé entre tous les composants de l'Artillery.
    /// OBSOLÈTE: Les composants utilisent maintenant GetComponent<Unit>() directement.
    /// </summary>
    [Obsolete("ArtilleryContext is obsolete. Components now use GetComponent<Unit>() directly.", false)]
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