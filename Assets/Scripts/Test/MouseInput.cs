using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class MouseInput
{
    private GridManager _gridManager;
    
    [Inject]
    private void Construct(GridManager gridManager)
    {
        _gridManager = gridManager;
    }
    
    public Vector3 GetMouseInput()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        return worldPosition;
    }

    
    public bool TryGetMouseClickPosition(out Vector3 position)
    {
        position = Vector3.zero;

        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return false;

        _gridManager.GetXYPosition(GetMouseInput(), out int x, out int y);

        if (!_gridManager.InBounds(x, y))
        {
            Debug.Log("Mouse input is out of bounds");
            return false;
        }

        position = _gridManager.GetWorldPosition(x, y);
        return true;
    }

    public bool TryGetMousePosOnGrid(out Vector3 position)
    {
        position = Vector3.zero;
        
        _gridManager.GetXYPosition(GetMouseInput(), out int x, out int y);

        if (!_gridManager.InBounds(x, y))
        {
            Debug.Log("Mouse input is out of bounds");
            return false;
        }

        position = _gridManager.GetWorldPosition(x, y);
        return true;
    }
    
}
