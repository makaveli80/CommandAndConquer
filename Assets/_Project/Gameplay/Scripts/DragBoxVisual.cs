using UnityEngine;
using UnityEngine.UI;

namespace CommandAndConquer.Gameplay
{
    /// <summary>
    /// Gère l'affichage visuel du rectangle de drag box pendant la sélection multiple.
    /// Composant UI Canvas overlay qui affiche un rectangle en temps réel.
    /// </summary>
    public class DragBoxVisual : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField]
        [Tooltip("RectTransform du rectangle de sélection (UI Image)")]
        private RectTransform dragBoxRect;

        [SerializeField]
        [Tooltip("Image component pour la couleur et transparence")]
        private Image dragBoxImage;

        [Header("Visual Settings")]
        [SerializeField]
        [Tooltip("Couleur du rectangle (vert transparent par défaut)")]
        private Color boxColor = new Color(0f, 1f, 0f, 0.2f);

        [SerializeField]
        [Tooltip("Couleur de la bordure")]
        private Color borderColor = new Color(0f, 1f, 0f, 0.8f);

        [SerializeField]
        [Tooltip("Épaisseur de la bordure (en pixels)")]
        private float borderWidth = 2f;

        private void Awake()
        {
            // Vérifier les références
            if (dragBoxRect == null)
            {
                Debug.LogError("[DragBoxVisual] dragBoxRect not assigned!");
            }

            if (dragBoxImage == null && dragBoxRect != null)
            {
                dragBoxImage = dragBoxRect.GetComponent<Image>();
            }

            // Configurer la couleur
            if (dragBoxImage != null)
            {
                dragBoxImage.color = boxColor;
            }

            // Cacher le rectangle au démarrage
            HideDragBox();
        }

        /// <summary>
        /// Affiche et met à jour le rectangle de drag box entre deux positions écran.
        /// </summary>
        /// <param name="startScreenPos">Position de départ du drag (screen space)</param>
        /// <param name="currentScreenPos">Position actuelle de la souris (screen space)</param>
        public void ShowDragBox(Vector2 startScreenPos, Vector2 currentScreenPos)
        {
            if (dragBoxRect == null) return;

            // Activer le rectangle
            dragBoxRect.gameObject.SetActive(true);

            // Calculer position et taille du rectangle
            Vector2 min = new Vector2(
                Mathf.Min(startScreenPos.x, currentScreenPos.x),
                Mathf.Min(startScreenPos.y, currentScreenPos.y)
            );

            Vector2 size = new Vector2(
                Mathf.Abs(currentScreenPos.x - startScreenPos.x),
                Mathf.Abs(currentScreenPos.y - startScreenPos.y)
            );

            // Appliquer position et taille au RectTransform
            dragBoxRect.anchoredPosition = min;
            dragBoxRect.sizeDelta = size;
        }

        /// <summary>
        /// Cache le rectangle de drag box.
        /// </summary>
        public void HideDragBox()
        {
            if (dragBoxRect != null)
            {
                dragBoxRect.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Vérifie si le drag box est actuellement affiché.
        /// </summary>
        public bool IsVisible => dragBoxRect != null && dragBoxRect.gameObject.activeSelf;
    }
}
