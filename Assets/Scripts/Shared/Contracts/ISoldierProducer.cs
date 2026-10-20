using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>A grid-occupying structure that can produce soldiers.</summary>
    public interface ISoldierProducer : IOccupant
    {
        /// <summary>False for buildings that cannot produce soldiers (e.g. Power Plant).</summary>
        bool CanProduceSoldiers { get; }

        /// <summary>The designated cell where produced soldiers prefer to emerge.</summary>
        Vector2Int SpawnCell { get; }
    }
}
