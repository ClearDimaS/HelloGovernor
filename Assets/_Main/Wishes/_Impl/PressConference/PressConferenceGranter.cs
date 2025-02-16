using System.Collections;
using Zenject;

public class PressConferenceGranter : UIWishGranter
{
    [Inject] protected UI_Manager uiManager;
    
    protected override UI_Panel GetPanel()
    {
        return uiManager.GetPanel<PressConferenceUI_Panel>();
    }
}
