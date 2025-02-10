using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Wishes/UIGranter", fileName = "UIGranterConfig")]
public class UIWishGranterConfig : WishGranterConfig
{
    public UIWishProcessor wishProcessorPanel;
}

public abstract class UIWishProcessor : MonoBehaviour
{
    
}