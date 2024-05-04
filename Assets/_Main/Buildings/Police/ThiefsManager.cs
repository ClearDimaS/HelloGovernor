using UnityEngine;
using Zenject;

public class ThiefsManager : MonoBehaviour
{
    [Inject] protected ThiefsPool thiefsPool;
    [Inject] protected GameConfig config;

    [SerializeField] protected ThiefSpawnData[] thiefSpawns;
    protected float lastStealTime;
    protected bool isStealing;
    public bool IsStealing => isStealing;
    protected ThiefBehaviour activeThief;

    private void Update()
    {
        if (isStealing)
        {
            return;
        }
        if (Time.time - lastStealTime > config.thiefPause)
        {
            activeThief = thiefsPool.GetElement();
            var spawnData = thiefSpawns[Random.Range(0, thiefSpawns.Length)];
            activeThief.transform.position = spawnData.spawn.position;
            activeThief.Init(spawnData.escape);
            isStealing = true;
        }
    }

    public void FinishSteal(ThiefBehaviour thief)
    {
        lastStealTime = Time.time;
        isStealing = false;
        thiefsPool.Pool(thief);
        activeThief = null;
    }

    public ThiefBehaviour GetStealer()
    {
        return activeThief;
    }
}