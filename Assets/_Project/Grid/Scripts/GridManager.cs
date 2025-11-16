using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Grid
{
    /// <summary>
    /// Gère la grille logique du jeu pour le positionnement et le déplacement des unités.
    /// Chaque cellule fait 1.0 unité Unity (sprites 128px avec PPU=128).
    /// Les positions sont toujours centrées (+0.5f) pour un placement visuel optimal.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [Header("Grid Configuration")]
        [SerializeField] private int width = 20;
        [SerializeField] private int height = 20;

        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        [SerializeField] private Color gridColor = new Color(0, 1, 0, 0.2f);
        [SerializeField] private Color occupiedColor = new Color(1, 0, 0, 0.3f);

        private GridCell[,] cells;

        public int Width => width;
        public int Height => height;

        private void Awake()
        {
            InitializeGrid();
        }

        /// <summary>
        /// Initialise la grille avec les cellules
        /// </summary>
        private void InitializeGrid()
        {
            cells = new GridCell[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new GridCell(x, y);
                }
            }

            Debug.Log($"[GridManager] Grid initialized: {width}x{height} cells (cellSize=1.0 unity)");
        }

        /// <summary>
        /// Convertit une position grille en position monde (TOUJOURS CENTRÉE).
        /// Avec PPU=128 et cellSize=1.0, la conversion est ultra-simple.
        /// Le +0.5f centre l'objet dans la cellule.
        /// </summary>
        /// <param name="gridPosition">Position dans la grille</param>
        /// <returns>Position monde au centre de la cellule</returns>
        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, 0);
        }

        /// <summary>
        /// Convertit une position grille en position monde (surcharge)
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x + 0.5f, y + 0.5f, 0);
        }

        /// <summary>
        /// Convertit une position monde en position grille.
        /// Avec cellSize=1.0, c'est juste un FloorToInt (pas besoin de -0.5f).
        /// </summary>
        /// <param name="worldPosition">Position dans le monde</param>
        /// <returns>Position dans la grille</returns>
        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x);
            int y = Mathf.FloorToInt(worldPosition.y);
            return new GridPosition(x, y);
        }

        /// <summary>
        /// Vérifie si une position grille est valide (dans les limites de la grille)
        /// </summary>
        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridPosition.x >= 0 && gridPosition.x < width &&
                   gridPosition.y >= 0 && gridPosition.y < height;
        }

        /// <summary>
        /// Vérifie si une position grille est valide (surcharge)
        /// </summary>
        public bool IsValidGridPosition(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        /// <summary>
        /// Récupère une cellule de la grille
        /// </summary>
        public GridCell GetCell(GridPosition gridPosition)
        {
            if (!IsValidGridPosition(gridPosition))
            {
                Debug.LogWarning($"[GridManager] Invalid grid position: {gridPosition}");
                return null;
            }

            return cells[gridPosition.x, gridPosition.y];
        }

        /// <summary>
        /// Récupère une cellule de la grille (surcharge)
        /// </summary>
        public GridCell GetCell(int x, int y)
        {
            if (!IsValidGridPosition(x, y))
            {
                Debug.LogWarning($"[GridManager] Invalid grid position: ({x}, {y})");
                return null;
            }

            return cells[x, y];
        }

        /// <summary>
        /// Vérifie si une cellule est libre (non-occupée)
        /// </summary>
        public bool IsFree(GridPosition gridPosition)
        {
            GridCell cell = GetCell(gridPosition);
            return cell != null && !cell.IsOccupied;
        }

        /// <summary>
        /// Calcule la distance Manhattan entre deux positions grille
        /// (utile pour le pathfinding et la portée des unités)
        /// </summary>
        public int GetManhattanDistance(GridPosition from, GridPosition to)
        {
            return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
        }

        /// <summary>
        /// Affiche la grille en mode Debug (Gizmos)
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos)
                return;

            // Dessine les lignes de la grille
            Gizmos.color = gridColor;

            // Lignes verticales
            for (int x = 0; x <= width; x++)
            {
                Vector3 start = new Vector3(x, 0, 0);
                Vector3 end = new Vector3(x, height, 0);
                Gizmos.DrawLine(start, end);
            }

            // Lignes horizontales
            for (int y = 0; y <= height; y++)
            {
                Vector3 start = new Vector3(0, y, 0);
                Vector3 end = new Vector3(width, y, 0);
                Gizmos.DrawLine(start, end);
            }

            // Affiche les cellules occupées en rouge
            if (cells != null)
            {
                Gizmos.color = occupiedColor;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (cells[x, y].IsOccupied)
                        {
                            Vector3 center = GetWorldPosition(x, y);
                            Gizmos.DrawCube(center, new Vector3(0.9f, 0.9f, 0.1f));
                        }
                    }
                }
            }
        }
    }
}
