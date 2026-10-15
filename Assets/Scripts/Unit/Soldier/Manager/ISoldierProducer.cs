using UnityEngine;

public interface ISoldierProducer
{
    Vector2Int OriginCell { get; }
    Vector2Int Size { get; }
}