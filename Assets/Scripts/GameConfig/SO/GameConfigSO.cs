using UnityEngine;

namespace Rts2D.GameConfig.DataSO
{
    [CreateAssetMenu(menuName = "GameConfig", fileName = "Data/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        [SerializeField] private int gridWidth = 15;
        [SerializeField] private int gridHeight = 15;
        [SerializeField, Min(1)] private int cellSize = 1;
        [SerializeField, Min(0f)] private float cameraMargin = 0.5f;
        [SerializeField] private Color validPlacementColor = new Color(0.2f, 1f, 0.3f, 0.45f);
        [SerializeField] private Color invalidPlacementColor = new Color(1f, 0.15f, 0.15f, 0.55f);

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public int CellSize => cellSize;
        public float CameraMargin => cameraMargin;
        public Color ValidPlacementColor => validPlacementColor;
        public Color InvalidPlacementColor => invalidPlacementColor;
    }
}
