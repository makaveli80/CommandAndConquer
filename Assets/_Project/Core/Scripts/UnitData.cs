using UnityEngine;

namespace CommandAndConquer.Core
{
    /// <summary>
    /// ScriptableObject de base pour stocker les données des unités.
    /// </summary>
    public abstract class UnitData : ScriptableObject
    {
        [Header("Informations de base")]
        [Tooltip("Nom de l'unité")]
        public string unitName;

        [Tooltip("Description de l'unité")]
        [TextArea(2, 4)]
        public string description;

        [Header("Déplacement")]
        [Tooltip("Vitesse de déplacement de l'unité")]
        public float moveSpeed = 5f;

        [Tooltip("L'unité peut-elle se déplacer?")]
        public bool canMove = true;

        [Header("Visuel")]
        [Tooltip("Préfab de l'unité")]
        public GameObject prefab;
    }
}
