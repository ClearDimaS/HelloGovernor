using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class BuildingData
{
    public string title;
    public Sprite icon;
    public BuildingLevelData[] levels;
}

[Serializable]
public class BuildingLevelData
{
    public GameObject option;
}