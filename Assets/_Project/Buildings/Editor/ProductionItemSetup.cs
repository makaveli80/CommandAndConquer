using UnityEngine;
using UnityEditor;
using CommandAndConquer.Buildings;

namespace CommandAndConquer.Editor
{
    /// <summary>
    /// One-time setup utility to create ProductionItem assets for Buggy and Artillery.
    /// Menu: Tools > Command & Conquer > Setup Production Items
    /// </summary>
    public static class ProductionItemSetup
    {
        private const string DATA_PATH = "Assets/_Project/Buildings/Common/Data";
        private const string BUGGY_PREFAB = "Assets/_Project/Units/Buggy/Prefabs/Buggy.prefab";
        private const string ARTILLERY_PREFAB = "Assets/_Project/Units/Artillery/Prefabs/Artillery.prefab";

        [MenuItem("Tools/Command & Conquer/Setup Production Items")]
        public static void SetupProductionItems()
        {
            // Ensure Data folder exists
            if (!AssetDatabase.IsValidFolder(DATA_PATH))
            {
                string parentFolder = "Assets/_Project/Buildings/Common";
                AssetDatabase.CreateFolder(parentFolder, "Data");
                Debug.Log($"[ProductionItemSetup] Created folder: {DATA_PATH}");
            }

            // Create Buggy ProductionItem
            CreateBuggyProductionItem();

            // Create Artillery ProductionItem
            CreateArtilleryProductionItem();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[ProductionItemSetup] ✅ Production items created successfully!");
        }

        private static void CreateBuggyProductionItem()
        {
            string path = $"{DATA_PATH}/BuggyProductionItem.asset";

            // Check if already exists
            ProductionItem existing = AssetDatabase.LoadAssetAtPath<ProductionItem>(path);
            if (existing != null)
            {
                Debug.Log($"[ProductionItemSetup] Buggy production item already exists at: {path}");
                return;
            }

            // Create new asset
            ProductionItem item = ScriptableObject.CreateInstance<ProductionItem>();
            item.itemName = "Buggy";
            item.description = "Fast reconnaissance vehicle with light armor.";
            item.productionTime = 8f; // 8 seconds
            item.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BUGGY_PREFAB);
            item.isBuilding = false;

            AssetDatabase.CreateAsset(item, path);
            Debug.Log($"[ProductionItemSetup] Created Buggy production item at: {path}");
        }

        private static void CreateArtilleryProductionItem()
        {
            string path = $"{DATA_PATH}/ArtilleryProductionItem.asset";

            // Check if already exists
            ProductionItem existing = AssetDatabase.LoadAssetAtPath<ProductionItem>(path);
            if (existing != null)
            {
                Debug.Log($"[ProductionItemSetup] Artillery production item already exists at: {path}");
                return;
            }

            // Create new asset
            ProductionItem item = ScriptableObject.CreateInstance<ProductionItem>();
            item.itemName = "Artillery";
            item.description = "Long-range artillery unit with heavy firepower.";
            item.productionTime = 15f; // 15 seconds
            item.prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ARTILLERY_PREFAB);
            item.isBuilding = false;

            AssetDatabase.CreateAsset(item, path);
            Debug.Log($"[ProductionItemSetup] Created Artillery production item at: {path}");
        }
    }
}
