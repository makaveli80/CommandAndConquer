using System.Collections.Generic;
using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Grid
{
    /// <summary>
    /// Gère la grille logique du jeu pour le positionnement et le déplacement des unités.
    /// Chaque cellule fait 1.0 unité Unity (sprites 128px avec PPU=128).
    /// Les positions sont toujours centrées (+0.5f) pour un placement visuel optimal.
    ///
    /// Utilise un système de surveillance autonome (LateUpdate) pour gérer automatiquement
    /// les occupations de cellules sans intervention des unités.
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

        // Tracking des unités (position grille connue par GridManager)
        private Dictionary<UnitBase, GridPosition> unitPositions = new Dictionary<UnitBase, GridPosition>();

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

        #region Unit Registration (Autonomous System)

        /// <summary>
        /// Enregistre une unité sur la grille à une position donnée.
        /// Appelé une seule fois au spawn de l'unité.
        /// </summary>
        public bool RegisterUnit(UnitBase unit, GridPosition position)
        {
            if (!IsValidGridPosition(position))
            {
                Debug.LogError($"[GridManager] Cannot register unit at invalid position {position}");
                return false;
            }

            GridCell cell = GetCell(position);
            if (!cell.TryOccupy(unit))
            {
                Debug.LogWarning($"[GridManager] Cannot register unit at occupied cell {position}");
                return false;
            }

            unitPositions[unit] = position;
            Debug.Log($"[GridManager] Registered {unit.name} at {position}");
            return true;
        }

        /// <summary>
        /// Désenregistre une unité de la grille (destruction).
        /// </summary>
        public void UnregisterUnit(UnitBase unit)
        {
            if (unitPositions.TryGetValue(unit, out GridPosition position))
            {
                GridCell cell = GetCell(position);
                cell?.Release();
                unitPositions.Remove(unit);
                Debug.Log($"[GridManager] Unregistered {unit.name} from {position}");
            }
        }

        /// <summary>
        /// Vérifie si une cellule est libre (peut être occupée par l'unité demandant).
        /// C'est la SEULE méthode que les unités doivent appeler.
        /// </summary>
        public bool IsCellAvailableFor(GridPosition position, UnitBase requestingUnit)
        {
            if (!IsValidGridPosition(position))
                return false;

            GridCell cell = GetCell(position);

            // Libre ou occupée par l'unité elle-même
            return !cell.IsOccupied || cell.OccupyingUnit == requestingUnit;
        }

        /// <summary>
        /// Récupère la position grille d'une unité selon le GridManager.
        /// Utile pour le debug.
        /// </summary>
        public GridPosition GetUnitGridPosition(UnitBase unit)
        {
            if (unitPositions.TryGetValue(unit, out GridPosition position))
                return position;

            return new GridPosition(-1, -1); // Invalid
        }

        /// <summary>
        /// Compte le nombre d'unités enregistrées.
        /// </summary>
        public int GetRegisteredUnitCount()
        {
            return unitPositions.Count;
        }

        #endregion

        #region Autonomous Monitoring System

        /// <summary>
        /// Surveille les positions des unités et met à jour automatiquement les occupations.
        /// Utilise LateUpdate pour s'exécuter APRÈS les mouvements des unités.
        /// </summary>
        private void LateUpdate()
        {
            // Copier la liste des clés pour éviter les modifications pendant l'itération
            List<UnitBase> units = new List<UnitBase>(unitPositions.Keys);

            foreach (UnitBase unit in units)
            {
                // Vérifier que l'unité existe toujours
                if (unit == null)
                {
                    // Nettoyage automatique des unités détruites
                    unitPositions.Remove(unit);
                    continue;
                }

                // Récupérer la position actuelle de l'unité dans le monde
                Vector3 worldPos = unit.transform.position;
                GridPosition currentGridPos = GetGridPosition(worldPos);

                // Récupérer la dernière position connue par le GridManager
                GridPosition lastKnownPos = unitPositions[unit];

                // Détecter si l'unité a changé de cellule
                if (currentGridPos != lastKnownPos)
                {
                    // Vérifier que la position est valide
                    if (IsValidGridPosition(currentGridPos))
                    {
                        AutoUpdateUnitPosition(unit, lastKnownPos, currentGridPos);
                    }
                    else
                    {
                        Debug.LogWarning($"[GridManager] {unit.name} moved to invalid position {currentGridPos}");
                    }
                }
            }
        }

        /// <summary>
        /// Met à jour automatiquement la position d'une unité dans le système de grille.
        /// Gère la libération de l'ancienne cellule et l'occupation de la nouvelle.
        /// </summary>
        private void AutoUpdateUnitPosition(UnitBase unit, GridPosition oldPos, GridPosition newPos)
        {
            // Libérer l'ancienne cellule
            GridCell oldCell = GetCell(oldPos);
            if (oldCell != null && oldCell.OccupyingUnit == unit)
            {
                oldCell.Release();
            }

            // Occuper la nouvelle cellule
            GridCell newCell = GetCell(newPos);
            if (newCell != null)
            {
                if (newCell.TryOccupy(unit))
                {
                    // Succès - mettre à jour le tracking
                    unitPositions[unit] = newPos;
                    Debug.Log($"[GridManager] Auto-updated {unit.name}: {oldPos} → {newPos}");
                }
                else
                {
                    // COLLISION! Une autre unité occupe déjà cette cellule
                    Debug.LogWarning($"[GridManager] COLLISION: {unit.name} at {newPos} - cell occupied by {newCell.OccupyingUnit?.name}");

                    // Mettre à jour le tracking quand même pour maintenir la synchronisation
                    // Cela évite que le GridManager perde la trace de la position réelle de l'unité
                    unitPositions[unit] = newPos;
                }
            }
        }

        #endregion

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
