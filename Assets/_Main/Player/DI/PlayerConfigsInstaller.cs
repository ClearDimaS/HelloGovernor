using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Configs/Player/DI/PlayerConfigsInstaller", fileName = "PlayerConfigsInstaller", order = 0)]
public class PlayerConfigsInstaller : ScriptableObjectInstaller<PlayerConfigsInstaller>
{
    [SerializeField] private PlayerSkinConfig skinConfig;
   
    public override void InstallBindings()
    {
        Container.Bind<PlayerSkinConfig>().FromInstance(skinConfig).AsSingle().NonLazy();
    }
}