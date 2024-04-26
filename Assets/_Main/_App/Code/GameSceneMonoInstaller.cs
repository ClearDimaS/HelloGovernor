using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameSceneMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerSkinManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CameraManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<UI_Manager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<HousesManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<EnvironmentManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<UpgradablePricesManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}
