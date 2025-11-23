using UnityEngine;

namespace CommandAndConquer.Core
{
    /// <summary>
    /// Interface pour tous les composants de mouvement.
    /// Permet au composant Unit de communiquer avec n'importe quel type de mouvement
    /// (VehicleMovement, InfantryMovement, AircraftMovement, etc.).
    /// </summary>
    public interface IMovementComponent
    {
        /// <summary>
        /// Déplace l'unité vers une position cible sur la grille.
        /// </summary>
        /// <param name="targetPosition">Position cible sur la grille</param>
        void MoveTo(GridPosition targetPosition);

        /// <summary>
        /// Indique si l'unité est actuellement en mouvement.
        /// </summary>
        bool IsMoving { get; }

        /// <summary>
        /// Position mondiale cible actuelle (utilisée par VehicleAnimator pour les animations directionnelles).
        /// </summary>
        Vector3 CurrentTargetWorldPosition { get; }
    }
}
