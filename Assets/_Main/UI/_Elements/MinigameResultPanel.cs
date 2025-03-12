using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Minigame
{
    public abstract int GetReward();

    public abstract void Complete();
}
public class MinigameResultPanel : MonoBehaviour
{
    [SerializeField] private UI_ElementAnimator animator;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button claimButton;

    private Minigame game;
    private Action onClaim;
    
    private void Awake()
    {
        claimButton.onClick.AddListener(ClaimReward);
    }

    private void ClaimReward()
    {
        if (game == null)
        {
            return;
        }
        var fire = onClaim;
        fire?.Invoke();
        onClaim = null;
        gameObject.SetActive(false);
        game.Complete();
        game = null;
        Hide(false);
    }

    public void Show(Minigame game, Action onClaim)
    {
        this.game = game;
        this.onClaim = onClaim;
        var reward = game.GetReward();
        reward = Mathf.Max(0, reward);
        var symbol = reward > 0 ? "+" : "";
        rewardText.text = $"{symbol}{Price.ToMoneyString(reward)}";
        gameObject.SetActive(true);
        animator.Animate();
    }

    public void Hide(bool immediate)
    {
        animator.AnimateBack(instant:immediate);
    }
}