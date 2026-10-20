using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Service;
using Rts2D.Move.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Selection.Manager
{
    public class SelectionManager : ITickable, IInitializable, IDisposable
    {
        private readonly GridManager _gridManager;
        private readonly MouseInputService _mouseInputService;
        private readonly SelectionService _selectionService;
        private readonly InteractionService _interaction;
        private readonly MoveManager _move;
        private readonly AttackManager _attack;
        private readonly DestroyManager _destroy;

        public SelectionManager(GridManager gridManager, MouseInputService mouseInputService,
            SelectionService selectionService, InteractionService interaction,
            MoveManager move, AttackManager attack, DestroyManager destroy)
        {
            _gridManager = gridManager;
            _mouseInputService = mouseInputService;
            _selectionService = selectionService;
            _interaction = interaction;
            _move = move;
            _attack = attack;
            _destroy = destroy;
        }

        public void Initialize() => _destroy.Destroyed += HandleDestroyed;

        public void Dispose() => _destroy.Destroyed -= HandleDestroyed;

        public void Tick()
        {
            if (_interaction.IsPlacing || _interaction.IsClickConsumed) return;
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

        // A dead building or unit can't stay selected
        private void HandleDestroyed(IDamageable dead)
        {
            if (_selectionService.Current == dead)
                _selectionService.Clear();
        }

        private void GetClickedCell(out int x, out int y)
        {
            _gridManager.GetXYPosition(_mouseInputService.GetWorldPosition(), out x, out y);
        }
    }
}
