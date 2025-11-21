using UnityEngine;
using UnityEngine.InputSystem;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Buggy
{
    /// <summary>
    /// Script temporaire pour tester le déplacement du Buggy avec le clavier.
    /// À supprimer une fois le système de sélection implémenté.
    /// Utilise le New Input System.
    /// </summary>
    public class BuggyTestMovement : MonoBehaviour
    {
        private BuggyMovement buggyMovement;
        private Keyboard keyboard;

        private void Awake()
        {
            buggyMovement = GetComponent<BuggyMovement>();
            keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (buggyMovement == null || keyboard == null)
                return;

            // Pavé numérique = disposition spatiale de la carte (grille 20x20)
            // 7  8  9     →  Haut-gauche    Haut         Haut-droite
            // 4  5  6     →  Gauche         Centre       Droite
            // 1  2  3     →  Bas-gauche     Bas          Bas-droite

            if (keyboard.numpad1Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 1 : Bas-gauche (0, 0)");
                buggyMovement.MoveTo(new GridPosition(0, 0));
            }

            if (keyboard.numpad2Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 2 : Bas-centre (9, 0)");
                buggyMovement.MoveTo(new GridPosition(9, 0));
            }

            if (keyboard.numpad3Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 3 : Bas-droite (19, 0)");
                buggyMovement.MoveTo(new GridPosition(19, 0));
            }

            if (keyboard.numpad4Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 4 : Gauche-centre (0, 9)");
                buggyMovement.MoveTo(new GridPosition(0, 9));
            }

            if (keyboard.numpad5Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 5 : Centre (9, 9)");
                buggyMovement.MoveTo(new GridPosition(9, 9));
            }

            if (keyboard.numpad6Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 6 : Droite-centre (19, 9)");
                buggyMovement.MoveTo(new GridPosition(19, 9));
            }

            if (keyboard.numpad7Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 7 : Haut-gauche (0, 19)");
                buggyMovement.MoveTo(new GridPosition(0, 19));
            }

            if (keyboard.numpad8Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 8 : Haut-centre (9, 19)");
                buggyMovement.MoveTo(new GridPosition(9, 19));
            }

            if (keyboard.numpad9Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 9 : Haut-droite (19, 19)");
                buggyMovement.MoveTo(new GridPosition(19, 19));
            }

            // Numpad 0 : en dehors de la grille
            if (keyboard.numpad0Key.wasPressedThisFrame)
            {
                Debug.Log("[Test] Numpad 0 : En dehors de la grille (-5, -5)");
                buggyMovement.MoveTo(new GridPosition(-5, -5));
            }

            // Afficher l'aide
            if (keyboard.hKey.wasPressedThisFrame)
            {
                Debug.Log("=== BUGGY TEST CONTROLS (Pavé Numérique) ===\n" +
                    "Le pavé numérique correspond à la disposition spatiale :\n" +
                    "  7  8  9   →   Haut-gauche    Haut         Haut-droite\n" +
                    "  4  5  6   →   Gauche         Centre       Droite\n" +
                    "  1  2  3   →   Bas-gauche     Bas          Bas-droite\n" +
                    "  0         →   En dehors de la grille (-5,-5)\n" +
                    "  H         →   Afficher cette aide");
            }
        }
    }
}
