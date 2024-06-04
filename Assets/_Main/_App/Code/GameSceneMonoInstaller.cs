using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameSceneMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<WishGrantersManager>().FromComponentInHierarchy().AsSingle().NonLazy();
 
        Container.Bind<PlayerSkinManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CameraManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<UI_Manager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<HousesManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<EnvironmentManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<UpgradablePricesManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<WishesPool>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ChatGroupsPool>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CurrencyPool>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CurrencySingleStackPool>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<WishAssistantItemsPool>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<ThiefsManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<ThiefsPool>().FromComponentInHierarchy().AsSingle().NonLazy();

        Container.Bind<CompassManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<SkinChooser>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}
