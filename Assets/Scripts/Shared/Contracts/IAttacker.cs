using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything that can attack.</summary>
    public interface IAttacker
    {
        int Damage { get; }
        float AttackInterval { get; }

        /// <summary>Cells away it can hit from (1 = melee).</summary>
        int AttackRange { get; }

        Vector2Int Cell { get; }
    }
}