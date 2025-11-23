# Guide de Migration - Phase 2 : Prefabs

Ce guide vous accompagne pour migrer les prefabs Buggy et Artillery vers le nouveau système basé sur la composition.

## Vue d'ensemble

**Objectif** : Remplacer les composants obsolètes par les nouveaux composants génériques.

**Ancien système** :
```
Buggy (GameObject)
├── BuggyController (obsolete)
├── BuggyMovement (obsolete)
├── SelectableComponent
├── VehicleAnimator
└── SpriteRenderer
```

**Nouveau système** :
```
Buggy (GameObject)
├── Unit (nouveau, générique)
├── VehicleMovement (refactorisé, générique)
├── SelectableComponent (inchangé)
├── VehicleAnimator (inchangé)
└── SpriteRenderer (inchangé)
```

---

## Prérequis

1. ✅ Phase 1 complétée (3 commits sur `refactor/component-based-units`)
2. ✅ Projet compile sans erreurs (warnings [Obsolete] normaux)
3. ✅ Unity Editor ouvert sur la branche `refactor/component-based-units`

---

## Étape 1 : Créer les nouveaux UnitData assets

Les anciens `BuggyData` et `ArtilleryData` héritaient de `UnitData` (abstract). Maintenant `UnitData` est concret, il faut créer de nouvelles instances.

### 1.1 Créer BuggyData.asset

1. **Project** → Right-click dans `Assets/_Project/Units/Buggy/Data/`
2. **Create** → **Command & Conquer** → **Unit Data**
3. Renommer → `BuggyData`
4. **Inspector** → Configurer :
   ```
   Unit Name: Buggy
   Description: Fast reconnaissance vehicle
   Move Speed: 4.0
   Can Move: ✓ (checked)
   Prefab: (laisser vide pour l'instant)
   ```
5. **Save** (Ctrl+S)

### 1.2 Créer ArtilleryData.asset

1. **Project** → Right-click dans `Assets/_Project/Units/Artillery/Data/`
2. **Create** → **Command & Conquer** → **Unit Data**
3. Renommer → `ArtilleryData`
4. **Inspector** → Configurer :
   ```
   Unit Name: Artillery
   Description: Slow heavy artillery vehicle
   Move Speed: 1.5
   Can Move: ✓ (checked)
   Prefab: (laisser vide pour l'instant)
   ```
5. **Save** (Ctrl+S)

**✅ Checkpoint** : Vous avez maintenant 2 nouveaux assets UnitData.

---

## Étape 2 : Migrer le prefab Buggy

### 2.1 Ouvrir le prefab

1. **Project** → `Assets/_Project/Units/Buggy/Prefabs/Buggy.prefab`
2. **Double-click** pour ouvrir en mode Prefab

### 2.2 Supprimer les composants obsolètes

**Dans l'Inspector** :
1. Supprimer **BuggyController** :
   - Right-click → **Remove Component**
2. Supprimer **BuggyMovement** :
   - Right-click → **Remove Component**

**⚠️ Important** : Ne supprimez PAS SelectableComponent, VehicleAnimator, ou SpriteRenderer !

### 2.3 Ajouter les nouveaux composants

