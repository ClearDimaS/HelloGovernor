using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Configs/DI/ConfigsInstaller", fileName = "ConfigsInstaller")]
public class ProjectConfigsInstaller : ScriptableObjectInstaller<ProjectConfigsInstaller>
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BuildingsConfig buildingsConfig;
    [SerializeField] private WishesConfig wishesConfig;
    [SerializeField] private WishItemsConfig wishItemsConfig;
    
    public override void InstallBindings()
    {
        Container.Bind<GameConfig>().FromInstance(gameConfig).AsSingle().NonLazy();
        Container.Bind<BuildingsConfig>().FromInstance(buildingsConfig).AsSingle().NonLazy();
        Container.Bind<WishesConfig>().FromInstance(wishesConfig).AsSingle().NonLazy();
        Container.Bind<WishItemsConfig>().FromInstance(wishItemsConfig).AsSingle().NonLazy();
    }
}
