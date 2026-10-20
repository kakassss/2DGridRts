using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>
    /// Something MoveManager can walk across the grid. Path and destination are kept by
    /// MoveManager itself, so a mover only has to expose where it is and how fast it goes.
    /// </summary>
    public interface IMovable : ISelectable, IOccupant
    {
        GameObject GameObject { get; }   // for the active check and the transform
        Vector2Int Cell { get; }
        float MoveSpeed { get; }

        void SetCell(Vector2Int cell);
    }
}
