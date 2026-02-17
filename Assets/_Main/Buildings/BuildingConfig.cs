using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

[CreateAssetMenu(menuName = "Configs/Buildings/Building", fileName = "New Building")]
public class BuildingConfig : ScriptableObject, IKey<Type>
{
    [SerializeField] private BuildingBase building;
    [SerializeField] private LocalizedString titleLocalized;

    public string title => titleLocalized.GetLocalizedString();
    public Sprite icon;
    public BuildingLevelData[] levels;

    private Type type;
    public Type Key
    {
        get
        {
            if (type == null)
            {
                type = building.GetType();
            }

            return type;
        }
    }
}

[Serializable]
public class BuildingLevelData
{
    public GameObject option;
    public GameObject[] variants;
}