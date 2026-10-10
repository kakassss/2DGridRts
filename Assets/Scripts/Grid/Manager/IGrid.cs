using UnityEngine;

public interface IGrid
{
    public Vector3 GetWorldPosition(int x, int y);
    public void GetXYPosition(Vector3 worldPosition, out int x, out int y);
}
