# Implémentation de l'unité Buggy

Documentation de l'implémentation du Buggy - Première unité jouable du projet Command & Conquer.

**Date de création :** 2025-01-17
**Statut :** Étapes 1-7 complétées (validation terminée) ✅
**Prochaine étape :** Étape 8 (animations) OU Commit 9 (système de sélection)

---

## 📊 État actuel

### ✅ Étapes complétées (1-7)

#### **Étape 1 : BuggyData ScriptableObject** ✅
- **Fichier :** `Assets/_Project/Units/Buggy/Data/BuggyData.cs`
- **Asset :** `Assets/_Project/Units/Buggy/Data/BuggyData.asset`
- **Configuration :**
  - `unitName` : "Buggy"
  - `moveSpeed` : 4.0f (rapide - véhicule de reconnaissance)
  - `canMove` : true
- **Script Editor :** `CreateBuggyData.cs` pour création automatique

#### **Étape 2 : BuggyController** ✅
- **Fichier :** `Assets/_Project/Units/Buggy/Scripts/BuggyController.cs`
- **Responsabilités :**
  - Hérite de `UnitBase`
  - Implémente `IMovable` et `ISelectable`
  - Gère l'initialisation et coordination des composants
  - Occupe automatiquement la cellule de départ
  - Utilise `FindFirstObjectByType<GridManager>()` (nouvelle API Unity)
- **Référence :** `BuggyData.asset`

#### **Étape 3 : Prefab Buggy** ✅
- **Fichier :** `Assets/_Project/Units/Buggy/Prefabs/Buggy.prefab`
- **Composants :**
  - `SpriteRenderer` : sprite `buggy-0000`, Order in Layer = 10
  - `BuggyController` : référence à BuggyData
  - `BuggyMovement` : système de déplacement
  - `BoxCollider2D` : trigger 1x1 pour sélection future
- **Position par défaut :** (5.5, 5.5, 0) - centrée sur cellule (5,5)

#### **Étape 4 : BuggyMovement - Système de déplacement** ✅
- **Fichier :** `Assets/_Project/Units/Buggy/Scripts/BuggyMovement.cs`
- **Fonctionnalités :**

  **Pathfinding :**
  - Algorithme en ligne droite (8 directions)
  - Utilise `Math.Sign()` pour calculer deltaX/deltaY (-1, 0, +1)
  - Vérifie que chaque case du chemin est libre
  - Limite de sécurité : 1000 itérations max

  **Mouvement case par case :**
  - Libère cellule actuelle → Occupe cellule cible
  - Interpolation fluide avec `Vector3.MoveTowards()`
  - Snap précis au centre de la cellule (distance < 0.01)
  - Vitesse depuis `BuggyData.moveSpeed`

  **Système de destination en attente :**
  - Si changement de destination pendant mouvement :
    - Nouvelle destination mise en `pendingTargetPosition`
    - L'unité finit d'atteindre le centre de la case actuelle
    - Puis applique la nouvelle destination
  - Évite les cellules fantômes occupées
  - Mouvement fluide sans changement brusque

  **Méthodes principales :**
  ```csharp
  public void MoveTo(GridPosition target)
  private List<GridPosition> CalculatePath(start, end)
  private void MoveToNextCell()
  private void CancelCurrentMovement()
  ```

#### **Étape 5 : Tests déplacement** ✅
- Tests effectués avec BuggyTestMovement
- Déplacement case par case validé
- Occupation/libération des cellules validée
- Changement de destination fluide validé

#### **Étape 6 : Script de test clavier** ✅
- **Fichier :** `Assets/_Project/Units/Buggy/Scripts/BuggyTestMovement.cs`
- **Contrôles pavé numérique :**
  ```
  7  8  9     →  Haut-gauche    Haut         Haut-droite
  4  5  6     →  Gauche         Centre       Droite
  1  2  3     →  Bas-gauche     Bas          Bas-droite
  0           →  Retour position initiale (5,5)
  H           →  Afficher l'aide
  ```
- **Positions de test :**
  - Numpad 1: (0, 0) - Bas-gauche
  - Numpad 2: (9, 0) - Bas-centre
  - Numpad 3: (19, 0) - Bas-droite
  - Numpad 4: (0, 9) - Gauche-centre
  - Numpad 5: (9, 9) - Centre
  - Numpad 6: (19, 9) - Droite-centre
  - Numpad 7: (0, 19) - Haut-gauche
  - Numpad 8: (9, 19) - Haut-centre
  - Numpad 9: (19, 19) - Haut-droite
  - Numpad 0: (5, 5) - Home
