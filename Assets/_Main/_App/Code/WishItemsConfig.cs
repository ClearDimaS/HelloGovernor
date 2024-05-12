using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WishItemsConfig", fileName = "WishItemsConfig")]
public class WishItemsConfig : TypedCollectionConfig<WishItemsConfig.WishItemData, EWish>
{
    [Serializable]
    public class WishItemData : IKey<EWish>
    {
        public EWish type;
        public Sprite icon;
        public float takeTimer;
        
        public EWish Key => type;
    }
    
    public float GetTakeDuration(EWish type)
    {
        return GetItem(type).takeTimer;
    }
    
    public Sprite GetIcon(EWish type)
    {
        return GetItem(type).icon;
    }
}