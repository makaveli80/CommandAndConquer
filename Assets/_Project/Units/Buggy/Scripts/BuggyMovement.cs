using CommandAndConquer.Units._Project.Units.Common.Vehicle;
using UnityEngine;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// Gère le déplacement du Buggy sur la grille case par case.
    /// Hérite de VehicleMovement pour éviter la duplication de code.
    /// </summary>
    public class BuggyMovement : VehicleMovement
    {
        // Référence au contrôleur (donne accès au contexte partagé)
        private BuggyController controller;

        protected override VehicleContext Context => controller?.Context;
        protected override string UnitTypeName => "Buggy";

        private void Awake()
        {
            controller = GetComponent<BuggyController>();
        }
    }
}