- **Input System :** Utilise New Input System (`Unity.InputSystem`)

---

## 🔧 Configuration technique

### **Assembly Definitions**
- `CommandAndConquer.Units.asmdef` référence :
  - `CommandAndConquer.Core`
  - `CommandAndConquer.Grid`
  - `Unity.InputSystem`

### **Sprites**
- 16 sprites d'animation : `buggy-0000.png` à `buggy-0030.png` (numéros pairs)
- Configuration automatique via `UnitSpriteImporter.cs` :
  - PPU = 128
  - FilterMode = Point
  - Compression = Uncompressed
  - SpriteImportMode = Multiple

### **Grille**
- Taille : 20x20 cellules
- Taille cellule : 1.0 unité Unity
- Positions monde : centrées avec +0.5 (ex: cellule (5,5) → position (5.5, 5.5))
- Conversion : `FloorToInt()` pour monde → grille

---

## 🎮 Comment tester

### **Play mode rapide**
1. Ouvrir `Assets/_Project/Scenes/Game.unity`
2. Play ▶️
3. Appuyer sur les touches du pavé numérique (1-9)
4. Observer :
   - ✅ Buggy se déplace case par case
   - ✅ Cellules Gizmos : vert (libre) / rouge (occupée)
   - ✅ Changement de direction fluide
   - ✅ Messages debug dans Console

### **Tests à effectuer**
- **Test 1 :** Numpad 9 → Diagonal complet vers (19,19)
- **Test 2 :** Numpad 1 puis Numpad 9 rapidement → Changement direction fluide
- **Test 3 :** Spam plusieurs touches → Pas de cellules fantômes
- **Test 4 :** Observer les Gizmos → Une seule cellule rouge à la fois

---

#### **Étape 7 : Validation finale occupation/libération** ✅
**Objectif :** Tests approfondis du système de gestion des cellules

**Tests effectués (manuels) :**
1. **Plusieurs Buggies :**
   - ✅ 2-3 instances du prefab Buggy placées dans la scène
   - ✅ Aucune unité ne peut occuper la même cellule
   - ✅ Collisions de chemin gérées avec retry mechanism

2. **Scénarios edge case :**
   - ✅ Chemin bloqué par obstacle → Messages d'erreur corrects
   - ✅ Position invalide → Validation fonctionne
   - ✅ Cellule déjà occupée → Mouvement refusé ou retry activé

3. **Performance :**
   - ✅ Mouvement fluide avec plusieurs unités simultanées
   - ✅ Aucune cellule fantôme (pas de memory leak)
   - ✅ Système de coherence vérifie l'intégrité toutes les 60 frames

**Critères de validation :**
- ✅ Aucune cellule fantôme occupée
- ✅ Messages d'erreur clairs et pertinents
- ✅ Plusieurs unités peuvent coexister sans conflit
- ✅ Pas de crash ou comportement inattendu

**Résultat :** VALIDÉ - Le système de collision atomique fonctionne correctement

---

## ⏭️ Prochaines étapes

### **Étape 8 : Animations 8 directions (Optionnel)** 📝
**Objectif :** Ajouter les animations pour les 8 directions de mouvement

**À créer :**
1. **Animator Controller :**
   - `Assets/_Project/Units/Buggy/Animations/BuggyAnimator.controller`
   - Paramètre `Direction` (int 0-7)

2. **Animation Clips (8 directions) :**
   - North (0) : sprites buggy-0000, buggy-0002
   - NorthEast (1) : sprites buggy-0004, buggy-0006
   - East (2) : sprites buggy-0008, buggy-0010
   - SouthEast (3) : sprites buggy-0012, buggy-0014
   - South (4) : sprites buggy-0016, buggy-0018
   - SouthWest (5) : sprites buggy-0020, buggy-0022
   - West (6) : sprites buggy-0024, buggy-0026
   - NorthWest (7) : sprites buggy-0028, buggy-0030

3. **Script BuggyAnimator.cs :**
   - Calcule la direction depuis le vecteur de mouvement
   - Met à jour le paramètre `Direction` de l'Animator
   - Appelé depuis BuggyMovement pendant le déplacement

**Code exemple :**
```csharp
public class BuggyAnimator : MonoBehaviour
{
    private Animator animator;

    public void UpdateDirection(Vector2 direction)
    {
        // Calculer l'angle en degrés
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Convertir en index 0-7 (8 directions)
        int directionIndex = Mathf.RoundToInt(angle / 45f) % 8;

        animator.SetInteger("Direction", directionIndex);
    }
}
```

