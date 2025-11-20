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

        // Retry system (pour WaitingForNextCell)
        private float retryTimer = 0f;
        private const float RETRY_INTERVAL = 0.3f;  // Vérifie toutes les 0.3s
        private const int MAX_RETRIES = 20;         // 20 × 0.3s = 6 secondes max
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
            gridManager = FindFirstObjectByType<GridManager>();
        }

        private void Start()
        {
            if (controller != null && controller.Data != null)
            {
                data = controller.Data;
            }
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

            // Démarrer un nouveau mouvement
            if (!TryCalculatePath(controller.CurrentGridPosition, targetPosition, out movementPath))
            {
                state = MovementState.Blocked;
                return;
            }

            // Tenter de réserver la PREMIÈRE cellule atomiquement
            GridPosition firstCell = movementPath[0];

            if (!gridManager.TryMoveUnitTo(controller, firstCell))
            {
                // Première cellule occupée → attendre
                destination = targetPosition;
                currentPathIndex = 0;
                state = MovementState.WaitingForNextCell;
                retryTimer = 0f;
                retryCount = 0;
                Debug.Log($"[BuggyMovement] First cell {firstCell} occupied, waiting...");
                return;
            }

            // Cellule réservée! Démarrer le mouvement physique
            destination = targetPosition;
            currentPathIndex = 0;
            state = MovementState.Moving;
            StartMovingToNextCell();

            Debug.Log($"[BuggyMovement] Moving from {controller.CurrentGridPosition} to {targetPosition} ({movementPath.Count} steps)");
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
        /// Gère l'attente quand la cellule suivante est occupée.
        /// Réessaie périodiquement jusqu'à ce qu'elle se libère.
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
                if (gridManager.TryMoveUnitTo(controller, nextCell))
                {
                    // Réussi! Reprendre le mouvement
                    Debug.Log($"[BuggyMovement] Cell {nextCell} now free, resuming");
                    retryCount = 0;
                    state = MovementState.Moving;
                    StartMovingToNextCell();
                }
                else if (retryCount >= MAX_RETRIES)
                {
                    // Timeout
                    Debug.LogWarning($"[BuggyMovement] Gave up waiting for {nextCell}");
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

                if (gridManager.TryMoveUnitTo(controller, nextCell))
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
            targetWorldPosition = gridManager.GetWorldPosition(targetCellPosition);
        }

        #endregion

        #region Private: Validation & Pathfinding

        /// <summary>
        /// Valide si la requête de mouvement est possible.
        /// </summary>
        private bool IsValidMoveRequest(GridPosition targetPosition)
        {
            if (gridManager == null)
            {
                Debug.LogError("[BuggyMovement] GridManager not found!");
                return false;
            }

            if (targetPosition == controller.CurrentGridPosition)
            {
                Debug.Log("[BuggyMovement] Already at target position");
                return false;
            }

            if (!gridManager.IsValidGridPosition(targetPosition))
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
            path = GridPathfinder.CalculateStraightPath(gridManager, from, to);

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
