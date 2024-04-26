using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Configs/DI/ConfigsInstaller", fileName = "ConfigsInstaller")]
public class ProjectConfigsInstaller : ScriptableObjectInstaller<ProjectConfigsInstaller>
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BuildingsConfig buildingsConfig;
    public override void InstallBindings()
    {
        Container.Bind<GameConfig>().FromInstance(gameConfig).AsSingle().NonLazy();
        Container.Bind<BuildingsConfig>().FromInstance(buildingsConfig).AsSingle().NonLazy();
    }
}
