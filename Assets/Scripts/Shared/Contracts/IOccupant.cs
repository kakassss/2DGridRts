using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Grid üzerinde dikdörtgen bir alan kaplayan her şey.</summary>
    public interface IOccupant
    {
        Vector2Int MinCell { get; }
        Vector2Int MaxCell { get; }
    }
}