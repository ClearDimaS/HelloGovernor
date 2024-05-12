using System.Collections.Generic;
using UnityEngine;

public class TypedCollectionConfig<T, U> : ScriptableObject where T : IKey<U>
{
    [SerializeField] protected List<T> collection;

    private Dictionary<U, T> dict = new ();
    
    protected T GetItem(U key)
    {
        if (!dict.ContainsKey(key))
        {
            foreach (var item in collection)
            {
                if (item.Key.Equals(key))
                {
                    dict[key] = item;
                }
            }
        }

        return dict[key];
    }
}