using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Composant réutilisable pour gérer la sélection visuelle des unités.
    /// Change la couleur du sprite quand l'unité est sélectionnée.
    /// </summary>
    [RequireComponent(typeof(UnitBase))]
    public class SelectableComponent : MonoBehaviour
    {
        #region Configuration

        [Header("Visual Feedback")]
        [SerializeField]
        [Tooltip("Couleur appliquée au sprite quand sélectionné")]
        private Color selectedColor = new Color(0.5f, 1f, 0.5f, 1f); // Vert clair

        #endregion

        #region Private Fields

        private UnitBase unit;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private bool isSelected = false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            unit = GetComponent<UnitBase>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }

            // S'abonner aux événements de sélection
            if (unit != null)
            {
                unit.OnSelectedEvent += HandleSelected;
                unit.OnDeselectedEvent += HandleDeselected;
            }
        }

        private void OnDestroy()
        {
            // Se désabonner
            if (unit != null)
            {
                unit.OnSelectedEvent -= HandleSelected;
                unit.OnDeselectedEvent -= HandleDeselected;
            }
        }

        #endregion

        #region Selection Handling

        /// <summary>
        /// Appelé quand l'unité est sélectionnée.
        /// </summary>
        private void HandleSelected()
        {
            if (isSelected) return;
            isSelected = true;

            ApplySelectionVisual();
        }

        /// <summary>
        /// Appelé quand l'unité est désélectionnée.
        /// </summary>
        private void HandleDeselected()
        {
            if (!isSelected) return;
            isSelected = false;

            RemoveSelectionVisual();
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Applique le feedback visuel (changement de couleur du sprite).
        /// </summary>
        private void ApplySelectionVisual()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = selectedColor;
            }
        }

        /// <summary>
        /// Retire le feedback visuel (restaure la couleur d'origine).
        /// </summary>
        private void RemoveSelectionVisual()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Change la couleur de sélection.
        /// </summary>
        public void SetSelectedColor(Color color)
        {
            selectedColor = color;

            if (isSelected)
            {
                ApplySelectionVisual();
            }
        }

        #endregion
    }
}