using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DelftTools.Utils
{
    /// <summary>
    /// Use this dictionary to associate data with other data, without preventing either the key or 
    /// the value to be collected by the garbage collector. Useful to build cached lookups where the
    /// lifetime of the objects cannot be (reliably) tracked and the cache itself should never force
    /// objects to stay in memory.
    /// 
    /// If either the key or value is collected by the garbage collector, the entry will automatically
    /// disappear from the dictionary.
    /// 
    /// NOTE: if the value is referenced nowhere else (as is typical in caching), this dictionary won't 
    /// work for you: use ConditionalWeakTable.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class WeakDictionary<TKey, TValue>
        where TKey : class
        where TValue : class
    {
	    // keeps our keys weak, we keep our values weak ourselves
        private readonly ConditionalWeakTable<TKey, WeakReference> innerDictionary = new ConditionalWeakTable<TKey, WeakReference>();

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("Key already exists: " + key);
            innerDictionary.Add(key, new WeakReference(value));
        }

        public TValue this[TKey key]
        {
            get
            {
                WeakReference weakRef;
                if (innerDictionary.TryGetValue(key, out weakRef))
                {
                    var entry = (TValue) weakRef.Target;
                    if (entry != null)
                        return entry;
                    innerDictionary.Remove(key); // entry no longer alive: cleanup
                }
                return null;
            }
            set
            {
                if (ContainsKey(key))
                {
                    innerDictionary.Remove(key);
                }
                innerDictionary.Add(key, new WeakReference(value));
            }
        }

        public void Remove(TKey key)
        {
            innerDictionary.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            WeakReference weakRef;
            if (innerDictionary.TryGetValue(key, out weakRef))
            {
                if (weakRef.IsAlive)
                    return true;
                innerDictionary.Remove(key); // entry no longer alive: cleanup
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = null;
            return false;
        }
    }
}
