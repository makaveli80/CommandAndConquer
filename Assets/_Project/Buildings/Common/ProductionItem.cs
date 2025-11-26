using UnityEngine;

namespace CommandAndConquer.Buildings
{
    /// <summary>
    /// Defines an item that can be produced by a building (unit or building).
    /// Phase 2: Production System
    /// </summary>
    [CreateAssetMenu(fileName = "NewProductionItem", menuName = "Command & Conquer/Production Item")]
    public class ProductionItem : ScriptableObject
    {
        [Header("Information")]
        [Tooltip("Display name of the item")]
        public string itemName;

        [Tooltip("Icon displayed in the production UI")]
        public Sprite icon;

        [Tooltip("Description shown in tooltips")]
        [TextArea(2, 4)]
        public string description;

        [Header("Production")]
        [Tooltip("Time required to produce this item (in seconds)")]
        [Min(0.1f)]
        public float productionTime = 10f;

        [Tooltip("Prefab to instantiate when production completes")]
        public GameObject prefab;

        [Tooltip("Is this a building? (true) or a unit? (false)")]
        public bool isBuilding = false;

        // Validation
        private void OnValidate()
        {
            if (productionTime < 0.1f)
            {
                Debug.LogWarning($"[ProductionItem] '{itemName}' has very low production time ({productionTime}s). Minimum recommended: 0.1s");
            }

            if (prefab == null)
            {
                Debug.LogWarning($"[ProductionItem] '{itemName}' has no prefab assigned!");
            }
        }
    }
}
