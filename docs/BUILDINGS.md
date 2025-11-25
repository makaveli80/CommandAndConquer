# BUILDINGS.md

Documentation du système de bâtiments et de production

## Vue d'ensemble

Système de bâtiments avec production d'unités, file d'attente, et placement interactif.

**Bâtiment initial** : Construction Yard (2×2)
**Productions** : Buggy, Artillery
**Système de ressources** : Aucun (production gratuite basée sur le temps)
**Placement** : Aperçu fantôme avec validation visuelle (vert/rouge)

---

## Architecture

### Structure des Modules

```
Assets/_Project/
├── Buildings/              # NOUVEAU module
│   ├── Common/
│   │   ├── Building.cs                    # Composant générique (comme Unit.cs)
│   │   ├── BuildingData.cs               # ScriptableObject config
│   │   ├── ProductionQueue.cs            # File d'attente avec timer
│   │   └── SpawnPoint.cs                 # Point de sortie des unités
│   ├── ConstructionYard/
│   │   ├── Data/                         # ScriptableObjects
│   │   ├── Prefabs/                      # Prefab Construction Yard
│   │   └── Sprites/                      # Sprites du bâtiment
│   └── Scripts/
│       └── BuildingPlacementSystem.cs    # Système de placement fantôme
│
├── UI/
│   ├── Scripts/
│   │   ├── ProductionPanel.cs            # Panneau de production (droite)
│   │   ├── ProductionButton.cs           # Bouton pour chaque unité/bâtiment
│   │   └── ProductionQueueUI.cs          # Affichage de la file d'attente
│   ├── Prefabs/
│   │   └── ProductionPanelCanvas.prefab
│   └── Sprites/                          # Tileset UI
│
└── Grid/
    └── Scripts/
        └── GridManager.cs                # [EXTENSION] Support multi-cellule
```

### Dependency Graph

```
Core → Buildings, UI
Grid → Buildings (extensions multi-cellule)
Buildings → Core, Grid
UI → Buildings
```

---

## Composants Principaux

### 1. BuildingData (ScriptableObject)

Configuration d'un type de bâtiment.

```csharp
[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Command & Conquer/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("Informations de base")]
    public string buildingName;
    public string description;
    public Sprite sprite;

    [Header("Grille")]
    public int width = 1;           // Largeur en cellules
    public int height = 1;          // Hauteur en cellules

    [Header("Production")]
    public ProductionItem[] canProduce;  // Ce que ce bâtiment peut produire

    [Header("Spawn Point")]
    public Vector2Int spawnOffset;  // Offset du point de sortie (relatif à l'origine)
}
```

### 2. Building (Component)

Composant générique pour tous les bâtiments (pattern similaire à Unit.cs).

```csharp
public class Building : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;

    private GridPosition originPosition;      // Cellule d'origine (bas-gauche)
    private List<GridPosition> occupiedCells; // Toutes les cellules occupées
    private GridManager gridManager;

    // Composants
    private ProductionQueue productionQueue;
    private SpawnPoint spawnPoint;

    // Events
    public event Action<ProductionItem> OnProductionCompleted;

    // Lifecycle
    void Awake() { /* Auto-découverte composants */ }
    void Start() { /* Enregistrement sur la grille */ }
    void OnDestroy() { /* Libération des cellules */ }

    // API publique
    public void AddToProductionQueue(ProductionItem item) { }
    public GridPosition[] OccupiedCells { get; }
    public BuildingData Data { get; }
}
```

### 3. ProductionItem (ScriptableObject)

Définit un objet produisible (unité ou bâtiment).

```csharp
[CreateAssetMenu(fileName = "NewProductionItem", menuName = "Command & Conquer/Production Item")]
public class ProductionItem : ScriptableObject
{
    [Header("Informations")]
    public string itemName;
    public Sprite icon;              // Icône pour l'UI
    public string description;

    [Header("Production")]
    public float productionTime;     // Temps en secondes
    public GameObject prefab;        // Prefab à spawner
    public bool isBuilding;          // true = bâtiment, false = unité
}
```

