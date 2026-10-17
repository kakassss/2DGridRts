using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Service;
using Rts2D.Move.Manager;
using Rts2D.Placement.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Selection.Manager
{
    public class SelectionManager : ITickable
    {
        private readonly GridManager _gridManager;
        private readonly MouseInputService _mouseInputService;
        private readonly SelectionService _selectionService;
        private readonly PlacementManager _placement;
        private readonly MoveManager _move;
        private readonly AttackManager _attack;

        public SelectionManager(GridManager gridManager, MouseInputService mouseInputService,
            SelectionService selectionService, PlacementManager placement,
            MoveManager move, AttackManager attack)
        {
            _gridManager = gridManager;
            _mouseInputService = mouseInputService;
            _selectionService = selectionService;
            _placement = placement;
            _move = move;
            _attack = attack;
        }

        public void Tick()
        {
            if (_placement.IsPlacing) return;
            if (Mouse.current == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
                HandleLeftClick();
            else if (Mouse.current.rightButton.wasPressedThisFrame)
                HandleRightClick();
        }

        // Left click: select the building or soldier in the cell, clear the selection if empty
        private void HandleLeftClick()
        {
            GetClickedCell(out int x, out int y);
            IOccupant occupant = _gridManager.GetOccupant(x, y);

            if (occupant is ISelectable selected) _selectionService.Select(selected);
            else _selectionService.Clear();
        }

        // Right click: if the selected unit can move, attack or walk
        private void HandleRightClick()
        {
            if (_selectionService.Current is not IMovable movable) return;

            GetClickedCell(out int x, out int y);
            IOccupant clicked = _gridManager.GetOccupant(x, y);

            if (clicked is IDamageable target && !ReferenceEquals(clicked, movable))
            {
                _attack.Attack(movable, target);
            }
            else
            {
                _attack.Cancel(movable); // a new move order cancels the attack
                _move.MoveTo(movable, new Vector2Int(x, y));
            }
        }

        private void GetClickedCell(out int x, out int y)
        {
            _gridManager.GetXYPosition(_mouseInputService.GetWorldPosition(), out x, out y);
        }
    }
}
