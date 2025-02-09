using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/WishItemsConfig", fileName = "WishItemsConfig")]
public class WishItemsConfig : TypedCollectionConfig<WishItemsConfig.WishItemData, Type>
{
    [Serializable]
    public class WishItemData : IKey<Type>
    {
        private Type type;
        public WishGranter wishGranter;
        public Sprite icon;
        public float takeTimer;

        public Type Key
        {
            get
            {
                if (type == null)
                {
                    type = wishGranter.GetType();
                }

                return type;
            }
        } 
    }
    
    public float GetTakeDuration(WishGranter type)
    {
        return GetItem(type.GetType()).takeTimer;
    }
    
    public Sprite GetIcon(WishGranter type)
    {
        return GetItem(type.GetType()).icon;
    }
}