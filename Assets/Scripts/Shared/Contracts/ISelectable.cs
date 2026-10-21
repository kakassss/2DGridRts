using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Anything the player can select and see in the panel.</summary>
    public interface ISelectable
    {
        string DisplayName { get; }
        Sprite Icon { get; }
    }
}
