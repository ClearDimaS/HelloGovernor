using DG.Tweening;
using UnityEngine;
using Zenject;

public class UpgradableBuilding : UpgradableObject
{
    [Inject] private DiContainer container;
    [Inject] private BuildingsConfig buildingsConfig;
    
    [SerializeField] private EBuilding type;

    private GameObject spawnedGFX;
    private int spawnedLevel = -2;
    
    protected override void RefreshLevelGFX(bool instant)
    {
        base.RefreshLevelGFX(instant);
        if (spawnedLevel != data.level && data.level > 0)
        {
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
        }
    }
}