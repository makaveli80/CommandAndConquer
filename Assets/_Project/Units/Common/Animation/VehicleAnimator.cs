using UnityEngine;
using CommandAndConquer.Core;

namespace CommandAndConquer.Units.Common
{
    /// <summary>
    /// Composant réutilisable pour gérer l'animation directionnelle des véhicules.
    /// Met à jour le sprite du véhicule en fonction de sa direction de mouvement.
    /// Fonctionne avec n'importe quel composant implémentant IMovementComponent
    /// (VehicleMovement, InfantryMovement, AircraftMovement, etc.).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class VehicleAnimator : MonoBehaviour
    {
        #region Configuration

        [Header("Animation Data")]
        [SerializeField]
        [Tooltip("Configuration des sprites pour les 8 directions")]
        private VehicleAnimationData animationData;

        [Header("Settings")]
        [SerializeField]
        [Tooltip("Si activé, affiche des logs de debug pour les changements de direction")]
        private bool debugMode = false;

        #endregion

        #region Private Fields

        private SpriteRenderer spriteRenderer;
        private IMovementComponent movementComponent;
        private DirectionType currentDirection;
        private DirectionType lastMovementDirection;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            movementComponent = GetComponent<IMovementComponent>();

            if (animationData == null)
            {
                Debug.LogError($"[VehicleAnimator] {gameObject.name}: AnimationData not assigned!", this);
                enabled = false;
                return;
            }

            if (!animationData.AreAllSpritesAssigned())
            {
                Debug.LogWarning($"[VehicleAnimator] {gameObject.name}: Some sprites are missing in AnimationData!", this);
            }

            // Initialiser avec la direction par défaut
            currentDirection = animationData.DefaultDirection;
            lastMovementDirection = animationData.DefaultDirection;
            UpdateSprite(currentDirection);
        }

        private void Update()
        {
            UpdateAnimation();
        }

        #endregion

        #region Animation Update

        /// <summary>
        /// Met à jour l'animation en fonction de l'état du mouvement.
        /// </summary>
        private void UpdateAnimation()
        {
            if (movementComponent == null || animationData == null)
                return;

            DirectionType newDirection;

            if (movementComponent.IsMoving)
            {
                // Calculer la direction depuis la position actuelle vers la cible
                Vector2 delta = new Vector2(
                    movementComponent.CurrentTargetWorldPosition.x - transform.position.x,
                    movementComponent.CurrentTargetWorldPosition.y - transform.position.y
                );

                // Seulement mettre à jour si le delta est significatif
                // Évite de réinitialiser la direction quand le véhicule est presque arrivé
                if (delta.sqrMagnitude >= 0.01f)
                {
                    newDirection = DirectionUtils.GetDirectionFromDelta(delta);
                    lastMovementDirection = newDirection; // Mémoriser la dernière direction de mouvement
                }
                else
                {
                    // Delta trop petit (presque arrivé), garder la dernière direction
                    newDirection = lastMovementDirection;
                }
            }
            else
            {
                // Si à l'arrêt, garder la dernière direction de mouvement
                newDirection = lastMovementDirection;
            }

            // Mettre à jour le sprite seulement si la direction a changé
            if (newDirection != currentDirection)
            {
                currentDirection = newDirection;
                UpdateSprite(currentDirection);

                if (debugMode)
                {
                    Debug.Log($"[VehicleAnimator] {gameObject.name} direction changed to {currentDirection} ({DirectionUtils.GetAngleFromDirection(currentDirection)}°)");
                }
            }
        }

        /// <summary>
        /// Met à jour le sprite en fonction de la direction.
        /// </summary>
        /// <param name="direction">Direction actuelle</param>
        private void UpdateSprite(DirectionType direction)
        {
            if (spriteRenderer == null || animationData == null)
                return;

            Sprite newSprite = animationData.GetSpriteForDirection(direction);

            if (newSprite != null)
            {
                spriteRenderer.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning($"[VehicleAnimator] {gameObject.name}: No sprite found for direction {direction}");
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Force la mise à jour de la direction (utile pour l'initialisation).
        /// </summary>
        /// <param name="direction">Direction à afficher</param>
        public void SetDirection(DirectionType direction)
        {
            currentDirection = direction;
            lastMovementDirection = direction;
            UpdateSprite(direction);
        }

        /// <summary>
        /// Retourne la direction actuelle du véhicule.
        /// </summary>
        public DirectionType CurrentDirection => currentDirection;

        #endregion

        #region Gizmos (Debug)

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!debugMode || movementComponent == null || !movementComponent.IsMoving)
                return;

            // Dessiner une flèche indiquant la direction actuelle
            Vector3 directionVector = Quaternion.Euler(0, 0, DirectionUtils.GetAngleFromDirection(currentDirection)) * Vector3.right;
            Vector3 arrowEnd = transform.position + directionVector * 0.5f;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, arrowEnd);
            Gizmos.DrawSphere(arrowEnd, 0.1f);
        }
#endif

        #endregion
    }
}