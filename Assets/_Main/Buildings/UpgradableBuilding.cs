using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class UpgradableBuilding : UpgradableObject
{
    [Inject] private DiContainer container;
    [Inject] private BuildingsConfig buildingsConfig;
    [Inject] private UpgradablePricesManager _upgradablePricesManager;
    
    [SerializeField] private Transform boughtRoot;
    [SerializeField] private EBuilding type;
    
    private GameObject spawnedGFX;
    private int spawnedLevel = -2;

    public List<Price> GetPricesCopy()
    {
        return _upgradablePricesManager.GetLevelPrices(this).Select(x => x.GetCopy()).ToList();
    }
    
    protected override List<Price> GetPrices()
    {
        return _upgradablePricesManager.GetLevelPrices(this);
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
            var buildingData = buildingsConfig.GetBuildingData(type);
            var level = buildingData.levels[data.level - 1];
            var optionPrefab = level.options[data.optionIndex];
            if (spawnedGFX != null)
            {
                Destroy(spawnedGFX);
            }

            spawnedGFX = container.InstantiatePrefab(optionPrefab, levels[data.level - 1].transform);
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
        else
        {
            if (boughtRoot.gameObject.activeSelf != IsBought)
            {
                boughtRoot.gameObject.SetActive(IsBought);   
            }
        }
    }
}