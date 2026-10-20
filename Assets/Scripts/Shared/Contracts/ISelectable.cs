using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything the player can click to select and see in the information panel.</summary>
    public interface ISelectable
    {
        string DisplayName { get; }
        Sprite Icon { get; }
    }
}
