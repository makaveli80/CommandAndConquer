using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Artillery
{
    /// <summary>
    /// ScriptableObject contenant les données de l'Artillery.
    /// L'Artillery est une unité lourde, lente, avec une longue portée de tir.
    /// </summary>
    [CreateAssetMenu(fileName = "ArtilleryData", menuName = "Command & Conquer/Units/Artillery Data")]
    public class ArtilleryData : UnitData
    {
        // Les propriétés de base sont héritées de UnitData :
        // - unitName : Nom de l'unité
        // - description : Description
        // - moveSpeed : Vitesse de déplacement (très lente pour Artillery)
        // - canMove : Peut se déplacer
        // - prefab : Référence au prefab

        // Stats spécifiques à l'Artillery peuvent être ajoutées ici si besoin
        // Par exemple : fireRange, fireRate, attackDamage, armor, etc.
    }
}