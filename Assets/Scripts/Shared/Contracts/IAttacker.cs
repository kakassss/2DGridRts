using UnityEngine;

/// <summary>Saldırı yapabilen her şey (menzil + hasar + konum).</summary>
public interface IAttacker
{
    int Damage { get; }
    float AttackInterval { get; }
    int AttackRange { get; }
    Vector2Int Cell { get; }
}
