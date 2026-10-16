using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Saldırı yapabilen her şey (menzil + hasar + konum).</summary>
    public interface IAttacker
    {
        int Damage { get; }
        float AttackInterval { get; }
        Vector2Int Cell { get; }
    }
}