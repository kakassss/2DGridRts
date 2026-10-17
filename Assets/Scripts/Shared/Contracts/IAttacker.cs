using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything that can attack (damage + attack interval + position).</summary>
    public interface IAttacker
    {
        int Damage { get; }
        float AttackInterval { get; }
        Vector2Int Cell { get; }
    }
}