using System.Collections;
using UnityEngine;
using Zenject;

public class PressConferenceGranter : UIWishGranter
{
    [SerializeField] private PressConferenceConfig conferenceConfig;
    [Inject] protected UI_Manager uiManager;
    
    protected override UI_Panel GetPanel()
    {
        return uiManager.GetPanel<PressConferenceUI_Panel>();
    }

    protected override void OnActivate()
    {
        var game = new PressConferenceGame(conferenceConfig);
        (panel as PressConferenceUI_Panel).Init(game);
        base.OnActivate();
    }
}
