using UnityEngine;

namespace Rts2D.Shared.Contracts
{
    //Something that can be shown as an item in a product list (menu / information panel)
    public interface IProduct
    {
        string DisplayName { get; }
        Sprite Icon { get; }
    }
}
