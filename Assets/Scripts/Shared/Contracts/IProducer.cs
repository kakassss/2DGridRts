using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Something that can produce products (e.g. a Barracks producing soldiers).</summary>
    public interface IProducer
    {
        /// <summary>What this producer can make. Empty if it produces nothing (e.g. Power Plant).</summary>
        IReadOnlyList<IProduct> Producibles { get; }

        /// <summary>The cell where produced units prefer to appear.</summary>
        Vector2Int SpawnCell { get; }
    }
}
