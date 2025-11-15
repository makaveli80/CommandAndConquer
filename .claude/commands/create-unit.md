---
description: Crée une nouvelle unité avec le template de base
tags: [project]
---

Crée une nouvelle unité pour le jeu Command and Conquer.

Demande d'abord à l'utilisateur:
- Le nom de l'unité (format: PascalCase, ex: "TankLight", "Engineer")
- Le type d'unité (véhicule, infanterie, bâtiment)

Ensuite, génère la structure suivante dans `Assets/_Project/Units/<NomUnité>/`:

1. **Créer les dossiers**:
   - `Scripts/`
   - `Prefabs/`
   - `Sprites/`
   - `Data/`

2. **Créer les scripts de base**:
   - `Scripts/<NomUnité>Controller.cs` - Contrôleur principal avec namespace `CommandAndConquer.Units.<NomUnité>`
   - `Scripts/<NomUnité>Movement.cs` - Gestion du déplacement (si applicable)

3. **Créer le ScriptableObject**:
   - `Data/<NomUnité>Data.cs` - Script du ScriptableObject pour les stats

Structure du contrôleur:
```csharp
using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.<NomUnité>
{
    public class <NomUnité>Controller : UnitBase
    {
        [SerializeField] private <NomUnité>Data unitData;

        private void Awake()
        {
            // Initialisation
        }

        private void Start()
        {
            // Démarrage
        }

        public override void OnSelected()
        {
            // Comportement quand l'unité est sélectionnée
        }

        public override void OnDeselected()
        {
            // Comportement quand l'unité est désélectionnée
        }
    }
}
```

Structure du ScriptableObject:
```csharp
using UnityEngine;

namespace CommandAndConquer.Units.<NomUnité>
{
    [CreateAssetMenu(fileName = "<NomUnité>Data", menuName = "Units/<NomUnité>")]
    public class <NomUnité>Data : ScriptableObject
    {
        [Header("Stats de base")]
        public string unitName = "<Nom Unité>";
        public int maxHealth = 100;
        public float moveSpeed = 5f;

        [Header("Combat")]
        public int damage = 10;
        public float attackRange = 5f;
        public float attackCooldown = 1f;
    }
}
```

Après création, ajouter l'unité dans `UNITS.md` avec:
- Chemin
- Description
- Namespace
- Liste des fichiers

Respecter les conventions du fichier `CONVENTIONS.md`.