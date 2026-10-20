using UnityEngine;
using UnityEngine.EventSystems;

namespace Rts2D.InputHandling.Manager
{
    public enum InteractionMode
    {
        Idle,
        Placing
    }

    /// <summary>
    /// Shared input state: which mode the player is in and whether this frame's click
    /// was already handled, so two systems never react to the same click.
    /// </summary>
    public class InteractionService
    {
        private int _consumedFrame = -1;

        public InteractionMode Mode { get; private set; } = InteractionMode.Idle;
        public bool IsPlacing => Mode == InteractionMode.Placing;
        public bool IsClickConsumed => _consumedFrame == Time.frameCount;

        public void SetMode(InteractionMode mode) => Mode = mode;

        /// <summary>Marks this frame's mouse click as handled.</summary>
        public void ConsumeClick() => _consumedFrame = Time.frameCount;

        /// <summary>
        /// True if a click on the game world should be handled right now: the cursor is not over the UI,
        /// no building is being placed, and no other system has used this frame's click yet.
        /// </summary>
        public bool CanUseWorldClick() => !IsPlacing && !IsClickConsumed && !IsPointerOverUI();

        /// <summary>True while the cursor is over a UI element (menus, buttons).</summary>
        public bool IsPointerOverUI()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