### 4. ProductionQueue (Component)

Gère la file d'attente de production avec timer.

```csharp
public class ProductionQueue : MonoBehaviour
{
    private Queue<ProductionItem> queue = new Queue<ProductionItem>();
    private ProductionItem currentItem;
    private float currentProgress;   // 0.0 à 1.0

    // Events
    public event Action<ProductionItem> OnItemCompleted;
    public event Action<ProductionItem, float> OnProgressUpdated;

    // API
    public void AddToQueue(ProductionItem item) { }
    public void CancelCurrent() { }
    public int QueueCount { get; }
    public float CurrentProgress { get; }
    public ProductionItem CurrentItem { get; }

    void Update()
    {
        if (currentItem == null) return;

        currentProgress += Time.deltaTime / currentItem.productionTime;
        OnProgressUpdated?.Invoke(currentItem, currentProgress);

        if (currentProgress >= 1.0f)
        {
            OnItemCompleted?.Invoke(currentItem);
            currentItem = null;
            currentProgress = 0f;

            if (queue.Count > 0)
                currentItem = queue.Dequeue();
        }
    }
}
```

### 5. SpawnPoint (Component)

Point de sortie pour faire apparaître les unités produites.
**Note** : Associé à un bâtiment existant (pas créé séparément).

```csharp
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Vector2Int gridOffset; // Offset depuis l'origine du bâtiment

    private GridManager gridManager;
    private Building parentBuilding;

    public void SpawnUnit(GameObject unitPrefab)
    {
        GridPosition spawnPos = CalculateSpawnPosition();

        // Vérifier si la cellule est libre
        if (!gridManager.IsFree(spawnPos))
        {
            Debug.LogWarning($"[SpawnPoint] Cannot spawn unit, cell {spawnPos} is occupied!");
            return;
        }

        // Instantier l'unité
        Vector3 worldPos = gridManager.GetWorldPosition(spawnPos);
        GameObject unitObj = Instantiate(unitPrefab, worldPos, Quaternion.identity);

        Debug.Log($"[SpawnPoint] Spawned unit at {spawnPos}");
    }

    private GridPosition CalculateSpawnPosition()
    {
        GridPosition buildingOrigin = parentBuilding.OriginPosition;
        return new GridPosition(
            buildingOrigin.x + gridOffset.x,
            buildingOrigin.y + gridOffset.y
        );
    }
}
```

### 6. BuildingPlacementSystem (MonoBehaviour)

Gère le placement interactif des bâtiments avec aperçu fantôme.

```csharp
public class BuildingPlacementSystem : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Color validColor = new Color(0, 1, 0, 0.5f);   // Vert transparent
    [SerializeField] private Color invalidColor = new Color(1, 0, 0, 0.5f); // Rouge transparent

    private BuildingData buildingToPlace;
    private GameObject ghostObject;
    private SpriteRenderer ghostRenderer;
    private GridManager gridManager;
    private bool isPlacing = false;

    public event Action<BuildingData, GridPosition> OnBuildingPlaced;

    public void StartPlacement(BuildingData buildingData)
    {
        buildingToPlace = buildingData;
        isPlacing = true;

        // Créer l'aperçu fantôme
        ghostObject = new GameObject("BuildingGhost");
        ghostRenderer = ghostObject.AddComponent<SpriteRenderer>();
        ghostRenderer.sprite = buildingData.sprite;
        ghostRenderer.sortingOrder = 100;
    }

    void Update()
    {
        if (!isPlacing) return;

        // Suivre la souris
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridPosition gridPos = gridManager.GetGridPosition(mouseWorld);

        // Positionner le fantôme
        Vector3 worldPos = gridManager.GetWorldPosition(gridPos);
        ghostObject.transform.position = worldPos;

        // Vérifier validité
        bool valid = gridManager.CanPlaceBuilding(gridPos, buildingToPlace.width, buildingToPlace.height);
        ghostRenderer.color = valid ? validColor : invalidColor;

        // Confirmer placement
        if (Input.GetMouseButtonDown(0) && valid)
        {
            ConfirmPlacement(gridPos);
        }

        // Annuler
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private void ConfirmPlacement(GridPosition position)
    {
        OnBuildingPlaced?.Invoke(buildingToPlace, position);
        CleanupGhost();
    }
}
```

