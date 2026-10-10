using UnityEngine;
using Zenject;

public class GridGizmos : MonoBehaviour
{
    [SerializeField] private GridDataSO gridDataSO;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float size = gridDataSO.Size;
        float width = gridDataSO.Width * size;
        float height = gridDataSO.Height * size;

        // Dikey çizgiler
        for (int x = 0; x <= gridDataSO.Width; x++)
        {
            Vector3 start = transform.position + new Vector3(x * size, 0, 0);
            Vector3 end = start + new Vector3(0, height, 0);

            Gizmos.DrawLine(start, end);
        }

        // Yatay çizgiler
        for (int y = 0; y <= gridDataSO.Height; y++)
        {
            Vector3 start = transform.position + new Vector3(0, y * size, 0);

            Vector3 end = start + new Vector3(width, 0, 0);

            Gizmos.DrawLine(start, end);
        }
    }
}