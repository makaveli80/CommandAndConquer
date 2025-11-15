using UnityEngine;

namespace CommandAndConquer.Camera
{
    /// <summary>
    /// ScriptableObject defining camera movement boundaries.
    /// </summary>
    [CreateAssetMenu(fileName = "CameraBounds", menuName = "Command&Conquer/Camera/Camera Bounds")]
    public class CameraBounds : ScriptableObject
    {
        [Header("Movement Boundaries")]
        [Tooltip("Minimum X position the camera can move to")]
        public float minX = -10f;

        [Tooltip("Maximum X position the camera can move to")]
        public float maxX = 10f;

        [Tooltip("Minimum Y position the camera can move to")]
        public float minY = -10f;

        [Tooltip("Maximum Y position the camera can move to")]
        public float maxY = 10f;

        [Header("Zoom Boundaries")]
        [Tooltip("Minimum camera size/orthographic size")]
        public float minZoom = 3f;

        [Tooltip("Maximum camera size/orthographic size")]
        public float maxZoom = 15f;

        /// <summary>
        /// Clamps a position within the defined boundaries.
        /// </summary>
        /// <param name="position">The position to clamp</param>
        /// <returns>Clamped position within bounds</returns>
        public Vector3 ClampPosition(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, minX, maxX);
            position.y = Mathf.Clamp(position.y, minY, maxY);
            return position;
        }

        /// <summary>
        /// Clamps a zoom value within the defined zoom boundaries.
        /// </summary>
        /// <param name="zoom">The zoom value to clamp</param>
        /// <returns>Clamped zoom value</returns>
        public float ClampZoom(float zoom)
        {
            return Mathf.Clamp(zoom, minZoom, maxZoom);
        }
    }
}
