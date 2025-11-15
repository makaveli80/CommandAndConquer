using UnityEngine;

namespace CommandAndConquer.Core
{
    /// <summary>
    /// Classe de base abstraite pour toutes les unités du jeu.
    /// </summary>
    public abstract class UnitBase : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected string unitName;

        protected bool isSelected;

        protected virtual void Awake()
        {
            // Initialisation de base
        }

        protected virtual void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Initialise l'unité au démarrage.
        /// </summary>
        protected virtual void Initialize()
        {
            // À implémenter dans les classes dérivées
        }

        /// <summary>
        /// Appelé quand l'unité est sélectionnée.
        /// </summary>
        public virtual void OnSelected()
        {
            isSelected = true;
        }

        /// <summary>
        /// Appelé quand l'unité est désélectionnée.
        /// </summary>
        public virtual void OnDeselected()
        {
            isSelected = false;
        }

        public bool IsSelected => isSelected;
        public string UnitName => unitName;
    }
}
