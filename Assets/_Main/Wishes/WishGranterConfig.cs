using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ETimer
{
    None,
    Cooldown,
    Game,
}
//[CreateAssetMenu(menuName = "Configs/Wishes/WishGranter", fileName = "WishConfig")]
public abstract class WishGranterConfig : ScriptableObject, IKey<Type>
{
    public string processAnimation;
    public WishGranter granter;
    public float grantDuration;
    public int reward;
    public float chanceWeight;
    public ETimer timerType;
    [SerializeField, ShowIf(nameof(showCoolDown))] protected float cooldown;
    [SerializeField, ShowIf(nameof(showGame))] protected float gameDuration;
    protected bool showCoolDown => timerType != ETimer.None;
    protected bool showGame => timerType == ETimer.Game;
    
    private Type type;

    public Type Key
    {
        get
        {
            if (type == null)
            {
                if (granter == null)
                {
                    Debug.LogError($"granter null at: {name}");
                }
                type = granter.GetType();
            }

            return type;
        }
    }

    public WishGranterCooldownTimer GetCooldownTimer(WishGranter granterInstance)
    {
        if (timerType == ETimer.None)
        {
            return null;
        }
        return new WishGranterCooldownTimer(cooldown:cooldown, granterInstance, GetGameTimer(granter));
    }
    
    private WishGranterGameTimer GetGameTimer(WishGranter granter)
    {
        if (timerType != ETimer.Game)
        {
            return null;
        }
        return new WishGranterGameTimer(cooldown:cooldown, duration:gameDuration, granter);
    }
}

[Serializable]
public class WishGranterCooldownTimer 
{
    public float CoolDownTimeLeft => gameTimer != null ? gameTimer.CoolDownTimeLeft : (CoolDownTime - (Time.time - CoolDownStartTime));
    public bool IsCooldown =>gameTimer != null ? gameTimer.IsCooldown : isCoolDown;
    public float CoolDownTime => gameTimer != null ? gameTimer.CoolDownTime : coolDownTime;
    protected int processedCount;
    
    public float CoolDownStartTime { get; protected set; }

    protected WishGranter granter;
    protected float coolDownTime;
    protected bool isCoolDown;
    protected WishGranterGameTimer gameTimer;
    public WishGranterCooldownTimer(float cooldown, WishGranter granter, WishGranterGameTimer gameTimer)
    {
        this.gameTimer = gameTimer;
        coolDownTime = cooldown;
        CoolDownStartTime = Time.time;
        this.granter = granter;
    }

    public void OnUpdate()
    {
        if (gameTimer != null)
        {
            gameTimer.OnUpdate();
            return;
        }
        if (IsCooldown && Time.time - CoolDownStartTime > CoolDownTime)
        {
            isCoolDown = false;
            processedCount = granter.ProcessedCounter;
        }
        
        if (!IsCooldown && processedCount != granter.ProcessedCounter)
        {
            Restart();
        }
    }

    public void SetGameReady()
    {
        if (gameTimer != null)
        {
            gameTimer.SetGameReady();
            return;
        }
        isCoolDown = false;
        CoolDownStartTime = Time.time - (CoolDownTime + 1);
        processedCount = granter.ProcessedCounter;
    }

    public bool CanAddMore()
    {
        if (gameTimer != null)
        {
            return gameTimer.CanAddMore();
        }
        return !IsCooldown;
    }
    
    private void Restart()
    {
        CoolDownStartTime = Time.time;
        isCoolDown = true;
    }

    public WishGranterGameTimer GetGameTimer()
    {
        return gameTimer;
    }
}

[Serializable]
public class WishGranterGameTimer
{
    protected enum GameState
    {
        Cooldown,
        WaitingStart,
        Started,
        Over,
    }

    protected float cooldown;
    protected float duration;
    protected GameState gameState;
    protected int startProcessedCounter;
    protected float CoolDownStartTime { get; private set; }
    protected float GameStartTime { get; private set; }
    
    public bool IsCooldown => gameState == GameState.Cooldown;
    public float CoolDownTime => cooldown;
    public float CoolDownTimeLeft => IsCooldown ? CoolDownTime - (Time.time - CoolDownStartTime): 0f;
    public float GameTimeLeft => gameState == GameState.Started ? duration - (Time.time - GameStartTime): 0f;
    protected WishGranter granter;

    public WishGranterGameTimer(float cooldown, float duration, WishGranter granter)
    {
        this.cooldown = cooldown;
        this.duration = duration;
        gameState = GameState.Cooldown;
        CoolDownStartTime = Time.time;
        this.granter = granter;
    }
    
    public bool CanAddMore()
    {
        return gameState == GameState.WaitingStart || gameState == GameState.Started;
    }

    public void OnUpdate()
    {
        if (gameState == GameState.Cooldown)
        {
            if (CoolDownTimeLeft < 0f)
            {
                SetGameReady();
            }
            else
            {
                startProcessedCounter = granter.ProcessedCounter;
            }
        }
        if (gameState == GameState.WaitingStart)
        {
            if (granter.ProcessedCounter != startProcessedCounter)
            {
                gameState = GameState.Started;
                GameStartTime = Time.time;
            }
        }
        else if (gameState == GameState.Started)
        {
            if (GameTimeLeft < 0)
            {
                CoolDownStartTime = Time.time;
                gameState = GameState.Over;
            }
        }
    }

    public void SetGameReady()
    {
        gameState = GameState.WaitingStart;
        startProcessedCounter = granter.ProcessedCounter;
        CoolDownStartTime = Time.time - (CoolDownTime + 1);
    }

    public void ResetToCooldown()
    {
        gameState = GameState.Cooldown;
        CoolDownStartTime = Time.time;
    }

    public bool IsOver()
    {
        return gameState == GameState.Over;
    }
}
