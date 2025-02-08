using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BuildingLevelData
{
    public GameObject[] options;
}

[Serializable]
public class BuildingData
{
    public EBuilding type;
    public string title;
    public Sprite icon;
    public Sprite assistantIcon;
    public List<BuildingLevelData> levels;
}

public enum EBuilding
{
    Bank,
    Candyshop,
    Drinks,
    Fashion,
    Flowers,
    House,
    Police,
    PressConference, 
    Repair,
    Baloon,
    Boat, 
    Hospital,
    MagicShop,
    Market,
    TownHall,
    TrainStation,
    Umbrella
}

[CreateAssetMenu(menuName = "Configs/Buildings/Buildings Collection", fileName = "Buildings Collection")]
public class BuildingsConfig : ScriptableObject
{
    [SerializeField] private int[] citizenCountsForHouseLevels;
    [SerializeField] private BuildingData[] buildings;

    private Dictionary<EBuilding, BuildingData> dict = new ();
    
    public int[] CitizenCountsForHouseLevels => citizenCountsForHouseLevels;
    
    public BuildingData GetBuildingData(EBuilding type)
    {
        if (!dict.ContainsKey(type))
        {
            dict[type] = buildings.First(x => x.type == type);
        }

        return dict[type];
    }
}
