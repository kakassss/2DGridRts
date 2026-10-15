using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class BuildingNeighbors
{
    public static void Find(Vector2Int origin, Vector2Int size, List<Vector2Int> gridNeighbors)
    {
        gridNeighbors.Clear();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                gridNeighbors.Add(new Vector2Int(origin.x + x, origin.y + y));
            }
        }
    }
}
public class PlacementManager : ITickable, IInitializable, IDisposable
{
    private readonly ProductSelectionService _service;
    private readonly GameConfigSO _gameConfigSo;
    private readonly GridManager _gridManager;
    private readonly MouseInput _mouseInput;
    private readonly DestroyManager _destroyManager;

    private readonly List<Vector2Int> _cellNeighbors = new List<Vector2Int>();

    private BuildingDataSO _currentData;
    private GameObject _ghost;
    private SpriteRenderer _ghostRenderer;
    private Vector2Int _currentOriginCell;
    private bool _canPlace;

    public bool IsPlacing => _ghost != null;

    public PlacementManager(GridManager gridManager, MouseInput mouseInput,
        GameConfigSO gameConfigSo, ProductSelectionService service, DestroyManager destroyManager)
    {
        _gridManager = gridManager;
        _mouseInput = mouseInput;
        _gameConfigSo = gameConfigSo;
        _service = service;
        _destroyManager = destroyManager;
    }

    public void Initialize()
    {
        _service.ProductSelected += StartPlacement;  
    } 
    public void Dispose()
    {
        _service.ProductSelected -= StartPlacement;
        if (_ghost != null) UnityEngine.Object.Destroy(_ghost);
    }

    // Update yerine
    public void Tick()
    {
        if (_ghost == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame)
        {
            CancelPlacement();
            return;
        }

        UpdateGhost();

        if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
            TryPlace();
    }

    private void StartPlacement(BuildingDataSO data)
    {
        if (_ghost != null)
            UnityEngine.Object.Destroy(_ghost);

        _currentData = data;
        _ghost = UnityEngine.Object.Instantiate(data.BuildingPrefab);
        _ghostRenderer = _ghost.GetComponent<SpriteRenderer>();
    }

    private void CancelPlacement()
    {
        UnityEngine.Object.Destroy(_ghost);
        _ghost = null;
        _ghostRenderer = null;
    }

    private void UpdateGhost()
    {
        Vector2Int size = _currentData.Size;

        _currentOriginCell = GetBuildingOrigin(_mouseInput.GetMouseInput(), size);
        _canPlace = CanPlace(_currentOriginCell, size);

        _ghost.transform.position = GetBuildingCenter(_currentOriginCell, size);
        _ghostRenderer.color = _canPlace ? _gameConfigSo.ValidPlacementColor : _gameConfigSo.InvalidPlacementColor;
    }

    private void TryPlace()
    {
        if (!_canPlace)
            return;

        var view = _ghost.GetComponent<BuildingView>();
        if (view == null)
        {
            Debug.LogError($"{_ghost.name} prefab'ında BuildingView yok.");
            return;
        }

        view.Initialize(_currentData, _currentOriginCell);

        BuildingNeighbors.Find(_currentOriginCell, _currentData.Size, _cellNeighbors);
        foreach (Vector2Int cell in _cellNeighbors)
            _gridManager.SetOccupant(cell.x, cell.y, view);

        _ghostRenderer.color = Color.white;
        _ghost = null;
        _ghostRenderer = null;
        
        _destroyManager.Track(view);
    }

    private bool CanPlace(Vector2Int origin, Vector2Int size)
    {
        BuildingNeighbors.Find(origin, size, _cellNeighbors);

        foreach (Vector2Int cell in _cellNeighbors)
        {
            if (!_gridManager.InBounds(cell.x, cell.y)) return false;
            if (_gridManager.GetPositionIsOccupied(cell.x, cell.y)) return false;
        }
        return true;
    }

    private Vector2Int GetBuildingOrigin(Vector3 mouseWorld, Vector2Int size)
    {
        float cellSize = _gameConfigSo.CellSize;
        int x = Mathf.FloorToInt(mouseWorld.x / cellSize - size.x / 2f + 0.5f);
        int y = Mathf.FloorToInt(mouseWorld.y / cellSize - size.y / 2f + 0.5f);
        return new Vector2Int(x, y);
    }

    private Vector3 GetBuildingCenter(Vector2Int origin, Vector2Int size)
    {
        float cellSize = _gameConfigSo.CellSize;
        return new Vector3((origin.x + size.x / 2f) * cellSize,
                           (origin.y + size.y / 2f) * cellSize, 0f);
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
