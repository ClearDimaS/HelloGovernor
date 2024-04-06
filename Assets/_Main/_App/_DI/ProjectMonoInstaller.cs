using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Application.targetFrameRate = 60;
        Container.Bind<PlayerDataRepository>().AsSingle().NonLazy();
        Container.Bind<LocalCacheManager>().AsSingle().NonLazy();
    }
}
