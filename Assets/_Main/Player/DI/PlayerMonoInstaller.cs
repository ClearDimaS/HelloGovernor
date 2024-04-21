using System.Collections;
using System.Collections.Generic;
using Zenject;

public class PlayerMonoInstaller : MonoInstaller
{
   public override void InstallBindings()
   {
      Container.Bind<PlayerInput>().FromComponentInHierarchy().AsSingle().NonLazy();
      Container.Bind<PlayerController>().FromComponentInHierarchy().AsSingle().NonLazy();
      Container.Bind<PlayerSkinManager>().FromComponentInHierarchy().AsSingle().NonLazy();
   }
}