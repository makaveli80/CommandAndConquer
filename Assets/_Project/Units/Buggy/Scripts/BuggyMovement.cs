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
        Idle,                  // Pas de mouvement, unité immobile
        Moving,                // En mouvement vers la destination
        Blocked,               // Mouvement impossible (chemin bloqué ou destination occupée)
        WaitingForNextCell     // Attend que la cellule suivante se libère
    }

    /// <summary>
    /// Gère le déplacement du Buggy sur la grille case par case.
    /// Utilise un pathfinding en ligne droite avec déplacement dans les 8 directions.
    /// </summary>
    public class BuggyMovement : MonoBehaviour
    {
        #region Fields

        // Référence au contrôleur (donne accès au contexte partagé)
        private BuggyController controller;

        // Machine à états
        private MovementState state = MovementState.Idle;
        private GridPosition destination;

        // Chemin de déplacement
        private List<GridPosition> movementPath;
        private int currentPathIndex;

        // Cellule cible actuelle
        private GridPosition targetCellPosition;
        private Vector3 targetWorldPosition;

        // Retry system (pour WaitingForNextCell)
        private float retryTimer = 0f;
        private const float RETRY_INTERVAL = 0.1f;  // Vérifie toutes les 0.1s
        private const int MAX_RETRIES = 30;         // 30 × 0.1s = 3 secondes max
        private int retryCount = 0;

        #endregion

        #region Properties

        // Propriété publique principale
        public bool IsMoving => state == MovementState.Moving;

        // Propriétés pour debug (lecture seule)
        public MovementState CurrentState => state;
        public GridPosition CurrentDestination => destination;
        public IReadOnlyList<GridPosition> CurrentPath => movementPath;
        public int PathIndex => currentPathIndex;
        public Vector3 CurrentTargetWorldPosition => targetWorldPosition;

        #endregion

        #region Unity Lifecycle (Entry Points)

        private void Awake()
        {
            controller = GetComponent<BuggyController>();
        }

        private void Update()
        {
            switch (state)
            {
                case MovementState.Idle:
                    break;

                case MovementState.Moving:
                    HandleMoving();
                    break;

                case MovementState.WaitingForNextCell:
                    HandleWaitingForNextCell();
                    break;

                case MovementState.Blocked:
                    break;
            }
        }

        #endregion

        #region Public API (Entry Points)

        /// <summary>
        /// Déplace l'unité vers une position cible sur la grille.
        /// Calcule le chemin et configure l'état, la réservation est gérée par HandleWaitingForNextCell().
        /// Si un mouvement est en cours, l'ancien mouvement est annulé et un nouveau est calculé.
        /// </summary>
        public void MoveTo(GridPosition targetPosition)
        {
            // Validation de la requête
            if (!IsValidMoveRequest(targetPosition))
                return;

            // Si déjà en mouvement, recalculer depuis la cellule cible actuelle
            if (state == MovementState.Moving)
            {
                if (!TryCalculatePath(targetCellPosition, targetPosition, out List<GridPosition> newPath))
                {
                    // Impossible d'atteindre la nouvelle destination - on continue vers l'ancienne
                    Debug.LogWarning($"[BuggyMovement] Cannot change direction to {targetPosition}, continuing to {destination}");
                    return;
                }

                // Changement de direction réussi
                destination = targetPosition;
                newPath.Insert(0, targetCellPosition);
                movementPath = newPath;
                currentPathIndex = 0;

                Debug.Log($"[BuggyMovement] Direction changed to {targetPosition} ({movementPath.Count} steps)");
                return;
            }

            // Calculer le chemin pour un nouveau mouvement
            if (!TryCalculatePath(controller.CurrentGridPosition, targetPosition, out movementPath))
            {
                state = MovementState.Blocked;
                Debug.LogWarning($"[BuggyMovement] No valid path to {targetPosition}");
                return;
            }

            // Configurer l'état - la réservation sera gérée par HandleWaitingForNextCell()
            destination = targetPosition;
            currentPathIndex = 0;
            state = MovementState.WaitingForNextCell;
            retryTimer = 0f;
            retryCount = 0;

            Debug.Log($"[BuggyMovement] Path calculated to {targetPosition} ({movementPath.Count} steps), waiting for first cell");
        }

        #endregion

        #region Private: Movement Execution

        /// <summary>
        /// Gère le mouvement progressif vers la cellule cible.
        /// </summary>
        private void HandleMoving()
        {
            float distance = Vector3.Distance(transform.position, targetWorldPosition);

            if (distance < 0.01f)
            {
                OnReachedCell();
            }
            else
            {
                float moveSpeed = controller.Context.Data != null ? controller.Context.Data.moveSpeed : 3f;
                float step = moveSpeed * Time.deltaTime;

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetWorldPosition,
                    step
                );
            }
        }

        /// <summary>
        /// Gère l'attente et la réservation des cellules (première cellule ou cellules suivantes).
        /// C'est le point d'entrée pour toute réservation atomique de cellules.
        /// Réessaie périodiquement jusqu'à ce que la cellule se libère ou timeout.
        /// </summary>
        private void HandleWaitingForNextCell()
        {
            retryTimer += Time.deltaTime;

            if (retryTimer >= RETRY_INTERVAL)
            {
                retryTimer = 0f;
                retryCount++;

                GridPosition nextCell = movementPath[currentPathIndex];

                // Tenter de réserver la cellule atomiquement
                if (controller.Context.GridManager.TryMoveUnitTo(controller, nextCell))
                {
                    // Réussi! Démarrer ou reprendre le mouvement
                    Debug.Log($"[BuggyMovement] Cell {nextCell} reserved, starting movement");
                    retryCount = 0;
                    state = MovementState.Moving;
                    StartMovingToNextCell();
                }
                else if (retryCount >= MAX_RETRIES)
                {
                    // Timeout après MAX_RETRIES tentatives
                    Debug.LogWarning($"[BuggyMovement] Gave up waiting for {nextCell} after {MAX_RETRIES} retries");
                    state = MovementState.Blocked;
                    retryCount = 0;
                }
            }
        }

        /// <summary>
        /// Appelé quand l'unité atteint une cellule du chemin.
        /// </summary>
        private void OnReachedCell()
        {
            // Snap à la position exacte
            transform.position = targetWorldPosition;

            // Update local tracking
            controller.UpdateGridPosition(targetCellPosition);

            currentPathIndex++;

            if (currentPathIndex < movementPath.Count)
            {
                // Tenter de réserver la PROCHAINE cellule atomiquement
                GridPosition nextCell = movementPath[currentPathIndex];

                if (controller.Context.GridManager.TryMoveUnitTo(controller, nextCell))
                {
                    // Cellule réservée! Continuer le mouvement
                    StartMovingToNextCell();
                }
                else
                {
                    // Cellule occupée → attendre
                    state = MovementState.WaitingForNextCell;
                    retryTimer = 0f;
                    retryCount = 0;
                    Debug.Log($"[BuggyMovement] Cell {nextCell} occupied, waiting...");
                }
            }
            else
            {
                // Arrivé à destination
                state = MovementState.Idle;
                movementPath = null;
                Debug.Log($"[BuggyMovement] Reached destination: {destination}");
            }
        }

        /// <summary>
        /// Démarre le mouvement vers la prochaine cellule du chemin.
        /// </summary>
        private void StartMovingToNextCell()
        {
            if (movementPath == null || currentPathIndex >= movementPath.Count)
            {
                state = MovementState.Idle;
                movementPath = null;
                return;
            }

            targetCellPosition = movementPath[currentPathIndex];
            targetWorldPosition = controller.Context.GridManager.GetWorldPosition(targetCellPosition);
        }

        #endregion

        #region Private: Validation & Pathfinding

        /// <summary>
        /// Valide si la requête de mouvement est possible.
        /// </summary>
        private bool IsValidMoveRequest(GridPosition targetPosition)
        {
            if (controller?.Context?.GridManager == null)
            {
                Debug.LogError("[BuggyMovement] GridManager not found in context!");
                return false;
            }

            if (targetPosition == controller.CurrentGridPosition)
            {
                Debug.Log("[BuggyMovement] Already at target position");
                return false;
            }

            if (!controller.Context.GridManager.IsValidGridPosition(targetPosition))
            {
                Debug.LogWarning($"[BuggyMovement] Invalid target position: {targetPosition}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calcule et valide un chemin entre deux positions.
        /// </summary>
        /// <param name="from">Position de départ</param>
        /// <param name="to">Position d'arrivée</param>
        /// <param name="path">Le chemin calculé (out)</param>
        /// <returns>True si le chemin est valide, False sinon</returns>
        private bool TryCalculatePath(GridPosition from, GridPosition to, out List<GridPosition> path)
        {
            path = GridPathfinder.CalculateStraightPath(controller.Context.GridManager, from, to);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning($"[BuggyMovement] No valid path from {from} to {to}");
                return false;
            }
            return true;
        }

        #endregion
    }
}
