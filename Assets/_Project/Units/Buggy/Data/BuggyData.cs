using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// ScriptableObject contenant les données du Buggy.
    /// Le Buggy est un véhicule léger et rapide de reconnaissance.
    /// </summary>
    [CreateAssetMenu(fileName = "BuggyData", menuName = "Command & Conquer/Units/Buggy Data")]
    public class BuggyData : UnitData
    {
        // Les propriétés de base sont héritées de UnitData :
        // - unitName : Nom de l'unité
        // - description : Description
        // - moveSpeed : Vitesse de déplacement
        // - canMove : Peut se déplacer
        // - prefab : Référence au prefab

        // Stats spécifiques au Buggy peuvent être ajoutées ici si besoin
        // Par exemple : fireRate, attackRange, armor, etc.
    }
}
