# Système de Caméra RTS

Documentation du système de caméra pour le projet Command & Conquer RTS.

## Vue d'ensemble

Le système de caméra RTS permet un contrôle fluide de la caméra avec :
- Déplacement clavier (WASD / Flèches)
- Déplacement par edge scrolling (souris aux bords de l'écran)
- Zoom avec la molette de souris
- Limites de déplacement configurables
- Support complet du New Input System

**Architecture :** Vue 2D pure sur les axes X, Y (pas de rotation de caméra nécessaire).

## Composants

### CameraController.cs
Contrôleur principal de la caméra. Gère les inputs et le mouvement.

**Emplacement :** `Camera/Scripts/CameraController.cs`

### CameraBounds.cs
ScriptableObject définissant les limites de mouvement de la caméra.

**Emplacement :** `Camera/Scripts/CameraBounds.cs`

### CameraInputActions.inputactions
Configuration des inputs via le New Input System.

**Emplacement :** `Camera/Scripts/CameraInputActions.inputactions`

## Installation dans l'éditeur

### Étape 1 : Configurer les Input Actions

1. Sélectionnez `Camera/Scripts/CameraInputActions.inputactions`
2. Dans l'Inspector :
   - Cochez **"Generate C# Class"**
   - Cliquez sur **"Apply"**
3. Unity va générer la classe C# automatiquement

### Étape 2 : Créer un CameraBounds asset

1. Dans le dossier `Camera/` :
   - Clic droit → Create → Command&Conquer → Camera → Camera Bounds
2. Nommez-le `DefaultCameraBounds`
3. Configurez les valeurs (voir Configuration ci-dessous)

### Étape 3 : Configurer la Main Camera

1. Ouvrez votre scène de jeu
2. Sélectionnez **Main Camera** dans la Hierarchy
3. **Transform :**
   - Position : `(0, 0, -10)`
   - Rotation : `(0, 0, 0)`
4. **Camera component :**
   - Projection : `Orthographic`
   - Size : `5` (ou selon vos besoins)
5. **Ajouter le composant :**
   - Add Component → CameraController
6. **Assigner le CameraBounds :**
   - Glissez `DefaultCameraBounds` dans le champ "Camera Bounds"

### Étape 4 : Tester

1. Lancez le Play mode
2. Testez les contrôles :
   - **WASD / Flèches** : Déplacement
   - **Bords de l'écran** : Edge scrolling
   - **Molette souris** : Zoom

## Configuration

### CameraBounds (ScriptableObject)

#### Movement Boundaries
```
minX = -10f    // Limite gauche
maxX = 10f     // Limite droite
minY = -10f    // Limite bas
maxY = 10f     // Limite haut
```

**Conseil :** Ajustez ces valeurs selon la taille de votre map.

**Exemple pour une map 20x20 :**
```
minX = -10f
maxX = 10f
minY = -10f
maxY = 10f
```

#### Zoom Boundaries
```
minZoom = 3f   // Zoom maximum (proche)
maxZoom = 15f  // Zoom minimum (loin)
```

**Note :** Pour une caméra orthographique, ces valeurs correspondent à `orthographicSize`.

### CameraController (Component)

#### Movement Settings
```
Keyboard Move Speed = 10f      // Vitesse de déplacement au clavier
Edge Scroll Speed = 10f        // Vitesse de edge scrolling
Edge Scroll Border Size = 20f  // Distance du bord (en pixels)
```

**Conseil :**
- Augmentez `Keyboard Move Speed` pour un déplacement plus rapide
- Augmentez `Edge Scroll Border Size` pour une zone de détection plus grande

#### Zoom Settings
```
Zoom Speed = 2f          // Vitesse du zoom
Zoom Smoothness = 5f     // Fluidité de la transition
```

**Conseil :** Augmentez `Zoom Smoothness` pour un zoom plus fluide.

#### Options
```
Enable Edge Scrolling = true     // Activer/désactiver edge scrolling
Enable Keyboard Movement = true  // Activer/désactiver clavier
Enable Zoom = true               // Activer/désactiver zoom
```

## API pour développeurs

### Méthodes publiques

#### SetBounds(CameraBounds bounds)
Change les limites de la caméra au runtime.

```csharp
CameraBounds newBounds = // ... votre asset
cameraController.SetBounds(newBounds);
```

#### SetPosition(Vector3 position)
Téléporte instantanément la caméra à une position.

```csharp
cameraController.SetPosition(new Vector3(5f, 5f, -10f));
```

**Note :** La position sera automatiquement clampée selon les bounds.

#### SetZoom(float zoom)
Définit le niveau de zoom instantanément.

```csharp
cameraController.SetZoom(8f);
```

**Note :** Le zoom sera automatiquement clampé selon les bounds.

## Exemples d'utilisation

### Exemple 1 : Téléporter la caméra au spawn du joueur

```csharp
using CommandAndConquer.Camera;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform playerSpawnPoint;

    private void Start()
    {
        Vector3 spawnPos = playerSpawnPoint.position;
        spawnPos.z = -10f; // Garder la caméra à Z = -10
        cameraController.SetPosition(spawnPos);
    }
}
```

### Exemple 2 : Changer les bounds selon le niveau

```csharp
using CommandAndConquer.Camera;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private CameraBounds level1Bounds;
    [SerializeField] private CameraBounds level2Bounds;

    public void LoadLevel(int levelIndex)
    {
        switch (levelIndex)
        {
            case 1:
                cameraController.SetBounds(level1Bounds);
                break;
            case 2:
                cameraController.SetBounds(level2Bounds);
                break;
        }
    }
}
```

### Exemple 3 : Désactiver temporairement les contrôles

```csharp
using CommandAndConquer.Camera;
using UnityEngine;

public class CinematicController : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;

    public void StartCinematic()
    {
        // Désactiver le component pendant la cinématique
        cameraController.enabled = false;
    }

    public void EndCinematic()
    {
        cameraController.enabled = true;
    }
}
```

## Architecture technique

### Vue 2D (X, Y)

Le système utilise une vue 2D pure :
- **Axe X :** Mouvement horizontal (gauche/droite)
- **Axe Y :** Mouvement vertical (haut/bas)
- **Axe Z :** Profondeur de la caméra (fixe à -10, ou zoom pour perspective)

**Avantages :**
- Pas de rotation de caméra nécessaire
- Compatible avec tous les systèmes 2D de Unity
- Raycast 2D simplifiés
- Debugging plus facile

### Input System

Le système utilise le **New Input System** de Unity :
- Actions définies dans `CameraInputActions.inputactions`
- Événements avec `InputAction.CallbackContext`
- Support multi-plateforme natif

**Actions définies :**
- **Move** : Vector2 (WASD + Flèches)
- **Zoom** : Axis (Molette souris)
- **MousePosition** : Vector2 (Position souris)

## Gizmos

Quand la caméra est sélectionnée dans l'éditeur, les bounds sont affichés en **jaune** dans la Scene view.

**Utilité :** Visualiser rapidement la zone de déplacement autorisée.

## Troubleshooting

### La caméra ne bouge pas
- Vérifiez que `CameraInputActions.inputactions` a bien sa classe C# générée
- Vérifiez que le composant CameraController est activé
- Vérifiez que les options (Enable Keyboard Movement, etc.) sont cochées

### La caméra se déplace trop vite/lentement
- Ajustez `Keyboard Move Speed` et `Edge Scroll Speed`

### Le edge scrolling ne fonctionne pas
- Vérifiez que `Enable Edge Scrolling` est coché
- Augmentez `Edge Scroll Border Size` pour une zone plus grande
- Vérifiez que la souris est bien dans la fenêtre de jeu

### Le zoom ne fonctionne pas
- Vérifiez que `Enable Zoom` est coché
- Vérifiez que les bounds de zoom (`minZoom`, `maxZoom`) permettent le zoom souhaité

### Erreurs de compilation
- Vérifiez que `CommandAndConquer.Camera.asmdef` référence bien `Unity.InputSystem`
- Régénérez la classe C# depuis `CameraInputActions.inputactions`

## Notes pour Claude

**Ce système de caméra :**
- Utilise le New Input System (pas l'ancien)
- Fonctionne sur les axes X, Y (vue 2D pure)
- La caméra doit avoir Z = -10 (ou négatif)
- Pas de rotation nécessaire (0, 0, 0)

**Pour modifier :**
- Les inputs : Éditer `CameraInputActions.inputactions`
- Les bounds : Créer/modifier des assets `CameraBounds`
- Le comportement : Modifier `CameraController.cs`
