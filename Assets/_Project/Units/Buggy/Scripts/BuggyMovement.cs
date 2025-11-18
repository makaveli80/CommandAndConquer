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

            // Si déjà en mouvement, annuler l'ancien mouvement
            if (state == MovementState.Moving)
            {
                CancelCurrentMovement();
            }

            // Sauvegarder la destination
            destination = targetPosition;

            // Vérifier que la cellule cible est libre
            if (!gridManager.IsFree(targetPosition))
            {
                Debug.LogWarning($"[BuggyMovement] Target cell {targetPosition} is occupied!");
                state = MovementState.Blocked;
                return;
            }

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
        /// Démarre le mouvement vers la prochaine cellule du chemin.
        /// Gère l'occupation et la libération des cellules.
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
                    // Échec : réoccuper la cellule actuelle et passer en état Blocked
                    if (currentCell != null)
                    {
                        currentCell.TryOccupy(controller);
                    }

                    Debug.LogWarning($"[BuggyMovement] Blocked - Failed to occupy cell {targetCellPosition}!");
                    state = MovementState.Blocked;
                    movementPath = null;
                    return;
                }
            }

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
        /// Annule le mouvement en cours et libère toutes les cellules réservées.
        /// IMPORTANT: Ne libère PAS la cellule actuelle où se trouve le Buggy.
        /// </summary>
        private void CancelCurrentMovement()
        {
            if (state != MovementState.Moving || movementPath == null)
                return;

            // Libérer UNIQUEMENT les cellules futures (pas la cellule actuelle)
            // currentPathIndex pointe vers la cellule actuelle, donc on commence à +1
            for (int i = currentPathIndex + 1; i < movementPath.Count; i++)
            {
                GridCell cell = gridManager.GetCell(movementPath[i]);
                if (cell != null && cell.IsOccupied)
                {
                    cell.Release();
                }
            }

            // Réinitialiser l'état
            movementPath = null;
            currentPathIndex = 0;
            state = MovementState.Idle;

            Debug.Log($"[BuggyMovement] Movement cancelled. Staying at {controller.CurrentGridPosition}");
        }

    }
}
