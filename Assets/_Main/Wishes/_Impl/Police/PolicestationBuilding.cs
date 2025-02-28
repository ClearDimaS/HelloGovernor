using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PolicestationBuilding : BuildingBase, ICooldownable
{
    [Inject] private PlayerController player;
    [Inject] private WishesCollectionConfig wishesConfig;
    [Inject] protected ThiefsPool thiefsPool;

    [SerializeField] protected Transform spawnPlace;
    [SerializeField] protected float catchSpeed = 1f;
    [SerializeField] protected float catchRadius = 1.3f;

    protected ThiefBehaviour activeThief;
    protected bool IsSpawned => activeThief != null;
    private float lastSpawnTime = 0;
    protected float catchProgress = 0f;
    private List<Transform> extraAssistants = new ();

    public float CoolDown => wishesConfig.thiefSpawnPause;
    public float CoolDownTimeLeft { get; protected set; }
    public bool IsCooldown => CoolDownTimeLeft > 0;

    private void Update()
    {
        if (!IsBought)
        {
            return;
        }

        if (IsSpawned)
        {
            if ((player.transform.position - activeThief.transform.position).sqrMagnitude < catchRadius * catchRadius)
            {
                catchProgress += Time.deltaTime/catchSpeed;
            }
            else
            {
                foreach (var assistant in extraAssistants)
                {
                    if ((assistant.transform.position - activeThief.transform.position).sqrMagnitude < catchRadius)
                    {
                        catchProgress += Time.deltaTime/catchSpeed;
                    }
                }
            }
            activeThief.SetCatchProgress(catchProgress);
            if (catchProgress >= 1f)
            {
                SetThiefCatched(player);
            }

            if (GetTimeLeft() <= 0 && activeThief != null)
            {
                thiefsPool.Pool(activeThief);
                activeThief = null;
            }
        }
        else
        {
            var timeWaiting = Time.time - lastSpawnTime;
            CoolDownTimeLeft  = wishesConfig.thiefSpawnPause - timeWaiting;
            
            if (timeWaiting > wishesConfig.thiefSpawnPause)
            {
                lastSpawnTime = Time.time;
                SpawnThief();
            }
        }
    }

    public void AddAssistant(Transform extra)
    {
        extraAssistants.Add(extra);
    }

    private void SetThiefCatched(PlayerController catcher)
    {
        catcher.ReturnMoney(wishesConfig.thiefReward, activeThief.transform);
        thiefsPool.Pool(activeThief);
    }

    private void SpawnThief()
    {
        catchProgress = 0f;
        activeThief = thiefsPool.GetElement();
        activeThief.transform.position = spawnPlace.position;
    }

    public bool HasThief()
    {
        return activeThief != null;
    }

    public int GetTimeLeft()
    {
        var timeSinceSpawn = Mathf.RoundToInt(Time.time - lastSpawnTime);
        return wishesConfig.thiefLifeTime - timeSinceSpawn;
    }
}