using UnityEngine;
using Zenject;
using Rts2D.Selection.Manager;
using Rts2D.Selection.View;

namespace Rts2D.Selection.Installer
{
    public class SelectionInstaller : MonoInstaller
    {
       [SerializeField] private SelectionBoxView selectionBox;

       public override void InstallBindings()
       {
          Container.Bind<SelectionService>().AsSingle().NonLazy();
          Container.BindInterfacesAndSelfTo<SelectionManager>().AsSingle().NonLazy();
          Container.BindInstance(selectionBox);
       }
    }
}