---

## Extensions GridManager

### Multi-cell Support

Le GridManager doit être étendu pour supporter les bâtiments multi-cellules.

```csharp
// Nouvelles méthodes à ajouter dans GridManager.cs

/// <summary>
/// Vérifie si un bâtiment peut être placé à une position donnée.
/// </summary>
public bool CanPlaceBuilding(GridPosition origin, int width, int height)
{
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            GridPosition pos = new GridPosition(origin.x + x, origin.y + y);

            if (!IsValidGridPosition(pos) || !IsFree(pos))
                return false;
        }
    }
    return true;
}

/// <summary>
/// Occupe toutes les cellules d'un bâtiment.
/// </summary>
public bool TryOccupyBuildingCells(MonoBehaviour building, GridPosition origin, int width, int height)
{
    List<GridPosition> cells = new List<GridPosition>();

    // Collecter toutes les cellules
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            cells.Add(new GridPosition(origin.x + x, origin.y + y));
        }
    }

    // Tenter d'occuper toutes les cellules
    foreach (GridPosition pos in cells)
    {
        GridCell cell = GetCell(pos);
        if (!cell.TryOccupy(building))
        {
            // Échec - libérer les cellules déjà occupées
            foreach (GridPosition occupiedPos in cells)
            {
                if (occupiedPos == pos) break;
                GetCell(occupiedPos)?.Release();
            }
            return false;
        }
    }

    // Succès
    buildingCells[building] = cells;
    return true;
}

/// <summary>
/// Libère toutes les cellules d'un bâtiment.
/// </summary>
public void ReleaseBuildingCells(MonoBehaviour building)
{
    if (buildingCells.TryGetValue(building, out List<GridPosition> cells))
    {
        foreach (GridPosition pos in cells)
        {
            GetCell(pos)?.Release();
        }
        buildingCells.Remove(building);
    }
}

// Nouveau dictionnaire à ajouter aux champs
private Dictionary<MonoBehaviour, List<GridPosition>> buildingCells = new Dictionary<MonoBehaviour, List<GridPosition>>();
```

---

## Flow de Production

### Cycle Complet

```
1. User clique bouton "Buggy" dans UI
      ↓
2. ProductionButton.OnClick()
      ↓
3. Building.AddToProductionQueue(buggyItem)
      ↓
4. ProductionQueue.Update() [chaque frame]
   - currentProgress += Time.deltaTime / item.productionTime
   - OnProgressUpdated(item, progress) → UI se met à jour
      ↓
5. currentProgress >= 1.0 → OnItemCompleted(item)
      ↓
6. Building reçoit OnItemCompleted
      ↓
7. if (item.isBuilding)
       → BuildingPlacementSystem.StartPlacement(item)
       → User place le bâtiment (fantôme vert/rouge)
   else
       → SpawnPoint.SpawnUnit(item.prefab)
       → Unité apparaît au point de sortie
```

---

## Plan d'Implémentation (5 Phases)

### ✅ Phase 0 : Planification
- [x] Définir l'architecture
- [x] Créer le plan d'implémentation
- [x] Documenter dans BUILDINGS.md

### ✅ Phase 1 : Core Building System (COMPLÈTE)
**Objectif** : Bâtiments fonctionnels sur la grille

**Architecture & Code** :
- [x] Créer `BuildingData.cs` ScriptableObject
- [x] Créer `Building.cs` composant générique (ultra-simplifié avec Pivot Bottom Left)
- [x] Étendre `GridManager.cs` pour support multi-cellule
   - [x] `CanPlaceBuilding(GridPosition origin, int width, int height)`
   - [x] `TryOccupyBuildingCells(MonoBehaviour, GridPosition, int, int)` avec rollback atomique
   - [x] `ReleaseBuildingCells(MonoBehaviour)`
