using System.Collections.Generic;
using UnityEngine;

/// <summary>Information panelinde gösterilebilen her şey (bina, asker).</summary>
public interface ISelectable
{
    string DisplayName { get; }
    Sprite Icon { get; }

    /// <summary>Bu seçilebilirin üretebildiği askerler. Üretim yoksa boş liste.</summary>
    IReadOnlyList<SoldierDataSo> Producibles { get; }
}

public interface IAttacker
{
    int Damage { get; }
    float AttackInterval { get; }
    int AttackRange { get; }
    Vector2Int Cell { get; }
}