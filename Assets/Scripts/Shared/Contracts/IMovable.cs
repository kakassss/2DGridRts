using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    public interface IMovable : ISelectable, IOccupant
    {
        GameObject GameObject { get; }   // aktif mi kontrolü ve transform için
        Vector2Int Cell { get; }
        float MoveSpeed { get; }
        Vector2Int Destination { get; set; }
        Queue<Vector2Int> Path { get; }

        void SetCell(Vector2Int cell);
    }
}
