using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MouseInput : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    
    private GridManager _gridManager;
    
    [Inject]
    private void Construct(GridManager gridManager)
    {
        _gridManager = gridManager;
    }
    
    private Vector3 GetMouseInput()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        return worldPosition;
    }
    
    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _gridManager.GetXYPosition(GetMouseInput(),out int x, out int y);
            if (_gridManager.InBounds(x,y) == false)
            {
                Debug.Log("Mouse input is out of bounds");
                return;
            }
            
            Debug.Log("Data " + x + " " + y);
        }
    }
}
