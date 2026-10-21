using System;
using System.Collections.Generic;
using Rts2D.Shared.Contracts;

namespace Rts2D.Selection.Manager
{
    /// <summary>What the player has selected; the first item is shown in the panel.</summary>
    public class SelectionService
    {
        /// <summary>Raised when the selection changes and is not empty; passes the item to show.</summary>
        public event Action<ISelectable> Selected;
        public event Action Cleared;

        private readonly List<ISelectable> _selectedItems = new List<ISelectable>();

        public IReadOnlyList<ISelectable> SelectedItems => _selectedItems;

        /// <summary>The item shown in the panel, or null.</summary>
        public ISelectable Current => _selectedItems.Count > 0 ? _selectedItems[0] : null;

        public void Select(ISelectable selectable)
        {
            _selectedItems.Clear();
            _selectedItems.Add(selectable);
            Selected?.Invoke(selectable);
        }

        /// <summary>Replaces the selection (e.g. the soldiers in a drag box).</summary>
        public void SelectMany(IReadOnlyList<ISelectable> selectables)
        {
            if (selectables.Count == 0)
            {
                Clear();
                return;
            }

            _selectedItems.Clear();
            _selectedItems.AddRange(selectables);
            Selected?.Invoke(_selectedItems[0]);
        }

        /// <summary>Removes one item; clears the selection if none is left.</summary>
        public void Deselect(ISelectable selectable)
        {
            if (!_selectedItems.Remove(selectable)) return;

            if (_selectedItems.Count == 0)
                Cleared?.Invoke();
            else
                Selected?.Invoke(_selectedItems[0]); // refreshes the panel count
        }

        public void Clear()
        {
            _selectedItems.Clear();
            Cleared?.Invoke();
        }
    }
}
