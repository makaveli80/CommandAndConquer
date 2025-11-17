using System.Collections.Generic;
using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;

namespace CommandAndConquer.Units.Buggy
{
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

        // Chemin de déplacement
        private List<GridPosition> movementPath;
        private int currentPathIndex;

        // Mouvement vers une case
        private bool isMovingToCell;
        private GridPosition targetCellPosition;
        private Vector3 targetWorldPosition;

        // Nouvelle destination en attente
        private bool hasPendingTarget;
        private GridPosition pendingTargetPosition;

        // Propriété publique
        public bool IsMoving => isMovingToCell || (movementPath != null && movementPath.Count > 0);

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
        /// Si un mouvement est en cours, la nouvelle destination sera appliquée
        /// une fois que l'unité aura atteint le centre de la case actuelle.
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

            // Si un mouvement est en cours, mettre la nouvelle destination en attente
            if (isMovingToCell)
            {
                pendingTargetPosition = targetPosition;
                hasPendingTarget = true;
                Debug.Log($"[BuggyMovement] New destination pending: {targetPosition} (will apply after reaching current cell)");
                return;
            }

            // Vérifier que la cellule cible est libre
            if (!gridManager.IsFree(targetPosition))
            {
                Debug.LogWarning($"[BuggyMovement] Target cell {targetPosition} is occupied!");
                return;
            }

            // Calculer le chemin
            movementPath = CalculatePath(controller.CurrentGridPosition, targetPosition);

            if (movementPath == null || movementPath.Count == 0)
            {
                Debug.LogWarning("[BuggyMovement] No valid path found!");
                return;
            }

            // Démarrer le mouvement
            currentPathIndex = 0;
            MoveToNextCell();

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

                // Vérifier que la case est valide et libre
                if (!gridManager.IsValidGridPosition(next))
                {
                    Debug.LogWarning($"[BuggyMovement] Path blocked: {next} is out of bounds");
                    return null; // Chemin impossible
                }

                if (!gridManager.IsFree(next))
                {
                    Debug.LogWarning($"[BuggyMovement] Path blocked: {next} is occupied");
                    return null; // Chemin bloqué
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
        /// Démarre le mouvement vers la prochaine case du chemin.
        /// Gère l'occupation et la libération des cellules.
        /// </summary>
        private void MoveToNextCell()
        {
            if (movementPath == null || currentPathIndex >= movementPath.Count)
            {
                // Fin du chemin
                movementPath = null;
                return;
            }

            // Libérer la cellule actuelle
            GridCell currentCell = gridManager.GetCell(controller.CurrentGridPosition);
            if (currentCell != null)
            {
                currentCell.Release();
            }

            // Récupérer la prochaine case du chemin
            targetCellPosition = movementPath[currentPathIndex];

            // Occuper la nouvelle cellule
            GridCell targetCell = gridManager.GetCell(targetCellPosition);
            if (targetCell != null)
            {
                if (!targetCell.TryOccupy(controller))
                {
                    Debug.LogError($"[BuggyMovement] Failed to occupy cell {targetCellPosition}!");
                    movementPath = null;
                    return;
                }
            }

            // Calculer la position monde cible (centrée avec +0.5)
            targetWorldPosition = gridManager.GetWorldPosition(targetCellPosition);

            // Activer le mouvement
            isMovingToCell = true;
        }

        private void Update()
        {
            if (!isMovingToCell)
                return;

            // Calculer la distance restante
            float distance = Vector3.Distance(transform.position, targetWorldPosition);

            // Vérifier si on est arrivé (seuil de 0.01 unité)
            if (distance < 0.01f)
            {
                // Snap à la position exacte
                transform.position = targetWorldPosition;

                // Mettre à jour la position grille dans le controller
                controller.UpdateGridPosition(targetCellPosition);

                // Arrêter le mouvement vers cette case
                isMovingToCell = false;

                // Vérifier s'il y a une nouvelle destination en attente
                if (hasPendingTarget)
                {
                    hasPendingTarget = false;
                    GridPosition newTarget = pendingTargetPosition;

                    // Annuler le mouvement actuel
                    CancelCurrentMovement();

                    // Démarrer le nouveau mouvement
                    Debug.Log($"[BuggyMovement] Applying pending destination: {newTarget}");
                    MoveTo(newTarget);
                    return;
                }

                // Passer à la case suivante
                currentPathIndex++;

                if (currentPathIndex < movementPath.Count)
                {
                    // Encore des cases à traverser
                    MoveToNextCell();
                }
                else
                {
                    // Arrivé à destination finale
                    movementPath = null;
                    Debug.Log($"[BuggyMovement] Reached final destination: {targetCellPosition}");
                }
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
        /// Annule le mouvement en cours et libère toutes les cellules réservées.
        /// </summary>
        private void CancelCurrentMovement()
        {
            // Libérer toutes les cellules qui étaient réservées dans le chemin
            if (movementPath != null)
            {
                // Libérer les cellules non encore visitées du chemin
                for (int i = currentPathIndex; i < movementPath.Count; i++)
                {
                    GridCell cell = gridManager.GetCell(movementPath[i]);
                    if (cell != null && cell.IsOccupied)
                    {
                        cell.Release();
                    }
                }
            }

            // Réinitialiser l'état
            movementPath = null;
            currentPathIndex = 0;
            isMovingToCell = false;

            Debug.Log("[BuggyMovement] Current movement cancelled");
        }

    }
}
