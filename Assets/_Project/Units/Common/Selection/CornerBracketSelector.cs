using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Affiche des encoches en forme de L dans les 4 coins d'une unité sélectionnée.
    /// Les sprites sont créés dynamiquement et positionnés autour de l'unité.
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public class CornerBracketSelector : MonoBehaviour
    {
        #region Configuration

        [Header("Corner Bracket Settings")]
        [SerializeField]
        [Tooltip("Sprite en forme de L utilisé pour les encoches (blanc)")]
        private Sprite cornerBracketSprite;

        [SerializeField]
        [Tooltip("Distance des encoches depuis le centre de l'unité")]
        private float cornerDistance = 0.5f;

        [SerializeField]
        [Tooltip("Taille des sprites d'encoches")]
        private float cornerSize = 0.25f;

        [SerializeField]
        [Tooltip("Ordre de rendu des encoches (au-dessus de l'unité)")]
        private int sortingOrder = 10;

        [SerializeField]
        [Tooltip("Nom du sorting layer")]
        private string sortingLayerName = "Default";

        #endregion

        #region Private Fields

        // Les 4 GameObjects pour les encoches
        private GameObject topLeftCorner;
        private GameObject topRightCorner;
        private GameObject bottomLeftCorner;
        private GameObject bottomRightCorner;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Créer les encoches (invisibles au départ)
            CreateCornerBrackets();
            HideBrackets();
        }

        private void OnDestroy()
        {
            // Détruire les GameObjects des encoches
            DestroyCornerBrackets();
        }

        #endregion

        #region Corner Bracket Creation

        /// <summary>
        /// Crée les 4 GameObjects pour les encoches aux coins.
        /// </summary>
        private void CreateCornerBrackets()
        {
            // Coin supérieur gauche (0°) - ┌
            topLeftCorner = CreateCorner("TopLeft", new Vector3(-cornerDistance, cornerDistance, 0), 0f);

            // Coin supérieur droit (-90°) - ┐
            topRightCorner = CreateCorner("TopRight", new Vector3(cornerDistance, cornerDistance, 0), -90f);

            // Coin inférieur droit (180°) - ┘
            bottomRightCorner = CreateCorner("BottomRight", new Vector3(cornerDistance, -cornerDistance, 0), 180f);

            // Coin inférieur gauche (90°) - └
            bottomLeftCorner = CreateCorner("BottomLeft", new Vector3(-cornerDistance, -cornerDistance, 0), 90f);
        }

        /// <summary>
        /// Crée un GameObject pour une encoche de coin.
        /// </summary>
        private GameObject CreateCorner(string cornerName, Vector3 localPosition, float rotationZ)
        {
            GameObject corner = new GameObject($"Corner_{cornerName}");
            corner.transform.SetParent(transform);
            corner.transform.localPosition = localPosition;
            corner.transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
            corner.transform.localScale = Vector3.one * cornerSize;

            // Ajouter le SpriteRenderer
            SpriteRenderer sr = corner.AddComponent<SpriteRenderer>();
            sr.sprite = cornerBracketSprite;
            sr.color = Color.white;
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = sortingOrder;

            return corner;
        }

        /// <summary>
        /// Détruit tous les GameObjects des encoches.
        /// </summary>
        private void DestroyCornerBrackets()
        {
            if (topLeftCorner != null) Destroy(topLeftCorner);
            if (topRightCorner != null) Destroy(topRightCorner);
            if (bottomLeftCorner != null) Destroy(bottomLeftCorner);
            if (bottomRightCorner != null) Destroy(bottomRightCorner);
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Affiche les encoches (appelé par SelectableComponent).
        /// </summary>
        public void ShowBrackets()
        {
            SetBracketsActive(true);
        }

        /// <summary>
        /// Cache les encoches (appelé par SelectableComponent).
        /// </summary>
        public void HideBrackets()
        {
            SetBracketsActive(false);
        }

        /// <summary>
        /// Active ou désactive tous les GameObjects des encoches.
        /// </summary>
        private void SetBracketsActive(bool active)
        {
            if (topLeftCorner != null) topLeftCorner.SetActive(active);
            if (topRightCorner != null) topRightCorner.SetActive(active);
            if (bottomLeftCorner != null) bottomLeftCorner.SetActive(active);
            if (bottomRightCorner != null) bottomRightCorner.SetActive(active);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Met à jour le sprite utilisé pour les encoches.
        /// </summary>
        public void SetCornerSprite(Sprite sprite)
        {
            cornerBracketSprite = sprite;

            // Mettre à jour tous les corners
            UpdateCornerSprite(topLeftCorner);
            UpdateCornerSprite(topRightCorner);
            UpdateCornerSprite(bottomLeftCorner);
            UpdateCornerSprite(bottomRightCorner);
        }

        /// <summary>
        /// Met à jour le sprite d'un corner spécifique.
        /// </summary>
        private void UpdateCornerSprite(GameObject corner)
        {
            if (corner != null)
            {
                SpriteRenderer sr = corner.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sprite = cornerBracketSprite;
                }
            }
        }

        /// <summary>
        /// Met à jour la distance des encoches.
        /// </summary>
        public void SetCornerDistance(float distance)
        {
            cornerDistance = distance;

            // Repositionner les corners
            if (topLeftCorner != null) topLeftCorner.transform.localPosition = new Vector3(-cornerDistance, cornerDistance, 0);
            if (topRightCorner != null) topRightCorner.transform.localPosition = new Vector3(cornerDistance, cornerDistance, 0);
            if (bottomRightCorner != null) bottomRightCorner.transform.localPosition = new Vector3(cornerDistance, -cornerDistance, 0);
            if (bottomLeftCorner != null) bottomLeftCorner.transform.localPosition = new Vector3(-cornerDistance, -cornerDistance, 0);
        }

        #endregion

        #region Debug

        private void OnDrawGizmosSelected()
        {
            // Afficher la zone des encoches en mode édition
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position;

            // Dessiner un carré représentant la zone des encoches
            Vector3 topLeft = center + new Vector3(-cornerDistance, cornerDistance, 0);
            Vector3 topRight = center + new Vector3(cornerDistance, cornerDistance, 0);
            Vector3 bottomRight = center + new Vector3(cornerDistance, -cornerDistance, 0);
            Vector3 bottomLeft = center + new Vector3(-cornerDistance, -cornerDistance, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }

        #endregion
    }
}