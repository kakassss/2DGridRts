using UnityEngine;
using Rts2D.GameConfig.DataSO;

namespace Rts2D
{
    [RequireComponent(typeof(Camera))]
    public class GameBoardCameraView : MonoBehaviour
    {
        [SerializeField] private GameConfigSO config;
        [SerializeField] private float margin = 0.5f; 
        [SerializeField] private Camera camera;

        private int _lastWidth;
        private int _lastHeight;
        private Rect _lastRect;

        private void Awake()
        {
            camera.orthographic = true;
            Fit();
        }

        private void Update()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight || camera.rect != _lastRect)
                Fit();
        }

        private void Fit()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            _lastRect = camera.rect;

            float cellSize = config.CellSize;
            float gridWidth = config.GridWidth * cellSize + margin * 2f;
            float gridHeight = config.GridHeight * cellSize + margin * 2f;

            // Kameranın çizdiği gerçek alanın en-boy oranı (viewport rect dahil)
            float aspect = (Screen.width * camera.rect.width) / (Screen.height * camera.rect.height);

            // Hem genişliğe hem yüksekliğe sığacak en küçük ortho size
            float sizeByHeight = gridHeight / 2f;
            float sizeByWidth = gridWidth / (2f * aspect);
            camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

            // Kamerayı grid'in ortasına koy
            transform.position = new Vector3(
                config.GridWidth * cellSize / 2f,
                config.GridHeight * cellSize / 2f,
                transform.position.z);
        }
    }
}
