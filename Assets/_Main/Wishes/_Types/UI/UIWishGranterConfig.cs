using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "Configs/Wishes/UIGranter", fileName = "UIGranterConfig")]
public class UIWishGranterConfig : WishGranterConfig
{
    public LocalizedString tutorialTitleLocalized;
    public string tutorialTitle => tutorialTitleLocalized.GetLocalizedString();
    public Sprite tutorialIcon;
}
