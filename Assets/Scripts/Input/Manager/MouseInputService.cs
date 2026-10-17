using UnityEngine;
using UnityEngine.InputSystem;

namespace Rts2D.InputHandling.Service
{
    /// <summary>Farenin dünya konumunu verir.</summary>
    public class MouseInputService
    {
        private readonly Camera _camera;

        public MouseInputService()
        {
            _camera = Camera.main; // her çağrıda Camera.main aramamak için önbelleğe alınır
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