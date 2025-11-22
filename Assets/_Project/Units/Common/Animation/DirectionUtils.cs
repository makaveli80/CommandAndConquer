using UnityEngine;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Utilitaire statique pour calculer les directions à partir de vecteurs de mouvement.
    /// Convertit un vecteur de déplacement en une des 8 directions cardinales/intercardinales.
    /// </summary>
    public static class DirectionUtils
    {
        /// <summary>
        /// Calcule la direction cardinale/intercardinale à partir d'un vecteur de déplacement.
        /// </summary>
        /// <param name="delta">Vecteur de déplacement (non-normalisé acceptable)</param>
        /// <returns>Direction correspondante (E, NE, N, NW, W, SW, S, SE)</returns>
        public static DirectionType GetDirectionFromDelta(Vector2 delta)
        {
            // Si le vecteur est nul, retourner une direction par défaut
            if (delta.sqrMagnitude < 0.001f)
            {
                return DirectionType.S; // Par défaut: Sud (sprite de face)
            }

            // Calculer l'angle en degrés (0° = Est, sens anti-horaire)
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            // Normaliser l'angle entre 0 et 360
            if (angle < 0)
            {
                angle += 360f;
            }

            // Mapper l'angle à une des 8 directions
            // Chaque direction couvre 45° (360° / 8)
            // Décalage de 22.5° pour centrer les plages

            // E: 337.5° - 22.5° (ou -22.5° - 22.5°)
            if (angle >= 337.5f || angle < 22.5f)
                return DirectionType.E;

            // NE: 22.5° - 67.5°
            if (angle >= 22.5f && angle < 67.5f)
                return DirectionType.NE;

            // N: 67.5° - 112.5°
            if (angle >= 67.5f && angle < 112.5f)
                return DirectionType.N;

            // NW: 112.5° - 157.5°
            if (angle >= 112.5f && angle < 157.5f)
                return DirectionType.NW;

            // W: 157.5° - 202.5°
            if (angle >= 157.5f && angle < 202.5f)
                return DirectionType.W;

            // SW: 202.5° - 247.5°
            if (angle >= 202.5f && angle < 247.5f)
                return DirectionType.SW;

            // S: 247.5° - 292.5°
            if (angle >= 247.5f && angle < 292.5f)
                return DirectionType.S;

            // SE: 292.5° - 337.5°
            return DirectionType.SE;
        }

        /// <summary>
        /// Calcule la direction à partir de deux positions (from → to).
        /// </summary>
        /// <param name="from">Position de départ</param>
        /// <param name="to">Position d'arrivée</param>
        /// <returns>Direction du déplacement</returns>
        public static DirectionType GetDirectionFromPositions(Vector3 from, Vector3 to)
        {
            Vector2 delta = new Vector2(to.x - from.x, to.y - from.y);
            return GetDirectionFromDelta(delta);
        }

        /// <summary>
        /// Obtient l'angle en degrés correspondant à une direction.
        /// Utile pour le debug ou l'affichage.
        /// </summary>
        /// <param name="direction">Direction</param>
        /// <returns>Angle en degrés (0° = Est)</returns>
        public static float GetAngleFromDirection(DirectionType direction)
        {
            return direction switch
            {
                DirectionType.E => 0f,
                DirectionType.NE => 45f,
                DirectionType.N => 90f,
                DirectionType.NW => 135f,
                DirectionType.W => 180f,
                DirectionType.SW => 225f,
                DirectionType.S => 270f,
                DirectionType.SE => 315f,
                _ => 0f
            };
        }
    }
}