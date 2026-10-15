using System.Collections.Generic;
using UnityEngine;
using Rts2D.Unit.Soldier.DataSO;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Information panelinde gösterilebilen her şey (bina, asker).</summary>
    public interface ISelectable
    {
        string DisplayName { get; }
        Sprite Icon { get; }

        /// <summary>Bu seçilebilirin üretebildiği askerler. Üretim yoksa boş liste.</summary>
        IReadOnlyList<SoldierDataSo> Producibles { get; }
    }
}
