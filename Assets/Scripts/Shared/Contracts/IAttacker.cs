using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything that can attack (damage + attack interval + range + position).</summary>
    public interface IAttacker
    {
        int Damage { get; }
        float AttackInterval { get; }

        /// <summary>How many cells away it can hit from (1 = only next to the target).</summary>
        int AttackRange { get; }

        Vector2Int Cell { get; }
    }
}