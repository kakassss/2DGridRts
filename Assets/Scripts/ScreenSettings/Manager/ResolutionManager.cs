using System;
using UnityEngine;

namespace Rts2D.ScreenSettings.Manager
{
    /// <summary>Cycles the window through resolution presets.</summary>
    public class ResolutionManager
    {
        private readonly struct Preset
        {
            public readonly int Width;
            public readonly int Height;
            public readonly string Ratio;

            public Preset(int width, int height, string ratio)
            {
                Width = width;
                Height = height;
                Ratio = ratio;
            }
        }

        private static readonly Preset[] Presets =
        {
            new Preset(1920, 1080, "16:9"),
            new Preset(1280, 720, "16:9"),
            new Preset(1440, 900, "16:10"),
            new Preset(1024, 768, "4:3"),
            new Preset(1280, 1024, "5:4"),
            new Preset(2560, 1080, "21:9"),
            new Preset(900, 900, "1:1"),
            new Preset(720, 1280, "9:16"),
        };

        // Max share of the desktop, so the title bar and taskbar stay visible
        private const float MaxDesktopFill = 0.92f;

        private int _index = -1;

        /// <summary>Raised with the new label after a preset is applied.</summary>
        public event Action<string> LabelChanged;

        /// <summary>Current window size, shown before any preset is applied.</summary>
        public string CurrentLabel => $"{Screen.width}x{Screen.height}";

        public void ApplyNextPreset()
        {
            _index = (_index + 1) % Presets.Length;
            Preset preset = Presets[_index];

            Vector2Int size = FitToDesktop(preset.Width, preset.Height);
            Screen.SetResolution(size.x, size.y, FullScreenMode.Windowed);

            LabelChanged?.Invoke($"{size.x}x{size.y}  ({preset.Ratio})");
        }

        /// <summary>Shrinks the size (same aspect) if it does not fit the desktop.</summary>
        private Vector2Int FitToDesktop(int width, int height)
        {
            Resolution desktop = Screen.currentResolution;
            float scale = Mathf.Min(
                desktop.width * MaxDesktopFill / width,
                desktop.height * MaxDesktopFill / height,
                1f);

            return new Vector2Int(Mathf.RoundToInt(width * scale), Mathf.RoundToInt(height * scale));
        }
    }
}
