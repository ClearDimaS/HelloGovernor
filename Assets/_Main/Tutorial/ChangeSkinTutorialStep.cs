using UnityEngine;

public class ChangeSkinTutorialStep : TutorialStep
{
    protected SkinChooser skinChooser;
    protected Transform activationPlace;

    public ChangeSkinTutorialStep(SkinChooser skinChooser, Transform activationPlace, PlayerDataRepository repository) : base(repository)
    {
        this.skinChooser = skinChooser;
        this.activationPlace = activationPlace;
    }

    public override void Start()
    {
        base.Start();
        Debug.Log($"starting skin tutorial!");
        if (!IsCompleted() && playerRepository.Money < 200)
        {
            playerRepository.Money += 200;
        }
    }

    protected override string CreateKey()
    {
        return $"skin_tutorial";
    }

    protected override void UpdateProgress_Internal()
    {
        
    }

    public override float GetProgress()
    {
        if (skinChooser.WasOpen)
        {
            return 1f;
        }

        return 0f;
    }

    public override Transform GetCameraTarget()
    {
        return activationPlace;
    }

    public override Transform GetArrowTarget()
    {
        return activationPlace;
    }

    private static string notBoughtProgressText = "0/1";
    private static string boughtProgressText = "1/1";
    
    protected override string CreateProgressText()
    {
        if (skinChooser.WasOpen)
        {
            return boughtProgressText;
        }

        return notBoughtProgressText;
    }

    protected override string CreateTitle()
    {
        return $"Visit fashion";   
    }

    public override Sprite GetTutorialIcon()
    {
        return skinChooser.GetTutorialIcon();
    }

    public void ForcePurchase()
    {
        skinChooser.WasOpen = true;
    }
}