using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DelftTools.Utils
{
    public class EnumerableConditionalWeakTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey,TValue>> 
        where TKey:class 
        where TValue:class
    {
        private readonly IList<WeakReference> weakKeys = new List<WeakReference>();
        private readonly ConditionalWeakTable<TKey, TValue> innerDictionary = new ConditionalWeakTable<TKey, TValue>();

        public IEnumerable<TValue> Values
        {
            get { return GetKeys().Select(k => this[k]).ToList(); }
        }

        public IEnumerable<TKey> Keys { get { return GetKeys(); } }

        private IEnumerable<TKey> GetKeys()
        {
            var aliveKeys = new List<TKey>();
            foreach (var weakKey in weakKeys.ToList())
            {
                var key = (TKey) weakKey.Target;
                if (key != null)
                    aliveKeys.Add(key);
                else
                    weakKeys.Remove(weakKey); // no longer alive
            }
            return aliveKeys;
        }

        public void Add(TKey key, TValue value)
        {
            weakKeys.Add(new WeakReference(key));
            innerDictionary.Add(key, value);
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                return innerDictionary.TryGetValue(key, out value) ? value : null;
            }
        }

        public void Clear()
        {
            var keys = GetKeys();
            foreach (var key in keys)
                innerDictionary.Remove(key);
            weakKeys.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new DefaultListEnumerator<KeyValuePair<TKey, TValue>>(
                GetKeys()
                    .Select(key => new KeyValuePair<TKey, TValue>(key, this[key]))
                    .ToList());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return innerDictionary.TryGetValue(key, out value);
        }
    }
}