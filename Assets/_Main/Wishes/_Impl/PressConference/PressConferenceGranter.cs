using System.Collections;
using UnityEngine;
using Zenject;

public class PressConferenceGranter : UIWishGranter
{
    [Inject] protected UI_Manager uiManager;
    [Inject] private CompassManager compassManager;
    
    [SerializeField] private PressConferenceConfig conferenceConfig;

    protected override void ShowActivationPlace()
    {
        base.ShowActivationPlace();
        compassManager.AddTarget(activationPlace.transform, ECompasTarget.PressConference);
    }

    protected override void HideActivationPlace()
    {
        base.HideActivationPlace();
        compassManager.RemoveTarget(activationPlace.transform, ECompasTarget.PressConference);
    }
    
    protected override UI_Panel GetPanel()
    {
        return uiManager.GetPanel<PressConferenceUI_Panel>();
    }

    protected override void OnActivate()
    {
        var game = new PressConferenceGame(conferenceConfig, coolDownTimer.GetGameTimer(), GiveReward);
        (panel as PressConferenceUI_Panel).Init(game);
        base.OnActivate();
    }
    
    private void GiveReward(int count)
    {
        AddMoney(count, processPlaces[0].Position + Vector3.up);
    }
}
