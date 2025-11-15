using UnityEngine;
using UnityEngine.InputSystem;

namespace CommandAndConquer.Camera
{
    /// <summary>
    /// Controls RTS-style camera movement with keyboard, mouse edge scrolling, and zoom.
    /// Uses Unity's New Input System.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField, Tooltip("Camera movement speed with keyboard")]
        private float keyboardMoveSpeed = 10f;

        [SerializeField, Tooltip("Camera movement speed with mouse edge scrolling")]
        private float edgeScrollSpeed = 10f;

        [SerializeField, Tooltip("Distance from screen edge to trigger scrolling (in pixels)")]
        private float edgeScrollBorderSize = 20f;

        [Header("Zoom Settings")]
        [SerializeField, Tooltip("Zoom speed with mouse wheel")]
        private float zoomSpeed = 2f;

        [SerializeField, Tooltip("Smoothness of zoom transition")]
        private float zoomSmoothness = 5f;

        [Header("Boundaries")]
        [SerializeField, Tooltip("Camera bounds configuration")]
        private CameraBounds cameraBounds;

        [Header("Options")]
        [SerializeField, Tooltip("Enable edge scrolling with mouse")]
        private bool enableEdgeScrolling = true;

        [SerializeField, Tooltip("Enable keyboard movement")]
        private bool enableKeyboardMovement = true;

        [SerializeField, Tooltip("Enable mouse wheel zoom")]
        private bool enableZoom = true;

        private UnityEngine.Camera _camera;
        private CameraInputActions _inputActions;
        private float _targetZoom;
        private Vector3 _targetPosition;
        private Vector2 _moveInput;
        private Vector2 _mousePosition;
        private float _zoomInput;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _inputActions = new CameraInputActions();

            if (_camera.orthographic)
            {
                _targetZoom = _camera.orthographicSize;
            }
            else
            {
                // For perspective camera, we'll use the Z position as "zoom" (distance from scene)
                _targetZoom = -transform.position.z;
            }

            _targetPosition = transform.position;
        }

        private void OnEnable()
        {
            _inputActions.Camera.Enable();

            // Subscribe to input events
            _inputActions.Camera.Move.performed += OnMove;
            _inputActions.Camera.Move.canceled += OnMove;
            _inputActions.Camera.Zoom.performed += OnZoom;
            _inputActions.Camera.MousePosition.performed += OnMousePosition;
        }

        private void OnDisable()
        {
            // Unsubscribe from input events
            _inputActions.Camera.Move.performed -= OnMove;
            _inputActions.Camera.Move.canceled -= OnMove;
            _inputActions.Camera.Zoom.performed -= OnZoom;
            _inputActions.Camera.MousePosition.performed -= OnMousePosition;

            _inputActions.Camera.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        private void OnZoom(InputAction.CallbackContext context)
        {
            _zoomInput = context.ReadValue<float>();
        }

        private void OnMousePosition(InputAction.CallbackContext context)
        {
            _mousePosition = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            HandleKeyboardMovement();
            HandleEdgeScrolling();
            HandleZoom();
            ApplyMovement();
        }

        /// <summary>
        /// Handles camera movement with WASD or Arrow keys.
        /// </summary>
        private void HandleKeyboardMovement()
        {
            if (!enableKeyboardMovement) return;

            Vector3 inputDirection = new Vector3(_moveInput.x, _moveInput.y, 0f);
            _targetPosition += inputDirection * keyboardMoveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Handles camera movement when mouse is at screen edges.
        /// </summary>
        private void HandleEdgeScrolling()
        {
            if (!enableEdgeScrolling) return;

            Vector3 edgeDirection = Vector3.zero;

            // Check each screen edge
            if (_mousePosition.x >= Screen.width - edgeScrollBorderSize)
                edgeDirection.x += 1f;
            if (_mousePosition.x <= edgeScrollBorderSize)
                edgeDirection.x -= 1f;
            if (_mousePosition.y >= Screen.height - edgeScrollBorderSize)
                edgeDirection.y += 1f;
            if (_mousePosition.y <= edgeScrollBorderSize)
                edgeDirection.y -= 1f;

            _targetPosition += edgeDirection.normalized * edgeScrollSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Handles camera zoom with mouse wheel.
        /// </summary>
        private void HandleZoom()
        {
            if (!enableZoom) return;

            if (Mathf.Abs(_zoomInput) > 0.01f)
            {
                // Normalize scroll input (wheel values can vary)
                float normalizedZoom = _zoomInput * 0.01f;
                _targetZoom -= normalizedZoom * zoomSpeed;

                if (cameraBounds != null)
                {
                    _targetZoom = cameraBounds.ClampZoom(_targetZoom);
                }

                // Reset zoom input after processing
                _zoomInput = 0f;
            }
        }

        /// <summary>
        /// Applies movement and zoom to the camera with bounds checking.
        /// </summary>
        private void ApplyMovement()
        {
            // Apply bounds to target position
            if (cameraBounds != null)
            {
                _targetPosition = cameraBounds.ClampPosition(_targetPosition);
            }

            // Smoothly move camera to target position
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10f);

            // Apply zoom
            if (_camera.orthographic)
            {
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _targetZoom, Time.deltaTime * zoomSmoothness);
            }
            else
            {
                // For perspective camera, adjust Z position (distance from scene)
                Vector3 pos = transform.position;
                pos.z = Mathf.Lerp(pos.z, -_targetZoom, Time.deltaTime * zoomSmoothness);
                transform.position = pos;
            }
        }

        /// <summary>
        /// Sets camera bounds at runtime.
        /// </summary>
        /// <param name="bounds">New camera bounds</param>
        public void SetBounds(CameraBounds bounds)
        {
            cameraBounds = bounds;
        }

        /// <summary>
        /// Instantly moves camera to a position (useful for initialization or teleporting).
        /// </summary>
        /// <param name="position">Target position</param>
        public void SetPosition(Vector3 position)
        {
            if (cameraBounds != null)
            {
                position = cameraBounds.ClampPosition(position);
            }

            transform.position = position;
            _targetPosition = position;
        }

        /// <summary>
        /// Sets the camera zoom level.
        /// </summary>
        /// <param name="zoom">Target zoom level</param>
        public void SetZoom(float zoom)
        {
            if (cameraBounds != null)
            {
                zoom = cameraBounds.ClampZoom(zoom);
            }

            _targetZoom = zoom;

            if (_camera.orthographic)
            {
                _camera.orthographicSize = zoom;
            }
            else
            {
                Vector3 pos = transform.position;
                pos.z = -zoom;
                transform.position = pos;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (cameraBounds == null) return;

            // Draw camera bounds in the scene view (2D XY plane)
            Gizmos.color = Color.yellow;

            Vector3 bottomLeft = new Vector3(cameraBounds.minX, cameraBounds.minY, 0);
            Vector3 bottomRight = new Vector3(cameraBounds.maxX, cameraBounds.minY, 0);
            Vector3 topLeft = new Vector3(cameraBounds.minX, cameraBounds.maxY, 0);
            Vector3 topRight = new Vector3(cameraBounds.maxX, cameraBounds.maxY, 0);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
#endif
    }
}
