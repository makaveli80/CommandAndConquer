---
description: Génère un message de commit basé sur les derniers changements
tags: [git, project]
---

Analyse les derniers changements git et génère un message de commit approprié.

**Instructions:**

1. **Exécute les commandes git suivantes en parallèle:**
   - `git status` - Pour voir tous les fichiers non suivis et modifiés
   - `git diff` - Pour voir les changements non stagés
   - `git diff --staged` - Pour voir les changements déjà stagés

2. **Analyse tous les changements:**
   - Identifie la nature des modifications (nouvelle fonctionnalité, correction de bug, refactoring, documentation, etc.)
   - Repère les patterns communs dans les modifications
   - Comprends l'objectif global des changements

3. **Génère un message de commit qui suit les conventions:**
   - Format: `<type>: <description courte>`
   - Types possibles:
     - `feat`: Nouvelle fonctionnalité
     - `fix`: Correction de bug
     - `refactor`: Refactoring du code
     - `docs`: Documentation
     - `style`: Formatage, espaces, etc.
     - `test`: Ajout ou modification de tests
     - `chore`: Tâches de maintenance
   - La description doit être concise (max 50 caractères)
   - Focus sur le "pourquoi" plutôt que le "quoi"
   - En anglais

4. **Présente le message de commit généré** au format:
   ```
   Message de commit suggéré:
   <type>: <description>
   ```

5. **Demande confirmation** à l'utilisateur avant de créer le commit.

IMPORTANT: Ne crée PAS automatiquement le commit, présente juste le message suggéré.
