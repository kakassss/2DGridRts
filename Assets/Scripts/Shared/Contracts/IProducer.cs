using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Something that produces products (e.g. a Barracks).</summary>
    public interface IProducer
    {
        /// <summary>What it can make; empty if nothing.</summary>
        IReadOnlyList<IProduct> Producibles { get; }

        /// <summary>Where produced units prefer to appear.</summary>
        Vector2Int SpawnCell { get; }
    }
}
