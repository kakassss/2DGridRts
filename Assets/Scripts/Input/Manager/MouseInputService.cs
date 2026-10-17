using UnityEngine;
using UnityEngine.InputSystem;

namespace Rts2D.InputHandling.Service
{
    /// <summary>Provides the mouse position in world space.</summary>
    public class MouseInputService
    {
        private readonly Camera _camera;

        public MouseInputService(Camera camera)
        {
            _camera = camera;
        }

        public Vector3 GetWorldPosition()
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;

            return worldPosition;
        }
    }
}