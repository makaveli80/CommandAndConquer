using UnityEngine;

namespace CommandAndConquer.Buildings
{
    /// <summary>
    /// ScriptableObject pour stocker les données des bâtiments.
    /// Définit la configuration d'un type de bâtiment (taille, production, spawn point, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "NewBuildingData", menuName = "Command & Conquer/Building Data", order = 10)]
    public class BuildingData : ScriptableObject
    {
        [Header("Informations de base")]
        [Tooltip("Nom du bâtiment")]
        public string buildingName;

        [Tooltip("Description du bâtiment")]
        [TextArea(2, 4)]
        public string description;

        [Tooltip("Sprite du bâtiment")]
        public Sprite sprite;

        [Header("Grille")]
        [Tooltip("Largeur en cellules")]
        public int width = 1;

        [Tooltip("Hauteur en cellules")]
        public int height = 1;

        [Header("Spawn Point")]
        [Tooltip("Offset du point de sortie des unités (relatif à l'origine du bâtiment)")]
        public Vector2Int spawnOffset;

        // NOTE: Le champ "canProduce" (ProductionItem[]) sera ajouté en Phase 2
    }
}
