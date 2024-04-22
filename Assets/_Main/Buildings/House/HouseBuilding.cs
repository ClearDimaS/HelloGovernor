using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class BuildingLevel
{
    public ScaleAnimator[] colors;
}

public class HouseBuilding : MonoBehaviour
{
    [SerializeField] private BuildingLevel[] gfxLevels;

    private int colorOptionIndex = 0;
    private int levelIndex = 0;
    
    private void Awake()
    {
        colorOptionIndex = Random.Range(0, gfxLevels[0].colors.Length);
        levelIndex = Random.Range(0, gfxLevels.Length);
    }

    private void Start()
    {
        RefreshState();
    }

    private void RefreshState()
    {
        for (int i = 0; i < gfxLevels.Length; i++)
        {
            for (int j = 0; j < gfxLevels[i].colors.Length; j++)
            {
                if (j == colorOptionIndex && i == levelIndex)
                {
                    gfxLevels[i].colors[j].Show(true);
                }
                else
                {
                    gfxLevels[i].colors[j].Hide(true);
                }
            }
        }
    }
}
