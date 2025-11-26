using UnityEngine;
using UnityEngine.InputSystem;

namespace CommandAndConquer.Buildings
{
    /// <summary>
    /// Test script for Phase 2 Production System.
    /// Add this component to a Building to test production via keyboard shortcuts.
    ///
    /// Controls:
    /// - Press '1' to queue a Buggy (8s production)
    /// - Press '2' to queue an Artillery (15s production)
    /// - Press 'C' to cancel current production
    /// - Press 'Q' to clear queue
    ///
    /// NOTE: This is a temporary test script for Phase 2.
    /// It will be removed when the UI Production Panel is implemented in Phase 5.
    /// </summary>
    public class ProductionQueueTester : MonoBehaviour
    {
        [Header("Production Items")]
        [SerializeField]
        [Tooltip("ProductionItem for Buggy (assign BuggyProductionItem.asset)")]
        private ProductionItem buggyItem;

        [SerializeField]
        [Tooltip("ProductionItem for Artillery (assign ArtilleryProductionItem.asset)")]
        private ProductionItem artilleryItem;

        private Building building;

        private void Awake()
        {
            building = GetComponent<Building>();
            if (building == null)
            {
                Debug.LogError("[ProductionQueueTester] Building component not found!");
                enabled = false;
            }
        }

        private void Start()
        {
            Debug.Log("[ProductionQueueTester] ⌨️ Test Controls:");
            Debug.Log("  Press '1' = Queue Buggy (8s)");
            Debug.Log("  Press '2' = Queue Artillery (15s)");
            Debug.Log("  Press 'C' = Cancel current production");
            Debug.Log("  Press 'Q' = Clear queue");
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null) return;

            // Press '1' to queue Buggy
            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                QueueBuggy();
            }

            // Press '2' to queue Artillery
            if (keyboard.digit2Key.wasPressedThisFrame)
            {
                QueueArtillery();
            }

            // Press 'C' to cancel current production
            if (keyboard.cKey.wasPressedThisFrame)
            {
                CancelProduction();
            }

            // Press 'Q' to clear queue
            if (keyboard.qKey.wasPressedThisFrame)
            {
                ClearQueue();
            }
        }

        private void QueueBuggy()
        {
            if (buggyItem == null)
            {
                Debug.LogError("[ProductionQueueTester] Buggy ProductionItem not assigned!");
                return;
            }

            building.AddToProductionQueue(buggyItem);
            Debug.Log($"[ProductionQueueTester] ✅ Queued Buggy ({buggyItem.productionTime}s)");
        }

        private void QueueArtillery()
        {
            if (artilleryItem == null)
            {
                Debug.LogError("[ProductionQueueTester] Artillery ProductionItem not assigned!");
                return;
            }

            building.AddToProductionQueue(artilleryItem);
            Debug.Log($"[ProductionQueueTester] ✅ Queued Artillery ({artilleryItem.productionTime}s)");
        }

        private void CancelProduction()
        {
            if (building.ProductionQueue == null)
            {
                Debug.LogWarning("[ProductionQueueTester] No ProductionQueue on this building!");
                return;
            }

            building.ProductionQueue.CancelCurrent();
            Debug.Log("[ProductionQueueTester] ❌ Cancelled current production");
        }

        private void ClearQueue()
        {
            if (building.ProductionQueue == null)
            {
                Debug.LogWarning("[ProductionQueueTester] No ProductionQueue on this building!");
                return;
            }

            building.ProductionQueue.ClearQueue();
            Debug.Log("[ProductionQueueTester] 🗑️ Cleared production queue");
        }
    }
}
