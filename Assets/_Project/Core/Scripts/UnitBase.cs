using System;
using UnityEngine;

namespace CommandAndConquer.Core
{
    /// <summary>
    /// Classe de base abstraite pour toutes les unités du jeu.
    /// OBSOLÈTE: Utilisez le composant Unit (non-abstrait) à la place.
    /// </summary>
    [Obsolete("UnitBase is obsolete. Use the Unit component instead.", false)]
    public abstract class UnitBase : MonoBehaviour
    {
        [Header("Unit Info")]
        [SerializeField] protected string unitName;

        protected bool isSelected;

        // Événements de sélection pour SelectableComponent
        public event System.Action OnSelectedEvent;
        public event System.Action OnDeselectedEvent;

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
            OnSelectedEvent?.Invoke();
        }

        /// <summary>
        /// Appelé quand l'unité est désélectionnée.
        /// </summary>
        public virtual void OnDeselected()
        {
            isSelected = false;
            OnDeselectedEvent?.Invoke();
        }

        public bool IsSelected => isSelected;
        public string UnitName => unitName;
    }
}
