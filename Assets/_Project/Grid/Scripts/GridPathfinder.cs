using System.Collections.Generic;
using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Grid
{
    /// <summary>
    /// Utilitaire statique pour calculer des chemins sur la grille.
    /// Utilisable par toutes les unités du jeu.
    /// </summary>
    public static class GridPathfinder
    {
        /// <summary>
        /// Calcule un chemin en ligne droite entre deux positions.
        /// Supporte les 8 directions (N, NE, E, SE, S, SW, W, NW).
        /// </summary>
        /// <param name="gridManager">Le gestionnaire de grille</param>
        /// <param name="start">Position de départ</param>
        /// <param name="end">Position d'arrivée</param>
        /// <returns>Liste de positions représentant le chemin, ou null si impossible</returns>
        public static List<GridPosition> CalculateStraightPath(
            GridManager gridManager,
            GridPosition start,
            GridPosition end)
        {
            if (gridManager == null)
            {
                Debug.LogError("[GridPathfinder] GridManager is null");
                return null;
            }

            // Vérifier que start et end sont valides
            if (!gridManager.IsValidGridPosition(start))
            {
                Debug.LogWarning($"[GridPathfinder] Start position {start} is invalid");
                return null;
            }

            if (!gridManager.IsValidGridPosition(end))
            {
                Debug.LogWarning($"[GridPathfinder] End position {end} is invalid");
                return null;
            }

            // Si déjà à la destination
            if (start == end)
            {
                return new List<GridPosition>();
            }

            List<GridPosition> path = new List<GridPosition>();
            GridPosition current = start;

            // Limite de sécurité pour éviter les boucles infinies
            const int MAX_ITERATIONS = 1000;
            int iterations = 0;

            while (current != end && iterations < MAX_ITERATIONS)
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
                    Debug.LogWarning($"[GridPathfinder] Path blocked: {next} is out of bounds");
                    return null;
                }

                // Ajouter au chemin
                path.Add(next);
                current = next;
            }

            if (iterations >= MAX_ITERATIONS)
            {
                Debug.LogError("[GridPathfinder] Path calculation exceeded max iterations!");
                return null;
            }

            return path;
        }

        /// <summary>
        /// Calcule la distance de Manhattan entre deux positions.
        /// Utile pour estimer la longueur d'un chemin.
        /// </summary>
        public static int GetManhattanDistance(GridPosition a, GridPosition b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        /// <summary>
        /// Calcule la distance de Chebyshev (diagonales comptent comme 1).
        /// Représente le nombre réel de déplacements en 8 directions.
        /// </summary>
        public static int GetChebyshevDistance(GridPosition a, GridPosition b)
        {
            return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
        }
    }
}
