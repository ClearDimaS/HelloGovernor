using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

[CreateAssetMenu(menuName = "Configs/DI/ConfigsInstaller", fileName = "ConfigsInstaller")]
public class ProjectConfigsInstaller : ScriptableObjectInstaller<ProjectConfigsInstaller>
{
    [SerializeField] private GameConfig gameConfig;
    [FormerlySerializedAs("wishesConfig")] [SerializeField] private WishesCollectionConfig wishesCollectionConfig;
    [SerializeField] private BuildingsCollectionConfig buildingsCollectionConfig;
    
    public override void InstallBindings()
    {
        Container.Bind<GameConfig>().FromInstance(gameConfig).AsSingle().NonLazy();
        Container.Bind<WishesCollectionConfig>().FromInstance(wishesCollectionConfig).AsSingle().NonLazy();
        Container.Bind<BuildingsCollectionConfig>().FromInstance(buildingsCollectionConfig).AsSingle().NonLazy();
    }
}
