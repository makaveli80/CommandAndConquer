using UnityEngine;
using CommandAndConquer.Core;
using CommandAndConquer.Grid;
using CommandAndConquer.Units._Project.Units.Common.Vehicle;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// Composant optionnel pour visualiser le déplacement du Buggy avec des Gizmos.
    /// Peut être désactivé en production sans affecter la logique de mouvement.
    /// </summary>
    [RequireComponent(typeof(BuggyMovement))]
    public class BuggyMovementDebug : MonoBehaviour
    {
        private BuggyMovement movement;
        private BuggyController controller;

        private void Awake()
        {
            movement = GetComponent<BuggyMovement>();
            controller = GetComponent<BuggyController>();
        }

        private void OnDrawGizmos()
        {
            if (movement == null || controller?.Context?.GridManager == null)
                return;

            DrawStateIndicator();
            DrawPath();
            DrawCurrentTarget();
        }

        /// <summary>
        /// Dessine un cercle autour du Buggy selon son état (blanc/vert/orange/rouge).
        /// </summary>
        private void DrawStateIndicator()
        {
            Color stateColor = movement.CurrentState switch
            {
                MovementState.Idle => Color.white,
                MovementState.Moving => Color.green,
                MovementState.WaitingForNextCell => new Color(1f, 0.5f, 0f),  // Orange
                MovementState.Blocked => Color.red,
                _ => Color.gray
            };

            Gizmos.color = stateColor;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }

        /// <summary>
        /// Dessine le chemin complet : cellules + lignes + destination finale.
        /// </summary>
        private void DrawPath()
        {
            var path = movement.CurrentPath;
            if (path == null || path.Count == 0)
                return;

            DrawPathCells(path);
            DrawPathLines(path);
            DrawDestination();
        }

        /// <summary>
        /// Dessine les cellules du chemin (gris=visité, jaune=actuel, cyan=futur).
        /// </summary>
        private void DrawPathCells(System.Collections.Generic.IReadOnlyList<GridPosition> path)
        {
            int currentIndex = movement.PathIndex;

            for (int i = 0; i < path.Count; i++)
            {
                Vector3 cellWorldPos = controller.Context.GridManager.GetWorldPosition(path[i]);

                // Couleur selon progression
                if (i < currentIndex)
                    Gizmos.color = Color.gray;
                else if (i == currentIndex)
                    Gizmos.color = Color.yellow;
                else
                    Gizmos.color = Color.cyan;

                Gizmos.DrawWireCube(cellWorldPos, Vector3.one * 0.9f);
            }
        }

        /// <summary>
        /// Dessine les lignes reliant les cellules restantes du chemin.
        /// </summary>
        private void DrawPathLines(System.Collections.Generic.IReadOnlyList<GridPosition> path)
        {
            int currentIndex = movement.PathIndex;
            Vector3 previousPos = transform.position;

            Gizmos.color = Color.yellow;

            for (int i = currentIndex; i < path.Count; i++)
            {
                Vector3 cellWorldPos = controller.Context.GridManager.GetWorldPosition(path[i]);
                Gizmos.DrawLine(previousPos, cellWorldPos);
                previousPos = cellWorldPos;
            }
        }

        /// <summary>
        /// Dessine la destination finale en magenta.
        /// </summary>
        private void DrawDestination()
        {
            Vector3 finalDestPos = controller.Context.GridManager.GetWorldPosition(movement.CurrentDestination);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(finalDestPos, 0.5f);
        }

        /// <summary>
        /// Dessine la cellule cible actuelle et la ligne vers elle (si en mouvement).
        /// </summary>
        private void DrawCurrentTarget()
        {
            if (movement.CurrentState != MovementState.Moving)
                return;

            Vector3 targetPos = movement.CurrentTargetWorldPosition;

            // Sphère jaune sur la cible
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPos, 0.2f);

            // Ligne verte vers la cible
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, targetPos);
        }
    }
}
