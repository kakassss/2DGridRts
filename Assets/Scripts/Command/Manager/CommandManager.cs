using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Manager;
using Rts2D.Move.Manager;
using Rts2D.Selection.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Command.Manager
{
    /// <summary>
    /// Turns a right click into an order for the selected unit: attack what was clicked, or walk there.
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

        public CommandManager(GridManager grid, MouseInputService mouseInput, InteractionService interaction,
            SelectionService selection, MoveManager move, AttackManager attack)
        {
            _grid = grid;
            _mouseInput = mouseInput;
            _interaction = interaction;
            _selection = selection;
            _move = move;
            _attack = attack;
        }

        public void Tick()
        {
            if (Mouse.current == null || !Mouse.current.rightButton.wasPressedThisFrame) return;
            if (!_interaction.CanUseWorldClick()) return; // e.g. over the UI, or the click cancelled a placement

            if (_selection.Current is not IMovable unit) return; // only units take orders

            _grid.GetXYPosition(_mouseInput.GetWorldPosition(), out int x, out int y);
            IssueOrder(unit, new Vector2Int(x, y));
        }

        /// <summary>Attacks whatever damageable stands on the cell, otherwise walks to the cell.</summary>
        private void IssueOrder(IMovable unit, Vector2Int cell)
        {
            IOccupant clicked = _grid.GetOccupant(cell.x, cell.y);

            if (clicked is IDamageable target && !ReferenceEquals(clicked, unit))
            {
                _attack.Attack(unit, target);
                return;
            }

            _attack.Cancel(unit); // a new move order cancels the attack
            _move.MoveTo(unit, cell);
        }
    }
}
