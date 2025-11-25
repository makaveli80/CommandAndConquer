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
        // Utilise MonoBehaviour pour éviter dépendance cyclique avec Units assembly
        private Dictionary<MonoBehaviour, GridPosition> unitPositions = new Dictionary<MonoBehaviour, GridPosition>();

        // Tracking des bâtiments (multi-cellules)
        // Associe chaque bâtiment à la liste de toutes les cellules qu'il occupe
        private Dictionary<MonoBehaviour, List<GridPosition>> buildingCells = new Dictionary<MonoBehaviour, List<GridPosition>>();

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
        public bool RegisterUnit(MonoBehaviour unit, GridPosition position)
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
        public void UnregisterUnit(MonoBehaviour unit)
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
        /// ATTENTION: Cette méthode ne réserve PAS la cellule. Utiliser TryMoveUnitTo() pour une opération atomique.
        /// </summary>
        public bool IsCellAvailableFor(GridPosition position, MonoBehaviour requestingUnit)
        {
            if (!IsValidGridPosition(position))
                return false;

            GridCell cell = GetCell(position);

            // Libre ou occupée par l'unité elle-même
            return !cell.IsOccupied || cell.OccupyingUnit == requestingUnit;
        }

        /// <summary>
        /// Tente de déplacer une unité vers une nouvelle position de manière atomique.
        /// Libère l'ancienne cellule et occupe la nouvelle en une seule opération.
        /// Évite les race conditions où plusieurs unités tentent d'occuper la même cellule.
        /// </summary>
        /// <returns>True si le mouvement a réussi, False si la cellule cible est occupée</returns>
        public bool TryMoveUnitTo(MonoBehaviour unit, GridPosition newPos)
        {
            if (!IsValidGridPosition(newPos))
            {
                Debug.LogWarning($"[GridManager] Cannot move {unit.name} to invalid position {newPos}");
                return false;
            }

            // Récupérer l'ancienne position
            if (!unitPositions.TryGetValue(unit, out GridPosition oldPos))
            {
                Debug.LogWarning($"[GridManager] Unit {unit.name} not registered");
                return false;
            }

            // Si déjà à cette position, succès immédiat
            if (oldPos == newPos)
                return true;

            // Tenter d'occuper la nouvelle cellule
            GridCell newCell = GetCell(newPos);
            if (newCell == null)
                return false;

            // Vérifier si la cellule est disponible (libre ou occupée par cette unité)
            if (newCell.IsOccupied && newCell.OccupyingUnit != unit)
            {
                Debug.Log($"[GridManager] Cell {newPos} occupied by {newCell.OccupyingUnit?.name}, cannot move {unit.name}");
                return false;
            }

            // Libérer l'ancienne cellule
            GridCell oldCell = GetCell(oldPos);
            if (oldCell != null && oldCell.OccupyingUnit == unit)
            {
                oldCell.Release();
            }

            // Occuper la nouvelle cellule (atomique!)
            if (newCell.TryOccupy(unit))
            {
                unitPositions[unit] = newPos;
                Debug.Log($"[GridManager] Moved {unit.name}: {oldPos} → {newPos}");
                return true;
            }
            else
            {
                // Échec - remettre l'ancienne occupation
                oldCell?.TryOccupy(unit);
                Debug.LogWarning($"[GridManager] Failed to occupy {newPos} for {unit.name}");
                return false;
            }
        }

        /// <summary>
        /// Récupère la position grille d'une unité selon le GridManager.
        /// Utile pour le debug.
        /// </summary>
        public GridPosition GetUnitGridPosition(MonoBehaviour unit)
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

        #region Building Support (Multi-cell)

        /// <summary>
        /// Vérifie si un bâtiment peut être placé à une position donnée.
        /// Vérifie que toutes les cellules nécessaires (width × height) sont libres et valides.
        /// </summary>
        /// <param name="origin">Position d'origine (bas-gauche du bâtiment)</param>
        /// <param name="width">Largeur en cellules</param>
        /// <param name="height">Hauteur en cellules</param>
        /// <returns>True si toutes les cellules sont disponibles</returns>
        public bool CanPlaceBuilding(GridPosition origin, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridPosition pos = new GridPosition(origin.x + x, origin.y + y);

                    if (!IsValidGridPosition(pos) || !IsFree(pos))
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tente d'occuper toutes les cellules d'un bâtiment de manière atomique.
        /// Si une seule cellule ne peut pas être occupée, l'opération entière échoue
        /// et toutes les cellules déjà occupées sont libérées (rollback).
        /// </summary>
        /// <param name="building">Le bâtiment à placer</param>
        /// <param name="origin">Position d'origine (bas-gauche)</param>
        /// <param name="width">Largeur en cellules</param>
        /// <param name="height">Hauteur en cellules</param>
        /// <returns>True si toutes les cellules ont été occupées avec succès</returns>
        public bool TryOccupyBuildingCells(MonoBehaviour building, GridPosition origin, int width, int height)
        {
            List<GridPosition> cells = new List<GridPosition>();

            // Collecter toutes les cellules nécessaires
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells.Add(new GridPosition(origin.x + x, origin.y + y));
                }
            }

            // Tenter d'occuper toutes les cellules
            foreach (GridPosition pos in cells)
            {
                if (!IsValidGridPosition(pos))
                {
                    // Position invalide - rollback
                    RollbackBuildingOccupation(building, cells, pos);
                    Debug.LogWarning($"[GridManager] Cannot place building at {origin} - position {pos} is invalid");
                    return false;
                }

                GridCell cell = GetCell(pos);
                if (!cell.TryOccupy(building))
                {
                    // Cellule déjà occupée - rollback
                    RollbackBuildingOccupation(building, cells, pos);
                    Debug.LogWarning($"[GridManager] Cannot place building at {origin} - cell {pos} is occupied");
                    return false;
                }
            }

            // Succès - enregistrer toutes les cellules
            buildingCells[building] = cells;

            // Notifier le bâtiment des cellules occupées
            // Note: On utilise la réflexion pour éviter une dépendance cyclique avec Buildings assembly
            var method = building.GetType().GetMethod("SetOccupiedCells");
            if (method != null)
            {
                method.Invoke(building, new object[] { cells });
            }
            else
            {
                Debug.LogWarning($"[GridManager] SetOccupiedCells method not found on {building.GetType().Name}");
            }

            Debug.Log($"[GridManager] Building placed at {origin} ({width}×{height}), occupying {cells.Count} cells");
            return true;
        }

        /// <summary>
        /// Libère partiellement les cellules lors d'un rollback (opération échouée).
        /// </summary>
        private void RollbackBuildingOccupation(MonoBehaviour building, List<GridPosition> cells, GridPosition failedAt)
        {
            foreach (GridPosition pos in cells)
            {
                // Arrêter quand on atteint la cellule qui a échoué
                if (pos == failedAt)
                    break;

                GridCell cell = GetCell(pos);
                if (cell != null && cell.OccupyingUnit == building)
                {
                    cell.Release();
                }
            }
        }

        /// <summary>
        /// Libère toutes les cellules occupées par un bâtiment.
        /// Appelé quand le bâtiment est détruit ou déplacé.
        /// </summary>
        /// <param name="building">Le bâtiment dont les cellules doivent être libérées</param>
        public void ReleaseBuildingCells(MonoBehaviour building)
        {
            if (buildingCells.TryGetValue(building, out List<GridPosition> cells))
            {
                foreach (GridPosition pos in cells)
                {
                    GridCell cell = GetCell(pos);
                    if (cell != null && cell.OccupyingUnit == building)
                    {
                        cell.Release();
                    }
                }
                buildingCells.Remove(building);
                Debug.Log($"[GridManager] Released {cells.Count} cells for building {building.name}");
            }
        }

        /// <summary>
        /// Récupère le nombre de bâtiments enregistrés.
        /// </summary>
        public int GetRegisteredBuildingCount()
        {
            return buildingCells.Count;
        }

        #endregion

        #region Grid Coherence Verification (Safety Net)

        /// <summary>
        /// Vérifie périodiquement la cohérence de la grille comme "filet de sécurité".
        /// Détecte les incohérences sans interférer avec le fonctionnement normal.
        /// </summary>
        private void LateUpdate()
        {
            // Nettoyage automatique des unités détruites
            CleanupDestroyedUnits();

            // Vérification de cohérence (pas à chaque frame pour la performance)
            if (Time.frameCount % 60 == 0) // Toutes les 60 frames (~1 seconde)
            {
                VerifyGridCoherence();
            }
        }

        /// <summary>
        /// Nettoie automatiquement les unités et bâtiments détruits du tracking.
        /// </summary>
        private void CleanupDestroyedUnits()
        {
            // Nettoyer les unités détruites
            List<MonoBehaviour> destroyedUnits = new List<MonoBehaviour>();

            foreach (var kvp in unitPositions)
            {
                if (kvp.Key == null)
                {
                    destroyedUnits.Add(kvp.Key);
                }
            }

            foreach (var unit in destroyedUnits)
            {
                if (unitPositions.TryGetValue(unit, out GridPosition pos))
                {
                    GridCell cell = GetCell(pos);
                    cell?.Release();
                    unitPositions.Remove(unit);
                    Debug.LogWarning($"[GridManager] Auto-cleaned destroyed unit at {pos}");
                }
            }

            // Nettoyer les bâtiments détruits
            List<MonoBehaviour> destroyedBuildings = new List<MonoBehaviour>();

            foreach (var kvp in buildingCells)
            {
                if (kvp.Key == null)
                {
                    destroyedBuildings.Add(kvp.Key);
                }
            }

            foreach (var building in destroyedBuildings)
            {
                if (buildingCells.TryGetValue(building, out List<GridPosition> cells))
                {
                    foreach (GridPosition pos in cells)
                    {
                        GridCell cell = GetCell(pos);
                        cell?.Release();
                    }
                    buildingCells.Remove(building);
                    Debug.LogWarning($"[GridManager] Auto-cleaned destroyed building ({cells.Count} cells)");
                }
            }
        }

        /// <summary>
        /// Vérifie la cohérence entre le tracking des unités/bâtiments et l'état de la grille.
        /// Log des warnings si des incohérences sont détectées.
        /// </summary>
        private void VerifyGridCoherence()
        {
            // Vérification 1: Chaque unité enregistrée occupe-t-elle bien sa cellule?
            foreach (var kvp in unitPositions)
            {
                MonoBehaviour unit = kvp.Key;
                GridPosition trackedPos = kvp.Value;

                if (unit == null)
                    continue;

                GridCell cell = GetCell(trackedPos);
                if (cell != null)
                {
                    if (!cell.IsOccupied)
                    {
                        Debug.LogError($"[GridManager] COHERENCE ERROR: Unit {unit.name} tracked at {trackedPos} but cell is not occupied!");
                    }
                    else if (cell.OccupyingUnit != unit)
                    {
                        Debug.LogError($"[GridManager] COHERENCE ERROR: Unit {unit.name} tracked at {trackedPos} but cell is occupied by {cell.OccupyingUnit?.name}!");
                    }
                }
            }

            // Vérification 1b: Chaque bâtiment enregistré occupe-t-il bien ses cellules?
            foreach (var kvp in buildingCells)
            {
                MonoBehaviour building = kvp.Key;
                List<GridPosition> cells = kvp.Value;

                if (building == null)
                    continue;

                foreach (GridPosition pos in cells)
                {
                    GridCell cell = GetCell(pos);
                    if (cell != null)
                    {
                        if (!cell.IsOccupied)
                        {
                            Debug.LogError($"[GridManager] COHERENCE ERROR: Building {building.name} should occupy {pos} but cell is not occupied!");
                        }
                        else if (cell.OccupyingUnit != building)
                        {
                            Debug.LogError($"[GridManager] COHERENCE ERROR: Building {building.name} should occupy {pos} but cell is occupied by {cell.OccupyingUnit?.name}!");
                        }
                    }
                }
            }

            // Vérification 2: Chaque cellule occupée correspond-elle à une unité ou un bâtiment enregistré?
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GridCell cell = cells[x, y];
                    if (cell.IsOccupied)
                    {
                        MonoBehaviour occupyingUnit = cell.OccupyingUnit;
                        if (occupyingUnit == null)
                        {
                            Debug.LogError($"[GridManager] COHERENCE ERROR: Cell ({x},{y}) is occupied but occupier is null!");
                            cell.Release(); // Auto-correction
                        }
                        else
                        {
                            // Vérifier si c'est une unité ou un bâtiment
                            bool isRegisteredUnit = unitPositions.ContainsKey(occupyingUnit);
                            bool isRegisteredBuilding = buildingCells.ContainsKey(occupyingUnit);

                            if (!isRegisteredUnit && !isRegisteredBuilding)
                            {
                                Debug.LogError($"[GridManager] COHERENCE ERROR: Cell ({x},{y}) occupied by {occupyingUnit.name} but not registered as unit or building!");
                            }
                            else if (isRegisteredUnit)
                            {
                                // Vérification unité
                                if (unitPositions[occupyingUnit] != new GridPosition(x, y))
                                {
                                    Debug.LogError($"[GridManager] COHERENCE ERROR: Cell ({x},{y}) occupied by unit {occupyingUnit.name} but unit is tracked at {unitPositions[occupyingUnit]}!");
                                }
                            }
                            else if (isRegisteredBuilding)
                            {
                                // Vérification bâtiment
                                GridPosition cellPos = new GridPosition(x, y);
                                List<GridPosition> buildingCellsList = buildingCells[occupyingUnit];

                                if (buildingCellsList == null)
                                {
                                    Debug.LogError($"[GridManager] COHERENCE ERROR: Building {occupyingUnit.name} registered but cell list is null!");
                                }
                                else if (!buildingCellsList.Contains(cellPos))
                                {
                                    Debug.LogError($"[GridManager] COHERENCE ERROR: Cell ({x},{y}) occupied by building {occupyingUnit.name} but not in building's cell list!");
                                }
                            }
                        }
                    }
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
