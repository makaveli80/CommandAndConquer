namespace CommandAndConquer.Core
{
    /// <summary>
    /// Interface pour les unités qui peuvent se déplacer sur la grille.
    /// </summary>
    public interface IMovable
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
        /// Vitesse de déplacement de l'unité.
        /// </summary>
        float MoveSpeed { get; }
    }
}
