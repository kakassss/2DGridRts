using System;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Selection.Manager
{
    /// <summary>
    /// Left click selects the building or soldier under the cursor (or clears the selection on an empty cell).
    /// </summary>
    public class SelectionManager : ITickable, IInitializable, IDisposable
    {
        private readonly GridManager _gridManager;
        private readonly MouseInputService _mouseInputService;
        private readonly SelectionService _selectionService;
        private readonly InteractionService _interaction;
        private readonly IDamageableTracker _damageableTracker;

        public SelectionManager(GridManager gridManager, MouseInputService mouseInputService,
            SelectionService selectionService, InteractionService interaction,
            IDamageableTracker damageableTracker)
        {
            _gridManager = gridManager;
            _mouseInputService = mouseInputService;
            _selectionService = selectionService;
            _interaction = interaction;
            _damageableTracker = damageableTracker;
        }

        public void Initialize() => _damageableTracker.Destroyed += HandleDestroyed;

        public void Dispose() => _damageableTracker.Destroyed -= HandleDestroyed;

        public void Tick()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;
            if (!_interaction.CanUseWorldClick()) return;

            _gridManager.GetXYPosition(_mouseInputService.GetWorldPosition(), out int x, out int y);
            IOccupant occupant = _gridManager.GetOccupant(x, y);

            if (occupant is ISelectable selected) _selectionService.Select(selected);
            else _selectionService.Clear();
        }

        // A dead building or unit can't stay selected
        private void HandleDestroyed(IDamageable dead)
        {
            if (_selectionService.Current == dead)
                _selectionService.Clear();
        }
    }
}
