using System.Collections.Generic;
using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// États possibles du système de mouvement.
    /// </summary>
    public enum MovementState
    {
        Idle,        // Pas de mouvement, unité immobile
        Moving,      // En mouvement vers la destination
        Blocked      // Mouvement impossible (chemin bloqué ou destination occupée)
    }

    /// <summary>
    /// Gère le déplacement du Buggy sur la grille case par case.
    /// Utilise un pathfinding en ligne droite avec déplacement dans les 8 directions.
    /// </summary>
    public class BuggyMovement : MonoBehaviour
    {
        // Références
        private BuggyController controller;
        private GridManager gridManager;
        private BuggyData data;

        // Machine à états
        private MovementState state = MovementState.Idle;
        private GridPosition destination;

        // Chemin de déplacement
        private List<GridPosition> movementPath;
        private int currentPathIndex;

        // Cellule cible actuelle
        private GridPosition targetCellPosition;
        private Vector3 targetWorldPosition;

        // Propriété publique
        public bool IsMoving => state == MovementState.Moving;

        private void Awake()
        {
            controller = GetComponent<BuggyController>();
            gridManager = FindFirstObjectByType<GridManager>();
        }

        private void Start()
        {
            if (controller != null && controller.Data != null)
            {
                data = controller.Data;
            }
        }

        /// <summary>
        /// Déplace l'unité vers une position cible sur la grille.
        /// Si un mouvement est en cours, l'ancien mouvement est annulé et un nouveau est calculé.
        /// </summary>
        public void MoveTo(GridPosition targetPosition)
        {
            if (gridManager == null)
            {
                Debug.LogError("[BuggyMovement] GridManager not found!");
                return;
            }

            // Vérifier que la cible est différente de la position actuelle
            if (targetPosition == controller.CurrentGridPosition)
            {
                Debug.Log("[BuggyMovement] Already at target position");
                return;
            }

            // Vérifier que la position cible est valide
            if (!gridManager.IsValidGridPosition(targetPosition))
            {
                Debug.LogWarning($"[BuggyMovement] Invalid target position: {targetPosition}");
                return;
            }

            // Si déjà en mouvement, recalculer le chemin depuis la cellule en cours
            if (state == MovementState.Moving)
            {
                destination = targetPosition;

                // Calculer le nouveau chemin depuis la cellule qu'on est en train d'atteindre
                List<GridPosition> newPath = CalculatePath(targetCellPosition, targetPosition);

                if (newPath == null || newPath.Count == 0)
                {
                    Debug.LogWarning($"[BuggyMovement] No valid path to {targetPosition}");
                    state = MovementState.Blocked;
                    return;
                }

                // Insérer la cellule cible actuelle comme premier élément
                // (on continue vers elle, puis on suit le nouveau chemin)
                newPath.Insert(0, targetCellPosition);

                // Remplacer le chemin
                movementPath = newPath;
                currentPathIndex = 0; // On continue vers targetCellPosition (index 0)

                Debug.Log($"[BuggyMovement] Direction changed to {targetPosition} ({movementPath.Count} steps)");
                return;
            }

            // Sinon, démarrer un nouveau mouvement
            destination = targetPosition;

            // Calculer le chemin
            movementPath = CalculatePath(controller.CurrentGridPosition, targetPosition);

            if (movementPath == null || movementPath.Count == 0)
            {
                Debug.LogWarning($"[BuggyMovement] No valid path to {targetPosition}");
                state = MovementState.Blocked;
                return;
            }

            // Démarrer le mouvement
            currentPathIndex = 0;
            state = MovementState.Moving;
            StartMovingToNextCell();

            Debug.Log($"[BuggyMovement] Moving from {controller.CurrentGridPosition} to {targetPosition} ({movementPath.Count} steps)");
        }

        /// <summary>
        /// Calcule un chemin en ligne droite vers la cible.
        /// Déplacement dans les 8 directions (N, NE, E, SE, S, SW, W, NW).
        /// </summary>
        private List<GridPosition> CalculatePath(GridPosition start, GridPosition end)
        {
            List<GridPosition> path = new List<GridPosition>();
            GridPosition current = start;

            // Limite de sécurité pour éviter les boucles infinies
            int maxIterations = 1000;
            int iterations = 0;

            while (current != end && iterations < maxIterations)
            {
                iterations++;

                // Calculer la direction vers la cible (valeurs: -1, 0, ou +1)
                int deltaX = System.Math.Sign(end.x - current.x);
                int deltaY = System.Math.Sign(end.y - current.y);

                // Calculer la prochaine position
                GridPosition next = new GridPosition(current.x + deltaX, current.y + deltaY);

                // Vérifier que la case est valide
                if (!gridManager.IsValidGridPosition(next))
                {
                    Debug.LogWarning($"[BuggyMovement] Path invalid: {next} is out of bounds");
                    return null; // Chemin impossible
                }

                // Ajouter au chemin
                path.Add(next);
                current = next;
            }

            if (iterations >= maxIterations)
            {
                Debug.LogError("[BuggyMovement] Path calculation exceeded max iterations!");
                return null;
            }

            return path;
        }

        /// <summary>
        /// Démarre le mouvement vers la prochaine cellule du chemin.
        /// </summary>
        private void StartMovingToNextCell()
        {
            if (movementPath == null || currentPathIndex >= movementPath.Count)
            {
                // Fin du chemin (ne devrait pas arriver ici normalement)
                state = MovementState.Idle;
                movementPath = null;
                return;
            }

            // Récupérer la prochaine case du chemin
            targetCellPosition = movementPath[currentPathIndex];

            // Calculer la position monde cible (centrée avec +0.5)
            targetWorldPosition = gridManager.GetWorldPosition(targetCellPosition);
        }

        private void Update()
        {
            switch (state)
            {
                case MovementState.Idle:
                    // Rien à faire
                    break;

                case MovementState.Moving:
                    HandleMoving();
                    break;

                case MovementState.Blocked:
                    // Rien à faire (pour l'instant, le retry sera ajouté plus tard)
                    break;
            }
        }

        /// <summary>
        /// Gère le mouvement progressif vers la cellule cible.
        /// </summary>
        private void HandleMoving()
        {
            // Calculer la distance restante
            float distance = Vector3.Distance(transform.position, targetWorldPosition);

            // Vérifier si on est arrivé à la cellule (seuil de 0.01 unité)
            if (distance < 0.01f)
            {
                OnReachedCell();
            }
            else
            {
                // Déplacement progressif vers la cible
                float moveSpeed = data != null ? data.moveSpeed : 3f;
                float step = moveSpeed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetWorldPosition,
                    step
                );
            }
        }

        /// <summary>
        /// Appelé quand l'unité atteint une cellule du chemin.
        /// </summary>
        private void OnReachedCell()
        {
            // Snap à la position exacte
            transform.position = targetWorldPosition;

            // Mettre à jour la position grille dans le controller
            controller.UpdateGridPosition(targetCellPosition);

            // Passer à la cellule suivante
            currentPathIndex++;

            if (currentPathIndex < movementPath.Count)
            {
                // Encore des cellules à traverser
                StartMovingToNextCell();
            }
            else
            {
                // Arrivé à la destination finale
                state = MovementState.Idle;
                movementPath = null;
                Debug.Log($"[BuggyMovement] Reached destination: {destination}");
            }
        }

        /// <summary>
        /// Gizmos pour visualiser le déplacement en mode Scene.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (gridManager == null || controller == null)
                return;

            // Couleur selon l'état
            Color stateColor = state switch
            {
                MovementState.Idle => Color.white,
                MovementState.Moving => Color.green,
                MovementState.Blocked => Color.red,
                _ => Color.gray
            };

            // Position actuelle du Buggy
            Gizmos.color = stateColor;
            Gizmos.DrawWireSphere(transform.position, 0.3f);

            // Afficher le chemin complet
            if (movementPath != null && movementPath.Count > 0)
            {
                // Dessiner toutes les cellules du chemin
                for (int i = 0; i < movementPath.Count; i++)
                {
                    Vector3 cellWorldPos = gridManager.GetWorldPosition(movementPath[i]);

                    // Couleur différente pour les cellules visitées vs futures
                    if (i < currentPathIndex)
                    {
                        Gizmos.color = Color.gray; // Déjà visité
                    }
                    else if (i == currentPathIndex)
                    {
                        Gizmos.color = Color.yellow; // Cellule cible actuelle
                    }
                    else
                    {
                        Gizmos.color = Color.cyan; // Cellules futures
                    }

                    // Dessiner la cellule
                    Gizmos.DrawWireCube(cellWorldPos, Vector3.one * 0.9f);
                }

                // Dessiner les lignes du chemin restant (depuis la position actuelle)
                Vector3 previousPos = transform.position;
                Gizmos.color = Color.yellow;

                for (int i = currentPathIndex; i < movementPath.Count; i++)
                {
                    Vector3 cellWorldPos = gridManager.GetWorldPosition(movementPath[i]);
                    Gizmos.DrawLine(previousPos, cellWorldPos);
                    previousPos = cellWorldPos;
                }

                // Marquer la destination finale
                Vector3 finalDestPos = gridManager.GetWorldPosition(destination);
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(finalDestPos, 0.5f);
            }

            // Cellule cible actuelle (si en mouvement)
            if (state == MovementState.Moving)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(targetWorldPosition, 0.2f);

                // Ligne de la position actuelle vers la cible
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, targetWorldPosition);
            }
        }

    }
}