**Note :** Cette étape est optionnelle et peut être reportée après l'implémentation du système de sélection (Commit 9 de la ROADMAP).

---

## 🐛 Problèmes résolus

### **Bug 1 : Cellules fantômes occupées**
**Symptôme :** Quand on change de destination pendant un mouvement, des cellules restent marquées occupées.

**Cause :** L'ancien chemin n'était pas annulé et ses cellules pas libérées.

**Solution :**
- Ajout de `CancelCurrentMovement()` qui libère toutes les cellules du chemin non parcouru
- Système de destination en attente (`pendingTargetPosition`)
- L'unité finit sa case actuelle avant d'appliquer la nouvelle destination

### **Bug 2 : Erreur Input System**
**Symptôme :** `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class`

**Cause :** Le projet utilise New Input System, pas l'ancien.

**Solution :**
- Migration de `Input.GetKeyDown()` vers `Keyboard.current.numpad1Key.wasPressedThisFrame`
- Ajout de `using UnityEngine.InputSystem;`
- Ajout de `Unity.InputSystem` dans `CommandAndConquer.Units.asmdef`

### **Bug 3 : Namespace 'Grid' introuvable**
**Symptôme :** `error CS0234: The type or namespace name 'Grid' does not exist`

**Cause :** Référence d'assembly manquante.

**Solution :**
- Ajout de `"CommandAndConquer.Grid"` dans `CommandAndConquer.Units.asmdef`

---

## 📁 Structure des fichiers créés

```
Assets/_Project/Units/Buggy/
├── Data/
│   ├── BuggyData.cs                    ✅ ScriptableObject
│   ├── BuggyData.asset                 ✅ Configuration (moveSpeed=4.0)
│   └── Editor/
│       └── CreateBuggyData.cs          ✅ Utilitaire création asset
├── Scripts/
│   ├── BuggyController.cs              ✅ Contrôleur principal
│   ├── BuggyMovement.cs                ✅ Système déplacement case par case
│   └── BuggyTestMovement.cs            ✅ Script test pavé numérique
├── Prefabs/
│   └── Buggy.prefab                    ✅ Prefab complet
└── Sprites/
    ├── buggy-0000.png à buggy-0030.png ✅ 16 sprites animation
    └── (configurés automatiquement)
```

---

## 🔗 Dépendances

### **Scripts Core utilisés :**
- `UnitBase.cs` - Classe abstraite de base
- `IMovable.cs` - Interface déplacement
- `ISelectable.cs` - Interface sélection
- `GridPosition.cs` - Structure position grille
- `UnitData.cs` - ScriptableObject données

### **Scripts Grid utilisés :**
- `GridManager.cs` - Gestionnaire grille
- `GridCell.cs` - Cellule de grille

### **Packages Unity :**
- Unity.InputSystem - Nouveau système d'input

---

## 🎯 Prochaines étapes du plan global (ROADMAP)

Après validation du Buggy (étapes 7-8), reprendre le plan de la ROADMAP :

**Commit 9 : Système de sélection et déplacement**
- SelectionManager (clic gauche pour sélectionner)
- Commande de mouvement (clic droit pour déplacer)
- InputManager pour centraliser les inputs
- Feedback visuel de sélection

**Commit 10 : Deuxième unité (optionnel)**
- Créer une autre unité pour valider l'architecture
- Utiliser le template `/create-unit` pour gagner du temps

---

## 💡 Notes pour Claude Code

### **Pour reprendre le travail :**
1. Lire ce document pour connaître l'état actuel
2. Vérifier les todos avec la commande appropriée
3. Consulter `ROADMAP.md` pour le plan global
4. Les tests sont dans `BuggyTestMovement.cs` (touches pavé numérique)

### **Commandes utiles :**
- `/test-game` - Lance la scène Game dans Unity
- `/gen-commit` - Génère un message de commit
- Appuyer sur H en play mode - Affiche l'aide des contrôles

### **Fichiers de référence :**
- `ROADMAP.md` - Plan complet du projet
- `CONVENTIONS.md` - Standards de code
- `UNITS.md` - Catalogue des unités
- `CHANGELOG.md` - Historique des changements

### **Système de coordonnées :**
- Grille : entiers (5, 5)
- Monde : floats (5.5, 5.5) - toujours +0.5 pour centrage
- Conversion : `GridManager.GetGridPosition()` et `GetWorldPosition()`

---

**Dernière mise à jour :** 2025-01-21
**Étapes validées :** 1-7 / 8
**Prochaine action :** Étape 8 (animations optionnel) OU Commit 9 de ROADMAP (système de sélection)
