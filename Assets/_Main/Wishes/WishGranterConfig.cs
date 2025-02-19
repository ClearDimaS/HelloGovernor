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
                type = granter.GetType();
            }

            return type;
        }
    }

    public WishGranterTimer GetTimer(WishGranter granterInstance)
    {
        switch (timerType)
        {
            case ETimer.None:
                return new NullWishGranterTimer(granterInstance);
            case ETimer.Cooldown:
                return new WishGranterCooldownTimer(cooldown:cooldown, granterInstance);
            case ETimer.Game:
                return new WishGranterGameTimer(cooldown:cooldown, duration:gameDuration, granterInstance);
            default:
                throw new NotImplementedException($"");
        }
    }
}

public class NullWishGranterTimer : WishGranterTimer
{
    public NullWishGranterTimer(WishGranter granter) : base(granter)
    {
    }

    public override bool IsCooldown => false;
    public override float CoolDownTime => -1;
    public override float CoolDownTimeLeft => -1;
    public override float GameTimeLeft => -1;
    public override bool CanCooldown => false;

    public override bool CanAddMore()
    {
        return true;
    }

    public override void OnUpdate()
    {

    }
}

public abstract class WishGranterTimer
{
    public abstract bool IsCooldown { get;  }
    public abstract float CoolDownTime { get; }
    public abstract float CoolDownTimeLeft { get; }
    public abstract float GameTimeLeft { get; }
    public abstract bool CanCooldown { get;  }

    protected WishGranter granter;
    
    public WishGranterTimer(WishGranter granter)
    {
        this.granter = granter;
    }

    public abstract bool CanAddMore();
    
    public abstract void OnUpdate();
}

public class WishGranterCooldownTimer : WishGranterTimer
{
    public override float GameTimeLeft => -1;
    public override bool CanCooldown => true;
    public override float CoolDownTimeLeft => (CoolDownTime - (Time.time - CoolDownStartTime));
    public override bool IsCooldown => isCoolDown;
    public override float CoolDownTime => coolDownTime;
    
    public float CoolDownStartTime { get; protected set; }

    protected float coolDownTime;
    protected bool isCoolDown;
    public WishGranterCooldownTimer(float cooldown, WishGranter granter) : base(granter)
    {
        coolDownTime = cooldown;
        CoolDownStartTime = Time.time;
    }

    public override void OnUpdate()
    {
        if (IsCooldown && Time.time - CoolDownStartTime > CoolDownTime)
        {
            isCoolDown = false;
        }

        if (!IsCooldown && granter.HasAnyone())
        {
            Restart();
        }
    }

    public override bool CanAddMore()
    {
        return !IsCooldown;
    }
    
    private void Restart()
    {
        CoolDownStartTime = Time.time;
        isCoolDown = true;
    }
}

public class WishGranterGameTimer : WishGranterTimer
{
    protected enum GameState
    {
        Cooldown,
        WaitingStart,
        Started,
    }

    protected float cooldown;
    protected float duration;
    protected GameState gameState;
    protected int startProcessedCounter;
    protected float CoolDownStartTime { get; private set; }
    protected float GameStartTime { get; private set; }
    
    public override bool IsCooldown => gameState == GameState.Cooldown;
    public override float CoolDownTime => cooldown;
    public override float CoolDownTimeLeft => IsCooldown ? CoolDownTime - (Time.time - CoolDownStartTime): 0f;
    public override float GameTimeLeft => gameState == GameState.Started ? duration - (Time.time - GameStartTime): 0f;
    public override bool CanCooldown => true;

    public WishGranterGameTimer(float cooldown, float duration, WishGranter granter) : base(granter)
    {
        this.cooldown = cooldown;
        this.duration = duration;
        gameState = GameState.Cooldown;
        startProcessedCounter = granter.ProcessedCounter;
        CoolDownStartTime = Time.time;
    }
    
    public override bool CanAddMore()
    {
        return gameState == GameState.WaitingStart || gameState == GameState.Started;
    }

    public override void OnUpdate()
    {
        if (gameState == GameState.Cooldown)
        {
            if (CoolDownTimeLeft < 0f)
            {
                gameState = GameState.WaitingStart;
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
                gameState = GameState.Cooldown;
            }
        }
    }
}
