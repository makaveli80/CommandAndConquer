---
description: Lance la scène de jeu dans Unity
tags: [project]
---

Lance la scène de jeu principale dans l'éditeur Unity.

Étapes:

1. **Vérifier que Unity est ouvert**:
   - Si Unity n'est pas ouvert, informer l'utilisateur d'ouvrir le projet d'abord

2. **Identifier la scène de jeu**:
   - Chercher la scène principale dans `Assets/_Project/Scenes/`
   - Par défaut: `Game.unity`
   - Si elle n'existe pas, proposer de la créer

3. **Instructions pour l'utilisateur**:
   - Dans Unity Editor: File > Open Scene > sélectionner la scène
   - Ou: Double-cliquer sur `Game.unity` dans le Project panel
   - Appuyer sur Play (▶) pour lancer le jeu

4. **Si la scène Game.unity n'existe pas**:
   - Proposer de créer une nouvelle scène
   - Configurer la scène avec:
     - Main Camera avec CameraController
     - GridManager
     - EventSystem pour l'Input System
   - Sauvegarder dans `Assets/_Project/Scenes/Game.unity`

5. **Tests à effectuer**:
   - Vérifier le déplacement de la caméra
   - Vérifier l'affichage de la grille
   - Tester le placement et déplacement des unités

Note: Unity ne peut pas être contrôlé directement en ligne de commande pendant qu'il est ouvert. Cette commande guide l'utilisateur pour lancer manuellement la scène de test.