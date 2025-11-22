using UnityEngine;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// ScriptableObject qui stocke les sprites d'animation pour les 8 directions d'un véhicule.
    /// Chaque type de véhicule (Buggy, Artillery, etc.) aura sa propre instance de ce ScriptableObject.
    /// </summary>
    [CreateAssetMenu(fileName = "VehicleAnimationData", menuName = "Command & Conquer/Animation/Vehicle Animation Data")]
    public class VehicleAnimationData : ScriptableObject
    {
        #region Configuration

        [Header("Animation Sprites (8 Directions)")]
        [Tooltip("Sprite pour la direction Est (droite) - 0°")]
        [SerializeField] private Sprite spriteE;

        [Tooltip("Sprite pour la direction Nord-Est - 45°")]
        [SerializeField] private Sprite spriteNE;

        [Tooltip("Sprite pour la direction Nord (haut) - 90°")]
        [SerializeField] private Sprite spriteN;

        [Tooltip("Sprite pour la direction Nord-Ouest - 135°")]
        [SerializeField] private Sprite spriteNW;

        [Tooltip("Sprite pour la direction Ouest (gauche) - 180°")]
        [SerializeField] private Sprite spriteW;

        [Tooltip("Sprite pour la direction Sud-Ouest - 225°")]
        [SerializeField] private Sprite spriteSW;

        [Tooltip("Sprite pour la direction Sud (bas) - 270°")]
        [SerializeField] private Sprite spriteS;

        [Tooltip("Sprite pour la direction Sud-Est - 315°")]
        [SerializeField] private Sprite spriteSE;

        [Header("Default Settings")]
        [Tooltip("Direction par défaut quand le véhicule est à l'arrêt (Idle)")]
        [SerializeField] private DirectionType defaultDirection = DirectionType.S;

        #endregion

        #region Public API

        /// <summary>
        /// Obtient le sprite correspondant à une direction donnée.
        /// </summary>
        /// <param name="direction">Direction souhaitée</param>
        /// <returns>Sprite correspondant, ou null si non défini</returns>
        public Sprite GetSpriteForDirection(DirectionType direction)
        {
            return direction switch
            {
                DirectionType.E => spriteE,
                DirectionType.NE => spriteNE,
                DirectionType.N => spriteN,
                DirectionType.NW => spriteNW,
                DirectionType.W => spriteW,
                DirectionType.SW => spriteSW,
                DirectionType.S => spriteS,
                DirectionType.SE => spriteSE,
                _ => spriteS // Fallback: Sud
            };
        }

        /// <summary>
        /// Direction par défaut pour l'état Idle.
        /// </summary>
        public DirectionType DefaultDirection => defaultDirection;

        /// <summary>
        /// Vérifie si tous les sprites sont assignés.
        /// Utile pour la validation dans l'éditeur.
        /// </summary>
        public bool AreAllSpritesAssigned()
        {
            return spriteE != null &&
                   spriteNE != null &&
                   spriteN != null &&
                   spriteNW != null &&
                   spriteW != null &&
                   spriteSW != null &&
                   spriteS != null &&
                   spriteSE != null;
        }

        #endregion

        #region Validation (Editor only)

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Vérifier que tous les sprites sont assignés
            if (!AreAllSpritesAssigned())
            {
                Debug.LogWarning($"[VehicleAnimationData] {name}: Certains sprites ne sont pas assignés!", this);
            }
        }
#endif

        #endregion
    }
}