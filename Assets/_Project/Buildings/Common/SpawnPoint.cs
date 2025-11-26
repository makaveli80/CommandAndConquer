using System;
using System.Collections.Generic;
using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Buildings
{
    /// <summary>
    /// Manages the spawn point for units produced by a building.
    /// Calculates spawn position relative to building origin and validates cell availability.
    /// Phase 3: Spawn System
    /// Phase 3.5: Spawn Queue - queues units when spawn cell is blocked
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        #region Configuration

        [Header("Spawn Queue Settings")]
        [SerializeField]
        [Tooltip("How often to check if spawn cell is free (in seconds)")]
        private float retryInterval = 0.5f;

        [SerializeField]
        [Tooltip("Maximum number of units that can be queued")]
        private int maxQueueSize = 10;

        #endregion

        #region Dependencies

        private GridManager gridManager;
        private Building parentBuilding;

        #endregion

        #region Spawn Queue State

        private Queue<GameObject> spawnQueue = new Queue<GameObject>();
        private float nextRetryTime = 0f;

        #endregion

        #region Events

        /// <summary>
        /// Fired when a unit is added to the spawn queue (cell was blocked).
        /// </summary>
        public event Action<GameObject, int> OnUnitQueued;

        /// <summary>
        /// Fired when a queued unit successfully spawns.
        /// </summary>
        public event Action<GameObject, int> OnQueuedUnitSpawned;

        #endregion

        #region Properties

        /// <summary>
        /// Number of units waiting to spawn.
        /// </summary>
        public int QueueCount => spawnQueue.Count;

        /// <summary>
        /// Is the spawn queue empty?
        /// </summary>
        public bool HasQueuedUnits => spawnQueue.Count > 0;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Auto-discover dependencies
            gridManager = FindFirstObjectByType<GridManager>();
            parentBuilding = GetComponent<Building>();

            if (gridManager == null)
            {
                Debug.LogError("[SpawnPoint] GridManager not found in scene!");
            }

            if (parentBuilding == null)
            {
                Debug.LogError("[SpawnPoint] SpawnPoint must be attached to a Building!");
            }
        }

        private void Update()
        {
            // Process spawn queue periodically
            if (spawnQueue.Count > 0 && Time.time >= nextRetryTime)
            {
                TrySpawnFromQueue();
                nextRetryTime = Time.time + retryInterval;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Spawns a unit at the designated spawn point.
        /// If the spawn cell is blocked, the unit is queued and will spawn when the cell is free.
        /// </summary>
        /// <param name="unitPrefab">The unit prefab to spawn</param>
        /// <returns>True if spawned immediately, false if queued</returns>
        public bool SpawnUnit(GameObject unitPrefab)
        {
            if (unitPrefab == null)
            {
                Debug.LogError("[SpawnPoint] Cannot spawn null prefab!");
                return false;
            }

            if (parentBuilding == null || gridManager == null)
            {
                Debug.LogError("[SpawnPoint] Missing dependencies!");
                return false;
            }

            // Try to spawn immediately
            if (TrySpawnImmediate(unitPrefab))
            {
                return true;
            }

            // Cell is blocked - add to queue
            return EnqueueUnit(unitPrefab);
        }

        /// <summary>
        /// Clears all queued units (e.g., when building is destroyed).
        /// </summary>
        public void ClearQueue()
        {
            int count = spawnQueue.Count;
            spawnQueue.Clear();
            Debug.Log($"[SpawnPoint] Cleared {count} queued units");
        }

        /// <summary>
        /// Returns the current spawn position (useful for debugging).
        /// </summary>
        public GridPosition GetSpawnPosition()
        {
            return CalculateSpawnPosition();
        }

        #endregion

        #region Spawn Logic

        /// <summary>
        /// Attempts to spawn a unit immediately without queueing.
        /// </summary>
        private bool TrySpawnImmediate(GameObject unitPrefab)
        {
            GridPosition spawnPos = CalculateSpawnPosition();

            // Check if spawn cell is free
            if (!gridManager.IsFree(spawnPos))
            {
                return false;
            }

            // Spawn the unit
            Vector3 worldPos = gridManager.GetWorldPosition(spawnPos);
            GameObject unitObj = Instantiate(unitPrefab, worldPos, Quaternion.identity);

            Debug.Log($"[SpawnPoint] ✅ Spawned '{unitPrefab.name}' at grid {spawnPos} (world {worldPos})");
            return true;
        }

        /// <summary>
        /// Adds a unit to the spawn queue.
        /// </summary>
        private bool EnqueueUnit(GameObject unitPrefab)
        {
            // Check queue limit
            if (spawnQueue.Count >= maxQueueSize)
            {
                Debug.LogWarning($"[SpawnPoint] Spawn queue is full ({maxQueueSize})! Cannot queue '{unitPrefab.name}'");
                return false;
            }

            // Add to queue
            spawnQueue.Enqueue(unitPrefab);
            Debug.Log($"[SpawnPoint] 📦 Queued '{unitPrefab.name}' (queue size: {spawnQueue.Count})");

            // Fire event
            OnUnitQueued?.Invoke(unitPrefab, spawnQueue.Count);

            return true;
        }

        /// <summary>
        /// Tries to spawn the next unit from the queue.
        /// </summary>
        private void TrySpawnFromQueue()
        {
            if (spawnQueue.Count == 0)
                return;

            GridPosition spawnPos = CalculateSpawnPosition();

            // Check if spawn cell is now free
            if (!gridManager.IsFree(spawnPos))
            {
                // Still blocked, will retry later
                return;
            }

            // Spawn the next unit
            GameObject unitPrefab = spawnQueue.Dequeue();
            Vector3 worldPos = gridManager.GetWorldPosition(spawnPos);
            GameObject unitObj = Instantiate(unitPrefab, worldPos, Quaternion.identity);

            Debug.Log($"[SpawnPoint] ✅ Spawned queued '{unitPrefab.name}' at grid {spawnPos} (queue: {spawnQueue.Count} remaining)");

            // Fire event
            OnQueuedUnitSpawned?.Invoke(unitPrefab, spawnQueue.Count);
        }

        /// <summary>
        /// Calculates the spawn position in grid coordinates.
        /// Position = building origin + spawn offset from BuildingData.
        /// </summary>
        private GridPosition CalculateSpawnPosition()
        {
            GridPosition buildingOrigin = parentBuilding.OriginPosition;
            Vector2Int spawnOffset = parentBuilding.Data.spawnOffset;

            return new GridPosition(
                buildingOrigin.x + spawnOffset.x,
                buildingOrigin.y + spawnOffset.y
            );
        }

        #endregion

        #region Debug

        /// <summary>
        /// Visualizes the spawn point in Scene view.
        /// Shows a green circle at the spawn location.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (parentBuilding == null || parentBuilding.Data == null)
                return;

            // Calculate spawn position
            GridPosition spawnPos = CalculateSpawnPosition();

            // Check if valid
            bool isValid = gridManager != null && gridManager.IsValidGridPosition(spawnPos);
            bool isFree = isValid && gridManager.IsFree(spawnPos);

            // Draw spawn point indicator (color based on queue state)
            if (spawnQueue.Count > 0)
            {
                Gizmos.color = new Color(1, 1, 0, 0.8f); // Yellow if queue has units
            }
            else
            {
                Gizmos.color = isFree ? new Color(0, 1, 0, 0.8f) : new Color(1, 0.5f, 0, 0.8f); // Green if free, orange if occupied
            }

            Vector3 worldPos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
            Gizmos.DrawWireSphere(worldPos, 0.4f);
            Gizmos.DrawLine(worldPos + Vector3.up * 0.4f, worldPos + Vector3.up * 0.7f); // Arrow up

            // Draw line from building center to spawn point
            if (parentBuilding.Data != null)
            {
                GridPosition origin = parentBuilding.OriginPosition;
                Vector3 buildingCenter = new Vector3(
                    origin.x + parentBuilding.Data.width / 2f,
                    origin.y + parentBuilding.Data.height / 2f,
                    0
                );

                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawLine(buildingCenter, worldPos);
            }
        }

        /// <summary>
        /// Debug UI showing spawn queue status.
        /// </summary>
        private void OnGUI()
        {
            if (!Application.isPlaying || spawnQueue.Count == 0)
                return;

            // Display spawn queue in bottom-left corner
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.yellow;

            string statusText = $"📦 Spawn Queue: {spawnQueue.Count} unit(s) waiting";
            GUI.Label(new Rect(10, Screen.height - 30, 400, 30), statusText, style);
        }

        #endregion
    }
}
