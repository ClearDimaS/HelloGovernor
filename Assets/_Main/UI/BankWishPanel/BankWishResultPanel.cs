using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BankWishResultPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button claimButton;

    private BankGame game;
    private Action onClaim;
    
    private void Awake()
    {
        claimButton.onClick.AddListener(ClaimReward);
    }

    private void ClaimReward()
    {
        var fire = onClaim;
        fire?.Invoke();
        onClaim = null;
        gameObject.SetActive(false);
        game.Complete();
    }

    public void Show(BankGame game, Action onClaim)
    {
        this.game = game;
        this.onClaim = onClaim;
        var reward = game.GetReward();
        reward = Mathf.Max(0, reward);
        var symbol = reward > 0 ? "+" : "";
        rewardText.text = $"{symbol}{Price.ToMoneyString(reward)}";
        gameObject.SetActive(true);
    }
}