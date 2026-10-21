using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Something listed in the production menu or information panel.</summary>
    public interface IProduct
    {
        string DisplayName { get; }
        Sprite Icon { get; }
    }
}
