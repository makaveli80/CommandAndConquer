using CommandAndConquer.Units._Project.Units.Common.Vehicle;
using UnityEngine;

namespace CommandAndConquer.Units.Artillery
{
    /// <summary>
    /// Gère le déplacement de l'Artillery sur la grille case par case.
    /// Hérite de VehicleMovement pour éviter la duplication de code.
    /// </summary>
    public class ArtilleryMovement : VehicleMovement
    {
        // Référence au contrôleur (donne accès au contexte partagé)
        private ArtilleryController controller;

        protected override VehicleContext Context => controller?.Context;
        protected override string UnitTypeName => "Artillery";

        private void Awake()
        {
            controller = GetComponent<ArtilleryController>();
        }
    }
}