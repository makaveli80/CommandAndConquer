namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Énumération des 8 directions cardinales et intercardinales.
    /// Utilisé pour l'animation des véhicules.
    /// </summary>
    public enum DirectionType
    {
        E = 0,    // East (Right) - 0°
        NE = 1,   // Northeast - 45°
        N = 2,    // North (Up) - 90°
        NW = 3,   // Northwest - 135°
        W = 4,    // West (Left) - 180°
        SW = 5,   // Southwest - 225°
        S = 6,    // South (Down) - 270°
        SE = 7    // Southeast - 315°
    }
}