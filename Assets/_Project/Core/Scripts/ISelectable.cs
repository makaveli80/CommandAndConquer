namespace CommandAndConquer.Core
{
    /// <summary>
    /// Interface pour les objets qui peuvent être sélectionnés par le joueur.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Appelé quand l'objet est sélectionné.
        /// </summary>
        void OnSelected();

        /// <summary>
        /// Appelé quand l'objet est désélectionné.
        /// </summary>
        void OnDeselected();

        /// <summary>
        /// Indique si l'objet est actuellement sélectionné.
        /// </summary>
        bool IsSelected { get; }
    }
}
