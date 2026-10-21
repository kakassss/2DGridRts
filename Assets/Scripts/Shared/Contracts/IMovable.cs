using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Something MoveManager can walk; MoveManager keeps its path.</summary>
    public interface IMovable : ISelectable, IOccupant
    {
        GameObject GameObject { get; }   // for the active check and the transform
        Vector2Int Cell { get; }
        float MoveSpeed { get; }

        void SetCell(Vector2Int cell);
    }
}
