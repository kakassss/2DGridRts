using UnityEngine;
using Zenject;

public class GridInstaller : MonoInstaller
{
    [SerializeField] private GridDataSO gridDataSO;
    
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GridManager>().AsSingle().WithArguments(gridDataSO).NonLazy();
    }
}
