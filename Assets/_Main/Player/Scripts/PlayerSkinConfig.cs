using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Player/Skins", fileName = "Player Skins", order = 0)]
public class PlayerSkinConfig : ScriptableObject
{
    public GameObject[] femaleSkins;
    public GameObject[] maleSkins;
}