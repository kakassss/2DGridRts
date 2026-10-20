using UnityEngine;
using Zenject;
using Rts2D.GameConfig.SO;

namespace Rts2D.GameCamera.Manager
{
    /// <summary>
    /// Keeps the whole game board visible at any resolution / aspect ratio
    /// by fitting the orthographic size to the grid and centering the camera on it.
    /// </summary>
    public class CameraFitManager : IInitializable, ITickable
    {
        private readonly Camera _camera;
        private readonly GameConfigSO _config;

        private int _lastWidth;
        private int _lastHeight;
        private Rect _lastRect;

        public CameraFitManager(Camera camera, GameConfigSO config)
        {
            _camera = camera;
            _config = config;
        }

        public void Initialize()
        {
            _camera.orthographic = true;
            Fit();
        }

        public void Tick()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight || _camera.rect != _lastRect)
                Fit();
        }

        private void Fit()
        {
            // A minimized window reports a zero-sized screen; skip to avoid dividing by zero.
            if (Screen.width <= 0 || Screen.height <= 0) return;

            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            _lastRect = _camera.rect;

            float cellSize = _config.CellSize;
            float margin = _config.CameraMargin;
            float gridWidth = _config.GridWidth * cellSize + margin * 2f;
            float gridHeight = _config.GridHeight * cellSize + margin * 2f;

            // Aspect ratio of the area the camera really draws (viewport rect included)
            float aspect = (Screen.width * _camera.rect.width) / (Screen.height * _camera.rect.height);

            // Smallest orthographic size that fits both the width and the height
            float sizeByHeight = gridHeight / 2f;
            float sizeByWidth = gridWidth / (2f * aspect);
            _camera.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);

            // Center the camera on the grid
            Transform t = _camera.transform;
            t.position = new Vector3(_config.GridWidth * cellSize / 2f, _config.GridHeight * cellSize / 2f,
                t.position.z);
        }
    }
}
