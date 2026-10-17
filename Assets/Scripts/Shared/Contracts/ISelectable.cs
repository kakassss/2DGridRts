using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything that can be shown in the information panel (building, soldier).</summary>
    public interface ISelectable
    {
        string DisplayName { get; }
        Sprite Icon { get; }

        /// <summary>Soldiers this selectable can produce. Empty list if it produces nothing.</summary>
        IReadOnlyList<IProduct> Producibles { get; }
    }
}
