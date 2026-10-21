using UnityEngine;
using UnityEngine.EventSystems;

namespace Rts2D.InputHandling.Manager
{
    public enum InteractionMode
    {
        Idle,
        Placing
    }

    /// <summary>Shared input state: the current mode and whether this frame's click was used.</summary>
    public class InteractionService
    {
        private int _consumedFrame = -1;

        public InteractionMode Mode { get; private set; } = InteractionMode.Idle;
        public bool IsPlacing => Mode == InteractionMode.Placing;
        public bool IsClickConsumed => _consumedFrame == Time.frameCount;

        public void SetMode(InteractionMode mode) => Mode = mode;

        /// <summary>Marks this frame's mouse click as handled.</summary>
        public void ConsumeClick() => _consumedFrame = Time.frameCount;

        /// <summary>True if a world click should be handled now.</summary>
        public bool CanUseWorldClick() => !IsPlacing && !IsClickConsumed && !IsPointerOverUI();

        /// <summary>True while the cursor is over a UI element.</summary>
        public bool IsPointerOverUI()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
