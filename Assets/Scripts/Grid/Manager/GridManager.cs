using UnityEngine;

public class GridManager : IGrid
{
    private Vector2Int[,] _grid;
    private GridDataSO _gridDataSo;
    
    public GridManager(GridDataSO gridDataSO)
    {
        _gridDataSo = gridDataSO;
        
        CreateGrid();
    }
    
    private void CreateGrid()
    {
        _grid = new Vector2Int[_gridDataSo.Width, _gridDataSo.Height];
        
        for (int i = 0; i < _gridDataSo.Width; i++)
        {
            for (int j = 0; j < _gridDataSo.Height; j++)
            {
                _grid[i, j] = new Vector2Int(i,j) * _gridDataSo.Size;
                //Debug.Log("Grid " + _grid[i,j]);
            }
        }
    }
    
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * _gridDataSo.Size;
    }
    
    public void GetXYPosition(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / _gridDataSo.Size);
        y = Mathf.FloorToInt(worldPosition.y / _gridDataSo.Size);
    }
    
    public bool InBounds(Vector3 worldPosition)
    {
        GetXYPosition(worldPosition,out int x, out int y);
        return x >= 0 && x < _gridDataSo.Width && y >= 0 && y < _gridDataSo.Height;
    }
    
    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < _gridDataSo.Width && y >= 0 && y < _gridDataSo.Height;
    }
}
