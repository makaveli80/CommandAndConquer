using System;
using System.Collections.Generic;
using UnityEngine;

namespace CommandAndConquer.Buildings
{
    /// <summary>
    /// Manages production queue with timer for buildings.
    /// Processes items in FIFO order with progress tracking.
    /// Phase 2: Production System
    /// </summary>
    public class ProductionQueue : MonoBehaviour
    {
        // State
        private Queue<ProductionItem> queue = new Queue<ProductionItem>();
        private ProductionItem currentItem;
        private float currentProgress; // 0.0 to 1.0

        // Events
        public event Action<ProductionItem> OnItemCompleted;
        public event Action<ProductionItem, float> OnProgressUpdated;
        public event Action<ProductionItem> OnItemStarted;

        // Properties
        public int QueueCount => queue.Count;
        public float CurrentProgress => currentProgress;
        public ProductionItem CurrentItem => currentItem;
        public bool IsProducing => currentItem != null;

        /// <summary>
        /// Adds an item to the production queue.
        /// If nothing is currently being produced, starts immediately.
        /// </summary>
        public void AddToQueue(ProductionItem item)
        {
            if (item == null)
            {
                Debug.LogError("[ProductionQueue] Cannot add null item to queue!");
                return;
            }

            // If idle, start production immediately
            if (currentItem == null)
            {
                StartProduction(item);
            }
            else
            {
                // Otherwise, add to queue
                queue.Enqueue(item);
                Debug.Log($"[ProductionQueue] Added '{item.itemName}' to queue. Queue size: {queue.Count + 1}");
            }
        }

        /// <summary>
        /// Cancels the current production and starts the next item in queue.
        /// </summary>
        public void CancelCurrent()
        {
            if (currentItem == null)
            {
                Debug.LogWarning("[ProductionQueue] No item currently in production to cancel.");
                return;
            }

            Debug.Log($"[ProductionQueue] Cancelled production of '{currentItem.itemName}'");
            currentItem = null;
            currentProgress = 0f;

            // Start next item if available
            if (queue.Count > 0)
            {
                ProductionItem nextItem = queue.Dequeue();
                StartProduction(nextItem);
            }
        }

        /// <summary>
        /// Clears the entire queue (but keeps current production).
        /// </summary>
        public void ClearQueue()
        {
            int count = queue.Count;
            queue.Clear();
            Debug.Log($"[ProductionQueue] Cleared {count} items from queue");
        }

        private void StartProduction(ProductionItem item)
        {
            currentItem = item;
            currentProgress = 0f;
            Debug.Log($"[ProductionQueue] Started production of '{item.itemName}' ({item.productionTime}s)");
            OnItemStarted?.Invoke(item);
        }

        private void Update()
        {
            if (currentItem == null) return;

            // Advance progress
            float deltaProgress = Time.deltaTime / currentItem.productionTime;
            currentProgress += deltaProgress;

            // Notify progress update
            OnProgressUpdated?.Invoke(currentItem, currentProgress);

            // Check if completed
            if (currentProgress >= 1.0f)
            {
                CompleteProduction();
            }
        }

        private void CompleteProduction()
        {
            Debug.Log($"[ProductionQueue] Completed production of '{currentItem.itemName}'");

            // Notify completion
            ProductionItem completedItem = currentItem;
            OnItemCompleted?.Invoke(completedItem);

            // Reset current item
            currentItem = null;
            currentProgress = 0f;

            // Start next item if available
            if (queue.Count > 0)
            {
                ProductionItem nextItem = queue.Dequeue();
                StartProduction(nextItem);
            }
        }

        // Debug visualization
        private void OnGUI()
        {
            if (!Application.isPlaying) return;
            if (currentItem == null) return;

            // Display production progress in top-left corner
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.white;

            string statusText = $"Producing: {currentItem.itemName} ({currentProgress * 100:F0}%)";
            if (queue.Count > 0)
            {
                statusText += $" | Queue: {queue.Count}";
            }

            GUI.Label(new Rect(10, 10, 400, 30), statusText, style);
        }
    }
}
