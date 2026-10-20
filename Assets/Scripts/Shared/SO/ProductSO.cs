using UnityEngine;
using Rts2D.Shared.Contracts;

namespace Rts2D.Shared.SO
{
    /// <summary>Base for every ScriptableObject that can be listed as a product (buildings, soldiers).</summary>
    public abstract class ProductSO : ScriptableObject, IProduct
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
    }
}