- [x] Corriger `VerifyGridCoherence()` pour supporter les bâtiments
- [x] Corriger `CleanupDestroyedUnits()` pour nettoyer aussi les bâtiments

**Bâtiment Airstrip (4×2)** :
- [x] Créer `BuildingDataCreator.cs` (Menu Editor)
- [x] Créer asset `AirstripData` (4×2)
- [x] Créer prefab `Airstrip` avec composants génériques
- [x] Implémenter Gizmos de debug (cellules bleus, centre jaune, origine verte)

**⚠️ Convention Pivot Bottom Left** :
- [x] Refactoriser pour utiliser **Pivot Bottom Left (0,0)** sur tous les sprites de bâtiments
- [x] Simplifier `Building.Initialize()` : `originPosition = GetGridPosition(position)` directement
- [x] Créer `BuildingSpriteImporter.cs` pour configuration automatique à l'import
- [x] Ajouter menus Tools pour reconfigurer sprites existants

**Documentation & Tooling** :
- [x] Documenter convention Pivot Bottom Left dans README.md et CLAUDE.md
- [x] Créer `Buildings/Airstrip/README.md` avec guide complet
- [x] Implémenter BuildingSpriteImporter avec AssetPostprocessor

**Tests** :
- [x] Airstrip occupe correctement 8 cellules (4×2)
- [x] Sprites alignés parfaitement avec les cellules
- [x] Position éditeur = position jeu (WYSIWYG)
- [x] GridManager détecte collisions multi-cellules
- [x] Vérification de cohérence fonctionne pour bâtiments
- [x] Cleanup automatique des bâtiments détruits

### 🔨 Phase 2 : Production System
**Objectif** : File d'attente avec timer fonctionnelle

7. Créer `ProductionItem.cs` ScriptableObject
8. Créer `ProductionQueue.cs` avec timer et events
9. Créer assets ProductionItem pour Buggy et Artillery
10. Intégrer `ProductionQueue` dans `Building.cs`
11. **Test** : Production via code (`Debug.Log` + timer)

### 🔨 Phase 3 : Spawn System
**Objectif** : Unités apparaissent au point de sortie

12. Créer `SpawnPoint.cs` composant
13. Ajouter `SpawnPoint` au prefab ConstructionYard
14. Connecter ProductionQueue → SpawnPoint dans Building.cs
15. Implémenter apparition d'unité avec vérification cellule libre
16. **Test** : Production complète Buggy/Artillery → spawn

### 🔨 Phase 4 : Building Placement
**Objectif** : Placement interactif avec feedback visuel

17. Créer `BuildingPlacementSystem.cs`
18. Implémenter aperçu fantôme avec suivi souris
19. Implémenter feedback couleur (vert=valide, rouge=invalide)
20. Implémenter validation placement (clic gauche) et annulation (clic droit)
21. Intégrer avec ProductionQueue (bâtiments produits → mode placement)
22. **Test** : Produire un bâtiment → placer avec fantôme

### 🔨 Phase 5 : UI Production Panel
**Objectif** : Interface graphique complète

23. Créer Canvas UI avec panneau à droite
24. Importer sprites du tileset UI
25. Créer `ProductionPanel.cs` et `ProductionButton.cs`
26. Créer `ProductionQueueUI.cs` avec barre de progression
27. Connecter UI ↔ Building sélectionné
28. Polish visual et animations UI
29. **Test** : Workflow complet depuis l'UI

---

## Tests & Validation

### Tests Phase 1
- [ ] Construction Yard occupe bien 4 cellules (2×2)
- [ ] GridManager détecte correctement les collisions multi-cellules
- [ ] Bâtiment se place au bon endroit (coordonnées world correctes)

### Tests Phase 2
- [ ] File d'attente fonctionne (FIFO)
- [ ] Timer progresse correctement
- [ ] Events OnItemCompleted déclenchés
- [ ] Multiple items en queue

### Tests Phase 3
- [ ] Unité spawn à la bonne position
- [ ] SpawnPoint vérifie cellule libre
- [ ] Unité ne spawn pas si cellule occupée

