using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Manager;
using Rts2D.Move.Manager;
using Rts2D.Selection.Manager;
using Rts2D.Shared.Contracts;
using Rts2D.Utils;

namespace Rts2D.Command.Manager
{
    /// <summary>
    /// Turns a right click into an order for the selected units: attack what was clicked, or walk there.
    /// New kinds of orders (gather, rally point...) are added here, without touching selection.
    /// </summary>
    public class CommandManager : ITickable
    {
        private readonly GridManager _grid;
        private readonly MouseInputService _mouseInput;
        private readonly InteractionService _interaction;
        private readonly SelectionService _selection;
        private readonly MoveManager _move;
        private readonly AttackManager _attack;
        private readonly SurroundingCellFinder _cellFinder;

        // Reused buffers, so issuing an order does not allocate
        private readonly List<IMovable> _units = new List<IMovable>();
        private readonly List<Vector2Int> _destinations = new List<Vector2Int>();

        public CommandManager(GridManager grid, MouseInputService mouseInput, InteractionService interaction,
            SelectionService selection, MoveManager move, AttackManager attack, SurroundingCellFinder cellFinder)
        {
            _grid = grid;
            _mouseInput = mouseInput;
            _interaction = interaction;
            _selection = selection;
            _move = move;
            _attack = attack;
            _cellFinder = cellFinder;
        }

        public void Tick()
        {
            if (Mouse.current == null || !Mouse.current.rightButton.wasPressedThisFrame) return;
            if (!_interaction.CanUseWorldClick()) return; // e.g. over the UI, or the click cancelled a placement

            CollectSelectedUnits();
            if (_units.Count == 0) return; // only units take orders

            _grid.GetXYPosition(_mouseInput.GetWorldPosition(), out int x, out int y);
            IssueOrder(new Vector2Int(x, y));
        }

        /// <summary>All selected units attack whatever damageable stands on the cell, otherwise they walk there.</summary>
        private void IssueOrder(Vector2Int cell)
        {
            IOccupant clicked = _grid.GetOccupant(cell.x, cell.y);

            if (clicked == null)
            {
                MoveGroup(cell);
                return;
            }

            if (clicked is IDamageable target && !IsSelectedUnit(clicked))
            {
                foreach (IMovable unit in _units)
                    _attack.Attack(unit, target);
            }
            // Clicking one of the selected units themselves gives no order
        }

        /// <summary>Sends each unit to its own free cell: the clicked cell first, then the nearest ones around it.</summary>
        private void MoveGroup(Vector2Int cell)
        {
            _cellFinder.FindFreeCellsNear(cell, _units.Count, _destinations);

            for (int i = 0; i < _units.Count && i < _destinations.Count; i++)
            {
                _attack.Cancel(_units[i]); // a new move order cancels the attack
                _move.MoveTo(_units[i], _destinations[i]);
            }
        }

        private void CollectSelectedUnits()
        {
            _units.Clear();
            foreach (ISelectable selected in _selection.SelectedItems)
            {
                if (selected is IMovable unit)
                    _units.Add(unit);
            }
        }

        private bool IsSelectedUnit(IOccupant occupant)
        {
            foreach (IMovable unit in _units)
            {
                if (ReferenceEquals(unit, occupant))
                    return true;
            }
            return false;
        }
    }
}
