using System.Collections.Generic;
using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Buildings
{
    /// <summary>
    /// Composant générique pour tous les bâtiments du jeu.
    /// Gère l'occupation multi-cellule sur la grille, la position, et les données de configuration.
    /// Architecture 100% composition - aucun héritage spécifique requis.
    /// </summary>
    public class Building : MonoBehaviour
    {
        #region Configuration

        [Header("Building Configuration")]
        [SerializeField]
        [Tooltip("Données de configuration du bâtiment (ScriptableObject)")]
        private BuildingData buildingData;

        #endregion

        #region Runtime State

        private GridPosition originPosition;           // Cellule d'origine (bas-gauche)
        private List<GridPosition> occupiedCells;      // Toutes les cellules occupées
        private GridManager gridManager;
        private ProductionQueue productionQueue;       // Phase 2: Production system

        #endregion

        #region Events

        /// <summary>
        /// Déclenché quand un item de production est terminé.
        /// Phase 2: Production System
        /// </summary>
        public event System.Action<ProductionItem> OnProductionCompleted;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Auto-découverte du GridManager
            gridManager = FindFirstObjectByType<GridManager>();

            if (gridManager == null)
            {
                Debug.LogError($"[Building] GridManager not found in scene!");
            }

            // Auto-découverte du ProductionQueue (Phase 2)
            productionQueue = GetComponent<ProductionQueue>();

            occupiedCells = new List<GridPosition>();
        }

        private void Start()
        {
            Initialize();
            SetupProductionQueue();
        }

        private void OnDestroy()
        {
            // Libérer toutes les cellules occupées
            if (gridManager != null && occupiedCells != null)
            {
                gridManager.ReleaseBuildingCells(this);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialise le bâtiment au démarrage.
        /// CONVENTION : Le sprite a son pivot en Bottom Left (0,0).
        /// Donc transform.position = coin bas-gauche = origine du bâtiment directement !
        /// </summary>
        private void Initialize()
        {
            if (gridManager == null)
            {
                Debug.LogError($"[Building] Cannot initialize without GridManager!");
                return;
            }

            if (buildingData == null)
            {
                Debug.LogError($"[Building] BuildingData is null!");
                return;
            }

            // Convention : Sprite avec pivot Bottom Left (0,0)
            // → transform.position = origine (coin bas-gauche) directement !
            originPosition = gridManager.GetGridPosition(transform.position);

            // Tenter d'occuper toutes les cellules
            if (!gridManager.TryOccupyBuildingCells(this, originPosition, buildingData.width, buildingData.height))
            {
                Debug.LogError($"[Building] Failed to occupy cells for '{BuildingName}' at {originPosition}");
                return;
            }

            Debug.Log($"[Building] '{BuildingName}' initialized at origin {originPosition} ({buildingData.width}×{buildingData.height})");
        }

        /// <summary>
        /// Calcule la position world du centre du bâtiment.
        /// Pour un bâtiment 4×2 à l'origine (5,9), le centre est à (7, 10).
        /// </summary>
        private Vector3 CalculateBuildingCenter()
        {
            // Centre = origin + (width/2, height/2) en coordonnées grid
            float centerX = originPosition.x + buildingData.width / 2f;
            float centerY = originPosition.y + buildingData.height / 2f;

            // Retourner la position world (sans +0.5f car on veut le centre exact entre les cellules)
            return new Vector3(centerX, centerY, 0);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Position d'origine du bâtiment (cellule bas-gauche).
        /// </summary>
        public GridPosition OriginPosition => originPosition;

        /// <summary>
        /// Liste de toutes les cellules occupées par ce bâtiment.
        /// </summary>
        public GridPosition[] OccupiedCells => occupiedCells.ToArray();

        /// <summary>
        /// Données de configuration du bâtiment (ScriptableObject).
        /// </summary>
        public BuildingData Data => buildingData;

        /// <summary>
        /// Nom du bâtiment (depuis BuildingData).
        /// </summary>
        public string BuildingName => buildingData != null ? buildingData.buildingName : "Unknown";

        /// <summary>
        /// Référence au GridManager (pour les composants qui en ont besoin).
        /// </summary>
        public GridManager GridManager => gridManager;

        /// <summary>
        /// Met à jour la liste des cellules occupées.
        /// Appelé par GridManager après l'occupation réussie.
        /// </summary>
        public void SetOccupiedCells(List<GridPosition> cells)
        {
            occupiedCells = cells;
        }

        #endregion

        #region Production System (Phase 2)

        /// <summary>
        /// Configure le système de production et connecte les events.
        /// Phase 2: Production System
        /// </summary>
        private void SetupProductionQueue()
        {
            if (productionQueue == null)
            {
                // Pas de ProductionQueue sur ce bâtiment (normal pour certains types)
                return;
            }

            // Connecter les events
            productionQueue.OnItemCompleted += HandleProductionCompleted;
            productionQueue.OnItemStarted += (item) => Debug.Log($"[Building] '{BuildingName}' started producing '{item.itemName}'");
            productionQueue.OnProgressUpdated += (item, progress) => { }; // Silent pour éviter spam

            Debug.Log($"[Building] '{BuildingName}' production system ready");
        }

        /// <summary>
        /// Ajoute un item à la file de production.
        /// Phase 2: Production System
        /// </summary>
        public void AddToProductionQueue(ProductionItem item)
        {
            if (productionQueue == null)
            {
                Debug.LogWarning($"[Building] '{BuildingName}' has no ProductionQueue component!");
                return;
            }

            if (item == null)
            {
                Debug.LogError($"[Building] Cannot add null item to production queue!");
                return;
            }

            productionQueue.AddToQueue(item);
            Debug.Log($"[Building] '{BuildingName}' added '{item.itemName}' to production queue");
        }

        /// <summary>
        /// Appelé quand un item de production est terminé.
        /// Phase 2: Logs uniquement. Phase 3: Spawn des unités.
        /// </summary>
        private void HandleProductionCompleted(ProductionItem item)
        {
            Debug.Log($"[Building] ✅ '{BuildingName}' completed production of '{item.itemName}'!");

            // Déclencher l'event public
            OnProductionCompleted?.Invoke(item);

            // Phase 3: SpawnPoint spawning sera ajouté ici
            // if (!item.isBuilding)
            // {
            //     spawnPoint?.SpawnUnit(item.prefab);
            // }
        }

        /// <summary>
        /// Accès public au ProductionQueue (pour l'UI en Phase 5).
        /// </summary>
        public ProductionQueue ProductionQueue => productionQueue;

        #endregion

        #region Debug

        /// <summary>
        /// Affiche les cellules occupées et le centre du bâtiment en mode Debug (Gizmos)
        /// </summary>
        private void OnDrawGizmos()
        {
            if (buildingData == null || occupiedCells == null || occupiedCells.Count == 0)
                return;

            // Dessiner les cellules occupées en bleu transparent
            Gizmos.color = new Color(0, 0, 1, 0.3f);
            foreach (GridPosition pos in occupiedCells)
            {
                Vector3 cellCenter = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0);
                Gizmos.DrawCube(cellCenter, new Vector3(0.9f, 0.9f, 0.1f));
            }

            // Dessiner le centre du bâtiment en jaune
            Gizmos.color = Color.yellow;
            Vector3 center = CalculateBuildingCenter();
            Gizmos.DrawWireSphere(center, 0.3f);

            // Dessiner l'origine en vert
            Gizmos.color = Color.green;
            Vector3 origin = new Vector3(originPosition.x + 0.5f, originPosition.y + 0.5f, 0);
            Gizmos.DrawWireCube(origin, new Vector3(0.5f, 0.5f, 0.1f));
        }

        #endregion
    }
}
