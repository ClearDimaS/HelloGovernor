using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class MinMaxData
{
    public Vector3 min;
    public Vector3 max;
    public Vector3 Center => (min + max) / 2f;
    public Vector3 Size => new Vector3(Mathf.Abs(min.x - max.x), Mathf.Abs(min.y - max.y), Mathf.Abs(min.z - max.z));
}
public class UpgradableBuilding : UpgradableObject
{
    [Inject] private DiContainer container;
    [Inject] private BuildingsCollectionConfig buildingsCollection;
    [Inject] private UpgradablePricesManager _upgradablePricesManager;

    [SerializeField] private bool addObstacles = true;
    [SerializeField] private Vector2 minMaxX;
    [SerializeField] private Vector2 minMaxZ;
    [SerializeField] private MinMaxData[] allowedSides;
    [SerializeField] private Transform boughtRoot;
    private BuildingBase buildingBase;
    private GameObject spawnedGFX;
    private int spawnedLevel = -2;
    public Vector3 Center => transform.position;
    public BuildingBase Building => buildingBase;

    protected override void OnAwake()
    {
        base.OnAwake();
        buildingBase = GetComponent<BuildingBase>();
    }

    private void OnDrawGizmos()
    {
        var color = Color.red;
        color.a = 0.4f;
        Gizmos.color = color;
        foreach (var side in allowedSides)
        {
            Gizmos.DrawCube(side.Center + Center + new Vector3(0, 0.1f, 0), side.Size);
        }
    }

    public Vector3 GetRandomPosition()
    {
        var randIndex = Random.Range(0, allowedSides.Length);
        var side = allowedSides[randIndex];
        var t = Random.Range(0, 1f);
        var pos = Center + Vector3.Lerp(side.min, side.max, t);
        NavMesh.SamplePosition(pos, out var hit, 10f, -1);
        return hit.position;
    }

    public List<Price> GetPricesCopy()
    {
        return _upgradablePricesManager.GetLevelPrices(this).Select(x => x.GetCopy()).ToList();
    }
    
    protected override List<Price> GetPrices()
    {
        return _upgradablePricesManager.GetLevelPrices(this);
    }

    protected override void OnUpgrade()
    {
        base.OnUpgrade();
        buildingBase.OnUpgrade();
    }

    protected override void RefreshLevelGFX(bool instant)
    {
        base.RefreshLevelGFX(instant);

        if (spawnedLevel != data.level && data.level > 0)
        {
            if (!boughtRoot.gameObject.activeSelf)
            {
                boughtRoot.gameObject.SetActive(true);
            }
            
            spawnedLevel = data.level;
            var buildingData = buildingsCollection.GetBuildingData(buildingBase);
            var levelIndex = Mathf.Clamp(data.level - 1, 0, buildingData.levels.Length - 1);
            var level = buildingData.levels[levelIndex];
            var optionPrefab = level.option;
            if (level.variants != null && level.variants.Length > 0)
            {
                var allCount = level.variants.Length + 1;
                var index = Random.Range(0, allCount);
                if (index > 0)
                {
                    optionPrefab = level.variants[index - 1];
                }
            }
            if (spawnedGFX != null)
            {
                Destroy(spawnedGFX);
            }

            spawnedGFX = container.InstantiatePrefab(optionPrefab, levels[levelIndex].transform);
            spawnedGFX.transform.localPosition = Vector3.zero;
            spawnedGFX.transform.localRotation = Quaternion.identity;

            if (instant)
            {
                spawnedGFX.transform.localScale = Vector3.one;
            }
            else
            {
                spawnedGFX.transform.localScale = Vector3.zero;
                spawnedGFX.transform.DOScale(Vector3.one * 1.3f, 0.3f).OnComplete(() =>
                {
                    spawnedGFX.transform.DOScale(1f, 0.15f);
                });
            }

            var colliders = spawnedGFX.GetComponentsInChildren<BoxCollider>(true);
            if (addObstacles)
            {
                foreach (var collider in colliders)
                {
                    var obstacleGO = new GameObject($"{collider.name}_obstacle");
                    obstacleGO.transform.SetParent(collider.transform, false);
                    obstacleGO.transform.localPosition = Vector3.zero;
                    obstacleGO.transform.localRotation = Quaternion.identity;
                    obstacleGO.transform.localScale = Vector3.one;
                
                    var obstacle = obstacleGO.AddComponent<NavMeshObstacle>();
                    obstacle.shape = NavMeshObstacleShape.Box;
                    obstacle.carving = true;
                    obstacle.carveOnlyStationary = true;
                    obstacle.carvingTimeToStationary = 0.1f;
                    obstacle.size = collider.size;
                    obstacle.center = collider.center;
                }
            }
        }
        else
        {
            if (boughtRoot.gameObject.activeSelf != IsBought)
            {
                boughtRoot.gameObject.SetActive(IsBought);   
            }
        }
    }

    public override Sprite GetPurchaseIcon()
    {
        return buildingsCollection.GetBuildingData(buildingBase).icon;
    }

    public override string GetTitle()
    {
        return buildingsCollection.GetBuildingData(buildingBase).title;
    }

    
    [Button]
    private void SetupMinMaxes(float min = 3, float max = 9)
    {
        allowedSides = new []
        {
            GetMinMax(0, min, max),
            GetMinMax(1, min, max),
            GetMinMax(2, min, max),
            GetMinMax(3, min, max)
        };
    }
    private MinMaxData GetMinMax(int rand, float minS, float maxS)
    {
        var min = new Vector3(minS, 0, minS);
        var max = new Vector3(maxS, 0, maxS);
        var minMax = new MinMaxData();
        switch (rand)
        {
            case 0: // right
                minMax.min = new Vector3(MinX(false), 0, MinZ(true));
                minMax.max = new Vector3(MaxX(false), 0, MaxZ(false));
                break;
            case 1: // up
                minMax.min = new Vector3(MaxX(true), 0, MinZ(false));
                minMax.max = new Vector3(MinX(false), 0, MaxZ(false));
                break;
            case 2: // left
                minMax.min = new Vector3(MaxX(true), 0, MaxZ(true));
                minMax.max = new Vector3(MinX(true), 0, MinZ(false));
                break;
            case 3: // bottom
                minMax.min = new Vector3(MinX(true), 0, MaxZ(true));
                minMax.max = new Vector3(MaxX(false), 0, MinZ(true));
                break;
            default:
                break;
        }

        float MinX(bool minus)
        {
            return minus ? -min.x : min.x;
        }
        float MaxX(bool minus)
        {
            return minus ? -max.x : max.x;
        }
        float MinZ(bool minus)
        {
            return minus ? -min.z : min.z;
        }
        float MaxZ(bool minus)
        {
            return minus ? -max.z : max.z;
        }

        minMax.min = new Vector3(Mathf.Clamp(minMax.min.x, minMaxX.x, minMaxX.y),
            0, 
            Mathf.Clamp(minMax.min.z, minMaxZ.x, minMaxZ.y));
        minMax.max = new Vector3(Mathf.Clamp(minMax.max.x, minMaxX.x, minMaxX.y),
            0, 
            Mathf.Clamp(minMax.max.z, minMaxZ.x, minMaxZ.y));
        return minMax;
    }

    public MinMaxData[] GetCorners()
    {
        return allowedSides;
    }
}