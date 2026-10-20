using System;
using Rts2D.Shared.Contracts;

namespace Rts2D.Shared.Selection
{
    public class SelectionService
    {
        public event Action<ISelectable> Selected;
        public event Action Cleared;

        public ISelectable Current { get; private set; }

        public void Select(ISelectable selectable)
        {
            Current = selectable;
            Selected?.Invoke(selectable);
        }

        public void Clear()
        {
            Current = null;
            Cleared?.Invoke();
        }
    }
}
