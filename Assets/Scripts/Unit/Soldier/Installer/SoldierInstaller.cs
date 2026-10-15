using UnityEngine;
using Zenject;

public class SoldierInstaller : MonoInstaller
{
    [SerializeField] private Transform _soldierContainer;
    [SerializeField] private SoldierListSO _soldierList;

    public override void InstallBindings()
    {
        Container.BindInstance(_soldierList);
        
        Container.Bind<SoldierProductionService>().AsSingle();
        Container.Bind<SoldierPoolManager>().AsSingle().WithArguments(_soldierContainer);
        Container.BindInterfacesAndSelfTo<SoldierSpawnManager>().AsSingle().NonLazy();
        
        Container.Bind<GridPathfinderManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<MoveManager>().AsSingle().NonLazy();
    }
}