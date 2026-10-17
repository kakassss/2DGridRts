using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything that covers a rectangular area on the grid.</summary>
    public interface IOccupant
    {
        Vector2Int MinCell { get; }
        Vector2Int MaxCell { get; }
    }
}