### Tests Phase 4
- [ ] Fantôme suit la souris
- [ ] Couleur change selon validité
- [ ] Placement confirme et instancie bâtiment
- [ ] Annulation fonctionne (clic droit)

### Tests Phase 5
- [ ] UI affiche les bonnes icônes
- [ ] Barre de progression se met à jour
- [ ] File d'attente s'affiche correctement
- [ ] Boutons disabled si production impossible

---

## Notes Techniques

### ⚠️ CRITICAL - Sprite Pivot Convention

**Tous les sprites de bâtiments DOIVENT avoir Pivot = Bottom Left (0, 0)**

**Pourquoi Bottom Left ?** :
- `transform.position` = coin bas-gauche = origine du bâtiment directement
- Ultra-simple : Position (5,9) → occupe cellules (5,9) à (width-1, height-1)
- WYSIWYG parfait : Position éditeur = position jeu
- Code minimal : `originPosition = gridManager.GetGridPosition(transform.position)`

**Configuration automatique** :
- Les nouveaux sprites dans `Buildings/*/Sprites/` sont auto-configurés par `BuildingSpriteImporter.cs`
- Menu manuel : `Tools > Command & Conquer > Reconfigure All Building Sprites`

**Comparaison Pivot Center vs Bottom Left** :

| Pivot Center (0.5, 0.5) | Pivot Bottom Left (0, 0) ✅ |
|-------------------------|---------------------------|
| Position = centre | Position = origine |
| Calcul complexe requis | Position directe |
| Bon pour rotation | Bon pour grille RTS |
| Code : 6 lignes | Code : 1 ligne |

### Coordinate System

**Convention Grid → World avec Pivot Bottom Left** :
```csharp
// Pour un Airstrip 4×2 placé à position (5, 9) :

// Position GameObject = origine directement (grâce au Pivot Bottom Left)
transform.position = new Vector3(5f, 9f, 0);

// Cellules occupées calculées depuis l'origine :
// (5,9), (6,9), (7,9), (8,9), (5,10), (6,10), (7,10), (8,10)

// Origine sur la grille :
GridPosition origin = gridManager.GetGridPosition(transform.position); // (5, 9)

// Centre visuel du bâtiment (pour Gizmos) :
Vector3 center = new Vector3(origin.x + width/2f, origin.y + height/2f, 0); // (7, 10)
```

### Spawn Point Offset

Le spawn point utilise un offset **relatif** à l'origine du bâtiment :

```csharp
// Construction Yard 2×2 à (5,5)
// Spawn offset = (2, 0) → en bas à droite
// Position spawn = (5+2, 5+0) = (7, 5)
```

### UI Update Pattern

```csharp
// ProductionQueue met à jour l'UI via events
productionQueue.OnProgressUpdated += (item, progress) => {
    productionQueueUI.UpdateProgressBar(progress);
};

productionQueue.OnItemCompleted += (item) => {
    productionQueueUI.RemoveFromDisplay();
};
```

---

## Prochaines Extensions

Après Phase 5, extensions possibles :

- **Ressources** : Système de crédits et coûts de production
- **Énergie** : Power Plants et consommation électrique
- **Multiple Buildings** : Plusieurs bâtiments de production
- **Rally Points** : Définir où les unités vont après spawn
- **Construction Animation** : Animation progressive de construction
- **Building Destruction** : Détruire des bâtiments
- **Production Cancellation** : Annuler items dans la queue avec remboursement

---

**Dernière mise à jour** : 2025-11-25
**Phase actuelle** : Phase 1 (Core Building System) ✅ **COMPLÈTE**
**Prochaine étape** : Phase 2 - Production System

**Changelog Phase 1** :
- ✅ Architecture complète avec support multi-cellule
- ✅ Convention Pivot Bottom Left implémentée
- ✅ BuildingSpriteImporter pour automation
- ✅ Airstrip 4×2 fonctionnel avec Gizmos de debug
- ✅ Documentation complète (BUILDINGS.md, README.md, CLAUDE.md)
- ✅ Tous les bugs corrigés (cohérence, cleanup, alignement)
