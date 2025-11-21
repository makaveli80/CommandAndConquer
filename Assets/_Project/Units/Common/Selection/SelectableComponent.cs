using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Composant réutilisable pour gérer la sélection visuelle des unités.
    /// Supporte plusieurs types de feedback visuel (couleur ou corner brackets).
    /// </summary>
    [RequireComponent(typeof(UnitBase))]
    public class SelectableComponent : MonoBehaviour
    {
        #region Configuration

        [Header("Visual Type")]
        [SerializeField]
        [Tooltip("Type de feedback visuel à utiliser")]
        private SelectionVisualType visualType = SelectionVisualType.CornerBrackets;

        [Header("Visual Feedback - Sprite Color")]
        [SerializeField]
        [Tooltip("Couleur appliquée au sprite quand sélectionné (si visualType = SpriteColor)")]
        private Color selectedColor = new Color(0.5f, 1f, 0.5f, 1f); // Vert clair

        #endregion

        #region Private Fields

        private UnitBase unit;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private bool isSelected = false;

        // Référence au CornerBracketSelector si utilisé
        private CornerBracketSelector cornerBracketSelector;

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

            // Obtenir le CornerBracketSelector si le type visuel l'utilise
            if (visualType == SelectionVisualType.CornerBrackets)
            {
                cornerBracketSelector = GetComponent<CornerBracketSelector>();
                if (cornerBracketSelector == null)
                {
                    Debug.LogWarning($"[SelectableComponent] CornerBracketSelector not found on {gameObject.name}, but visualType is set to CornerBrackets. Adding component automatically.");
                    cornerBracketSelector = gameObject.AddComponent<CornerBracketSelector>();
                }
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
        /// Applique le feedback visuel selon le type configuré.
        /// </summary>
        private void ApplySelectionVisual()
        {
            switch (visualType)
            {
                case SelectionVisualType.SpriteColor:
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = selectedColor;
                    }
                    break;

                case SelectionVisualType.CornerBrackets:
                    if (cornerBracketSelector != null)
                    {
                        cornerBracketSelector.ShowBrackets();
                    }
                    break;
            }
        }

        /// <summary>
        /// Retire le feedback visuel selon le type configuré.
        /// </summary>
        private void RemoveSelectionVisual()
        {
            switch (visualType)
            {
                case SelectionVisualType.SpriteColor:
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = originalColor;
                    }
                    break;

                case SelectionVisualType.CornerBrackets:
                    if (cornerBracketSelector != null)
                    {
                        cornerBracketSelector.HideBrackets();
                    }
                    break;
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