1. **Add Component** → Taper "Unit" → **Unit** (CommandAndConquer.Units.Common)
   - Dans l'Inspector :
     - **Unit Data** → Assigner `BuggyData` (asset créé à l'étape 1.1)

2. **Add Component** → Taper "VehicleMovement" → **VehicleMovement** (CommandAndConquer.Units._Project.Units.Common.Vehicle)
   - Aucune configuration requise (auto-découvre le Unit component)

### 2.4 Vérifier les composants existants

**SelectableComponent** :
- Devrait déjà être présent
- **Visual Type** = `CornerBrackets`
- Aucune modification requise

**VehicleAnimator** :
- Devrait déjà être présent
- **Animation Data** → Assigner `BuggyAnimationData` (si pas déjà fait)
- **Debug Mode** = false (ou true pour debug)

**SpriteRenderer** :
- Devrait déjà être présent
- Sprite assigné
- Aucune modification requise

### 2.5 Ordre des composants (recommandé)

Pour une meilleure lisibilité, réorganiser les composants dans cet ordre :
1. Transform
2. **Unit** ← Nouveau
3. **VehicleMovement** ← Nouveau
4. SelectableComponent
5. VehicleAnimator
6. SpriteRenderer
7. BoxCollider2D

*Astuce* : Glisser-déposer les composants dans l'Inspector pour les réorganiser.

### 2.6 Sauvegarder

1. **File** → **Save** (Ctrl+S)
2. **Fermer** le mode Prefab (cliquer sur `<` en haut à gauche)

**✅ Checkpoint** : Le prefab Buggy utilise maintenant le nouveau système !

---

## Étape 3 : Migrer le prefab Artillery

Répétez exactement les mêmes étapes que pour Buggy, mais avec :
- **Prefab** : `Assets/_Project/Units/Artillery/Prefabs/Artillery.prefab`
- **Unit Data** : Assigner `ArtilleryData` (créé à l'étape 1.2)
- **Animation Data** : Assigner `ArtilleryAnimationData`

**✅ Checkpoint** : Les deux prefabs utilisent maintenant le nouveau système !

---

## Étape 4 : Tester dans Unity

### 4.1 Ouvrir la scène de jeu

1. **Project** → `Assets/_Project/Scenes/Game.unity`
2. **Double-click** pour ouvrir

### 4.2 Vérifier la scène

Dans la **Hierarchy**, chercher les instances de Buggy et Artillery déjà présentes.

**Si des instances existent** :
1. Sélectionner chaque instance
2. **Inspector** → En haut → **Overrides** → **Apply All** (pour mettre à jour depuis le prefab)

**Si aucune instance** :
1. Drag & drop les prefabs Buggy et Artillery dans la scène
2. Positionner sur la grille (ex: (5, 5) pour Buggy, (10, 10) pour Artillery)

### 4.3 Play Mode - Tests

**Appuyer sur Play ▶️** et tester :

#### Test 1 : Sélection
- ✅ Cliquer sur Buggy → Corner brackets apparaissent
- ✅ Cliquer sur Artillery → Corner brackets apparaissent
- ✅ Cliquer dans le vide → Désélection

#### Test 2 : Mouvement
- ✅ Sélectionner Buggy → Clic droit sur grille → Buggy se déplace
- ✅ Sélectionner Artillery → Clic droit sur grille → Artillery se déplace (plus lent)
- ✅ Observer les sprites qui changent selon la direction (8 directions)

#### Test 3 : Console
Vérifier les logs dans la Console :
- ✅ `[Unit] 'Buggy' initialized at (x, y)`
- ✅ `[Unit] 'Artillery' initialized at (x, y)`
- ✅ `[VehicleMovement] Path calculated to...`
- ✅ Aucune erreur rouge

#### Test 4 : Multi-sélection
- ✅ Drag box pour sélectionner plusieurs unités
- ✅ Corner brackets sur toutes les unités sélectionnées
- ✅ Clic droit → Toutes se déplacent vers la destination

**✅ Checkpoint** : Tout fonctionne comme avant, mais avec le nouveau système !

---

## Étape 5 : Nettoyage (optionnel)

Si vous souhaitez nettoyer immédiatement les anciennes données (ou attendre Phase 3) :

### 5.1 Supprimer les anciens ScriptableObjects

**⚠️ Attention** : Faire une sauvegarde avant de supprimer !

1. `Assets/_Project/Units/Buggy/Data/BuggyData.asset` (ancien, dérivé de UnitData abstract)
2. `Assets/_Project/Units/Artillery/Data/ArtilleryData.asset` (ancien)

**Ne supprimez PAS** :
- Le nouveau `BuggyData` (créé à l'étape 1.1)
- Le nouveau `ArtilleryData` (créé à l'étape 1.2)

### 5.2 Vérifier les références cassées

Après suppression :
1. **Edit** → **Preferences** → **External Tools** → **Regenerate project files**
2. Vérifier qu'il n'y a pas d'erreurs dans la Console

---

## Étape 6 : Commit des changements

Une fois les tests réussis :

```bash
git add .
git commit -m "feat: migrate Buggy and Artillery prefabs to composition-based system

- Replace BuggyController/BuggyMovement with Unit + VehicleMovement
- Replace ArtilleryController/ArtilleryMovement with Unit + VehicleMovement
- Create new UnitData assets (BuggyData.asset, ArtilleryData.asset)
- Tested: selection, movement, animations, multi-selection all working

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>"
```

---

## Troubleshooting

### Problème : "NullReferenceException" au runtime

**Cause** : Unit component n'a pas de UnitData assigné.

**Solution** :
1. Sélectionner le prefab
2. **Inspector** → **Unit** → **Unit Data** → Assigner `BuggyData` ou `ArtilleryData`

---

### Problème : "Unit not moving"

**Cause** : VehicleMovement component manquant ou mal configuré.

**Solution** :
1. Vérifier que **VehicleMovement** est présent sur le GameObject
2. Vérifier que **Unit** est présent (VehicleMovement a `[RequireComponent(typeof(Unit))]`)

---

### Problème : "Corner brackets not showing"

**Cause** : SelectableComponent mal configuré.

**Solution** :
1. **SelectableComponent** → **Visual Type** = `CornerBrackets`
2. Vérifier que **CornerBracketSelector** est aussi présent (ajouté automatiquement)

---

### Problème : Warnings [Obsolete] dans la Console

**Cause** : Normal ! Les anciens composants sont marqués obsolètes.

**Solution** :
- ✅ Ignorer pour l'instant (seront supprimés en Phase 3)
- ⚠️ Si vous avez encore BuggyController/BuggyMovement sur les prefabs → Les supprimer (étape 2.2)

---

## Phase 3 : Nettoyage (après migration réussie)

Une fois Phase 2 terminée et testée, Phase 3 consistera à :
1. Supprimer tous les fichiers obsolètes (8 fichiers)
2. Faire hériter Unit de MonoBehaviour (au lieu de UnitBase)
3. Mettre à jour CLAUDE.md et documentation

**Ne faites PAS Phase 3 avant d'avoir validé Phase 2 !**

---

## Résumé

**Phase 2 complétée** ✅ si :
- [x] 2 nouveaux UnitData assets créés
- [x] Prefab Buggy migré (Unit + VehicleMovement)
- [x] Prefab Artillery migré (Unit + VehicleMovement)
- [x] Tests réussis (sélection, mouvement, animations)
- [x] Commit créé

**Prochaine étape** : Phase 3 (nettoyage fichiers obsolètes)

---

**Besoin d'aide ?** Créer une issue sur GitHub ou demander à Claude Code.
