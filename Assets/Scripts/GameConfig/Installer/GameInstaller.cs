using UnityEngine;
using Zenject;
using Rts2D.GameConfig.DataSO;
using Rts2D.Pooling;
using Rts2D.ProductionMenu.DataSO;
using Rts2D.ProductionMenu.View;

namespace Rts2D.GameConfig.Installer
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Data")]
        [SerializeField] private ProductDataSO catalog;
        [SerializeField] private GameConfigSO configSo;

        [Header("Prefabs")]
        [SerializeField] private ProductItemView productItemPrefab;

        [Header("Pool sizes")]
        [SerializeField] private int productItemPoolSize = 12;

        [Header("Scene References")]
        [SerializeField] private Transform poolContainer;
        [SerializeField] private Camera mainCamera;
        public override void InstallBindings()
        {
            Container.BindInstance(catalog);
            Container.BindInstance(configSo);

            Container.Bind<ObjectPool<ProductItemView>>()
                .AsSingle().WithArguments(productItemPrefab, poolContainer, productItemPoolSize);
        }
    }
}
