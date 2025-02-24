using UnityEngine;
using Zenject;

public class PolicestationBuilding : BuildingBase
{
    [Inject] private PlayerController player;
    [Inject] private WishesCollectionConfig wishesConfig;

    [SerializeField] protected float catchSpeed = 1f;
    [SerializeField] protected float catchRadius = 1.3f;
    [SerializeField] protected ThiefsPool thiefsPool;
   
    protected ThiefBehaviour activeThief;
    protected bool IsSpawned => activeThief != null;
    private float lastSpawnTime = -99999f;
    protected float catchProgress = 0f;

    private void Update()
    {
        if (!IsBought)
        {
            return;
        }

        if (IsSpawned)
        {
            if ((player.transform.position - activeThief.transform.position).sqrMagnitude < catchRadius)
            {
                catchProgress += Time.deltaTime/catchSpeed;
            }
            activeThief.SetCatchProgress(catchProgress);
            if (catchProgress >= 1f)
            {
                SetThiefCatched(player);
            }
        }
        else
        {
            if (Time.time - lastSpawnTime > wishesConfig.thiefSpawnPause)
            {
                lastSpawnTime = Time.time;
                SpawnThief();
            }
        }
    }

    private void SetThiefCatched(PlayerController catcher)
    {
        thiefsPool.Pool(activeThief);
    }

    private void SpawnThief()
    {
        catchProgress = 0f;
        activeThief = thiefsPool.GetElement();
    }
}