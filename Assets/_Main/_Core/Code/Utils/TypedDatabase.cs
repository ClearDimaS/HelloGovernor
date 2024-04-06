using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IKey<T>
{
    public T Key { get; }
}
public class TypedDatabase<T, U> : ScriptableObject where U : IKey<T>
{
    [SerializeField] protected List<U> datas;

    private Dictionary<T, U> datasCache = new ();

    public IEnumerable<U> Datas => datas;
    
    public virtual U GetData(T type)
    {
        if (!datasCache.ContainsKey(type))
        {
            datasCache.Add(type, 
                datas.FirstOrDefault(x => x.Key.Equals(type)));
        }

        return datasCache[type];
    }
}
