using UnityEngine;

namespace Rts2D.Unit.Soldier.Manager
{
    public interface ISoldierProducer
    {
        Vector2Int OriginCell { get; }
        Vector2Int Size { get; }
    }
}
