using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Manager;
using Rts2D.Selection.View;
using Rts2D.Shared.Contracts;

namespace Rts2D.Selection.Manager
{
    /// <summary>
    /// Left click selects the building or soldier under the cursor (or clears the selection on an empty cell).
    /// Left drag draws a box and selects every soldier inside it.
    /// </summary>
    public class SelectionManager : ITickable, IInitializable, IDisposable
    {
        // The mouse must move this far (screen pixels) while held before a click becomes a drag
        private const float DragThresholdPixels = 10f;

        private readonly GridManager _gridManager;
        private readonly MouseInputService _mouseInputService;
        private readonly SelectionService _selectionService;
        private readonly InteractionService _interaction;
        private readonly IDamageableTracker _damageableTracker;
        private readonly SelectionBoxView _selectionBox;

        // Reused buffer for the soldiers found inside the box
        private readonly List<ISelectable> _unitsInBox = new List<ISelectable>();

        private bool _isPressing;
        private bool _isDragging;
        private Vector2 _pressScreenPosition;
        private Vector3 _pressWorldPosition;

        public SelectionManager(GridManager gridManager, MouseInputService mouseInputService,
            SelectionService selectionService, InteractionService interaction,
            IDamageableTracker damageableTracker, SelectionBoxView selectionBox)
        {
            _gridManager = gridManager;
            _mouseInputService = mouseInputService;
            _selectionService = selectionService;
            _interaction = interaction;
            _damageableTracker = damageableTracker;
            _selectionBox = selectionBox;
        }

        public void Initialize() => _damageableTracker.Destroyed += HandleDestroyed;

        public void Dispose() => _damageableTracker.Destroyed -= HandleDestroyed;

        public void Tick()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame)
                BeginPress(mouse);
            else if (_isPressing && mouse.leftButton.isPressed)
                UpdateDrag(mouse);
            else if (_isPressing)
                EndPress(); // button released
        }

        private void BeginPress(Mouse mouse)
        {
            ResetPress(); // a new press always starts fresh, even if the last release was missed
            if (!_interaction.CanUseWorldClick()) return; // e.g. over the UI, or the click placed a building

            _isPressing = true;
            _pressScreenPosition = mouse.position.ReadValue();
            _pressWorldPosition = _mouseInputService.GetWorldPosition();
        }

        private void UpdateDrag(Mouse mouse)
        {
            Vector2 currentScreenPosition = mouse.position.ReadValue();

            if (!_isDragging)
            {
                float movedSqr = (currentScreenPosition - _pressScreenPosition).sqrMagnitude;
                if (movedSqr < DragThresholdPixels * DragThresholdPixels) return;

                _isDragging = true;
            }

            _selectionBox.Show(_pressScreenPosition, currentScreenPosition);
        }

        private void EndPress()
        {
            _isPressing = false;

            if (!_isDragging)
            {
                SelectAt(_pressWorldPosition);
                return;
            }

            ResetPress();
            SelectUnitsInBox(_pressWorldPosition, _mouseInputService.GetWorldPosition());
        }

        private void ResetPress()
        {
            _isPressing = false;
            _isDragging = false;
            _selectionBox.Hide();
        }

        // Click: the building or soldier on the cell, or nothing
        private void SelectAt(Vector3 worldPosition)
        {
            _gridManager.GetXYPosition(worldPosition, out int x, out int y);
            IOccupant occupant = _gridManager.GetOccupant(x, y);

            if (occupant is ISelectable selected) _selectionService.Select(selected);
            else _selectionService.Clear();
        }

        // Drag: every soldier whose cell is inside the box (buildings are left out, as in most RTS games)
        private void SelectUnitsInBox(Vector3 cornerA, Vector3 cornerB)
        {
            _gridManager.GetXYPosition(cornerA, out int ax, out int ay);
            _gridManager.GetXYPosition(cornerB, out int bx, out int by);

            // Only look at cells that are on the board
            int minX = Mathf.Max(Mathf.Min(ax, bx), 0);
            int maxX = Mathf.Min(Mathf.Max(ax, bx), _gridManager.Width - 1);
            int minY = Mathf.Max(Mathf.Min(ay, by), 0);
            int maxY = Mathf.Min(Mathf.Max(ay, by), _gridManager.Height - 1);

            _unitsInBox.Clear();
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (_gridManager.GetOccupant(x, y) is IMovable unit && !_unitsInBox.Contains(unit))
                        _unitsInBox.Add(unit);
                }
            }

            _selectionService.SelectMany(_unitsInBox);
        }

        // A dead building or unit can't stay selected
        private void HandleDestroyed(IDamageable dead) => _selectionService.Deselect(dead);
    }
}
