using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Grid
{
    /// <summary>
    /// Représente une cellule individuelle de la grille.
    /// Chaque cellule peut être occupée par une unité ou libre.
    /// Utilise MonoBehaviour pour éviter dépendance cyclique avec Units assembly.
    /// </summary>
    public class GridCell
    {
        private GridPosition gridPosition;
        private MonoBehaviour occupyingUnit;

        public GridPosition GridPosition => gridPosition;
        public bool IsOccupied => occupyingUnit != null;
        public MonoBehaviour OccupyingUnit => occupyingUnit;

        public GridCell(int x, int y)
        {
            gridPosition = new GridPosition(x, y);
            occupyingUnit = null;
        }

        /// <summary>
        /// Tente d'occuper cette cellule avec une unité
        /// </summary>
        /// <param name="unit">L'unité qui veut occuper la cellule</param>
        /// <returns>True si l'occupation a réussi, false sinon</returns>
        public bool TryOccupy(MonoBehaviour unit)
        {
            if (IsOccupied)
                return false;

            occupyingUnit = unit;
            return true;
        }

        /// <summary>
        /// Libère cette cellule (retire l'unité occupante)
        /// </summary>
        public void Release()
        {
            occupyingUnit = null;
        }

        /// <summary>
        /// Force l'occupation de cette cellule (même si déjà occupée)
        /// </summary>
        public void ForceOccupy(MonoBehaviour unit)
        {
            occupyingUnit = unit;
        }

        public override string ToString()
        {
            return $"GridCell({gridPosition.x}, {gridPosition.y}) - Occupied: {IsOccupied}";
        }
    }
}
