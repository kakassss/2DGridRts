using UnityEngine;
using Rts2D.GameConfig.DataSO;

namespace Rts2D.Grid.Gizmos
{
    public class GridGizmos : MonoBehaviour
    {
        [SerializeField] private GameConfigSO _gameConfigSO;

        private void SetAreaPosition()
        {
            transform.position = new Vector3(_gameConfigSO.GridWidth / 2f * -1, _gameConfigSO.GridWidth / 2f * -1);
        }

        private void OnDrawGizmos()
        {
            // NOT: Bu dosyanın namespace'i "Rts2D.Grid.Gizmos" olduğu için basit "Gizmos" adı
            // doğrudan namespace'e bağlanıyor; bu yüzden UnityEngine.Gizmos tam nitelikli yazıldı.
            UnityEngine.Gizmos.color = Color.green;

            float size = _gameConfigSO.CellSize;
            float width = _gameConfigSO.GridWidth * size;
            float height = _gameConfigSO.GridHeight * size;

            // Dikey çizgiler
            for (int x = 0; x <= _gameConfigSO.GridWidth; x++)
            {
                Vector3 start = transform.position + new Vector3(x * size, 0, 0);
                Vector3 end = start + new Vector3(0, height, 0);

                UnityEngine.Gizmos.DrawLine(start, end);
            }

            // Yatay çizgiler
            for (int y = 0; y <= _gameConfigSO.GridHeight; y++)
            {
                Vector3 start = transform.position + new Vector3(0, y * size, 0);

                Vector3 end = start + new Vector3(width, 0, 0);

                UnityEngine.Gizmos.DrawLine(start, end);
            }

            //SetAreaPosition();
        }
    }
}
