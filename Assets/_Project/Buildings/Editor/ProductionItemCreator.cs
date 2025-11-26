using UnityEngine;
using UnityEditor;
using CommandAndConquer.Buildings;

namespace CommandAndConquer.Editor
{
    /// <summary>
    /// Editor utility to quickly create ProductionItem assets.
    /// Menu: Assets > Create > Command & Conquer > Production Item (Quick)
    /// </summary>
    public static class ProductionItemCreator
    {
        [MenuItem("Assets/Create/Command & Conquer/Production Item (Quick)", priority = 1)]
        public static void CreateProductionItem()
        {
            // Create asset
            ProductionItem item = ScriptableObject.CreateInstance<ProductionItem>();
            item.itemName = "New Item";
            item.productionTime = 10f;
            item.isBuilding = false;

            // Save asset in selected folder
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (System.IO.Path.GetExtension(path) != "")
            {
                path = path.Replace(System.IO.Path.GetFileName(path), "");
            }

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/NewProductionItem.asset");
            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();

            // Select and focus
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = item;
            EditorGUIUtility.PingObject(item);

            Debug.Log($"[ProductionItemCreator] Created ProductionItem at: {assetPath}");
        }
    }
}
