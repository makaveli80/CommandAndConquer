using UnityEngine;

namespace CommandAndConquer.Gameplay
{
    /// <summary>
    /// Gère le changement de curseur en fonction du contexte de jeu.
    /// Supporte les curseurs animés (multiple frames).
    /// Singleton accessible globalement.
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        #region Singleton

        private static CursorManager instance;
        public static CursorManager Instance => instance;

        #endregion

        #region Configuration

        [Header("Cursor Textures")]
        [SerializeField]
        [Tooltip("Frames d'animation pour le curseur de survol d'unité")]
        private Texture2D[] hoverUnitFrames;

        [SerializeField]
        [Tooltip("Curseur pour indiquer une destination de mouvement")]
        private Texture2D moveCursor;

        [Header("Animation Settings")]
        [SerializeField]
        [Tooltip("FPS de l'animation du curseur")]
        private float animationFPS = 10f;

        [Header("Cursor Hotspot")]
        [SerializeField]
        [Tooltip("Point actif du curseur (en pixels depuis le coin supérieur gauche)")]
        private Vector2 cursorHotspot = new Vector2(16, 16);

        #endregion

        #region Private Fields

        // Type de curseur actuel
        private CursorType currentCursorType = CursorType.Default;

        // Animation
        private int currentFrame = 0;
        private float animationTimer = 0f;
        private bool isAnimating = false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Setup singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning("[CursorManager] Multiple instances detected! Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
        }

        private void Update()
        {
            // Animer le curseur si nécessaire
            if (isAnimating)
            {
                UpdateCursorAnimation();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Change le curseur selon le type spécifié.
        /// </summary>
        public void SetCursor(CursorType cursorType)
        {
            if (currentCursorType == cursorType)
                return;

            currentCursorType = cursorType;

            switch (cursorType)
            {
                case CursorType.Default:
                    ResetToDefaultCursor();
                    break;

                case CursorType.Hover:
                    SetAnimatedCursor(hoverUnitFrames);
                    break;

                case CursorType.Move:
                    SetStaticCursor(moveCursor);
                    break;
            }
        }

        /// <summary>
        /// Réinitialise le curseur au curseur par défaut du système.
        /// </summary>
        public void ResetCursor()
        {
            SetCursor(CursorType.Default);
        }

        #endregion

        #region Cursor Setup

        /// <summary>
        /// Configure un curseur statique (non-animé).
        /// </summary>
        private void SetStaticCursor(Texture2D cursorTexture)
        {
            isAnimating = false;

            if (cursorTexture != null)
            {
                Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
            }
            else
            {
                Debug.LogWarning($"[CursorManager] Texture manquante pour le curseur {currentCursorType}");
                ResetToDefaultCursor();
            }
        }

        /// <summary>
        /// Configure un curseur animé (multiple frames).
        /// </summary>
        private void SetAnimatedCursor(Texture2D[] frames)
        {
            if (frames == null || frames.Length == 0)
            {
                Debug.LogWarning($"[CursorManager] Aucune frame d'animation pour le curseur {currentCursorType}");
                ResetToDefaultCursor();
                return;
            }

            isAnimating = true;
            currentFrame = 0;
            animationTimer = 0f;

            // Afficher la première frame immédiatement
            Cursor.SetCursor(frames[0], cursorHotspot, CursorMode.Auto);
        }

        /// <summary>
        /// Réinitialise au curseur par défaut du système.
        /// </summary>
        private void ResetToDefaultCursor()
        {
            isAnimating = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        #endregion

        #region Animation

        /// <summary>
        /// Met à jour l'animation du curseur.
        /// </summary>
        private void UpdateCursorAnimation()
        {
            Texture2D[] frames = GetCurrentAnimationFrames();

            if (frames == null || frames.Length == 0)
            {
                isAnimating = false;
                return;
            }

            // Incrémenter le timer
            animationTimer += Time.deltaTime;

            // Changer de frame si nécessaire
            float frameDuration = 1f / animationFPS;
            if (animationTimer >= frameDuration)
            {
                animationTimer -= frameDuration;
                currentFrame = (currentFrame + 1) % frames.Length;

                // Mettre à jour le curseur avec la nouvelle frame
                Cursor.SetCursor(frames[currentFrame], cursorHotspot, CursorMode.Auto);
            }
        }

        /// <summary>
        /// Obtient les frames d'animation pour le curseur actuel.
        /// </summary>
        private Texture2D[] GetCurrentAnimationFrames()
        {
            switch (currentCursorType)
            {
                case CursorType.Hover:
                    return hoverUnitFrames;

                default:
                    return null;
            }
        }

        #endregion

        #region Debug

        private void OnValidate()
        {
            // Validation dans l'Inspector
            if (animationFPS <= 0)
            {
                animationFPS = 10f;
                Debug.LogWarning("[CursorManager] Animation FPS doit être > 0. Réinitialisé à 10.");
            }
        }

        #endregion
    }
}
