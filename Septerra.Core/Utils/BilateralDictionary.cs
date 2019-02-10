using System;
using System.Collections;
using System.Collections.Generic;

namespace Septerra.Core.DB
{
    public class BilateralDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private readonly Dictionary<TKey, TValue> _fwdDictionary;
        private readonly Dictionary<TValue, TKey> _revDictionary;
        private readonly Boolean _allowDublicateValues;

        public BilateralDictionary(Boolean allowDublicateValues = false, Int32 capacity = 0)
            : this(null, null, capacity)
        {
            _allowDublicateValues = allowDublicateValues;
        }

        public BilateralDictionary(IEqualityComparer<TKey> keyComparer, Int32 capacity = 0)
            : this(keyComparer, null, capacity)
        {
        }

        public BilateralDictionary(IEqualityComparer<TValue> valueComparer, Int32 capacity = 0)
            : this(null, valueComparer, capacity)
        {
        }

        public BilateralDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, Int32 capacity = 0)
        {
            _fwdDictionary = new Dictionary<TKey, TValue>(capacity, keyComparer);
            _revDictionary = new Dictionary<TValue, TKey>(capacity, valueComparer);
        }

        public void Add(TKey key, TValue value)
        {
            try
            {
                _fwdDictionary.Add(key, value);
            }
            catch (ArgumentException)
            {
                Log.Error($"Forward: {key}, {value}, ({_fwdDictionary[key]})");
                throw;
            }

            if (_allowDublicateValues)
            {
                if (!_revDictionary.ContainsKey(value))
                    _revDictionary.Add(value, key);
            }
            else
            {
                _revDictionary.Add(value, key);
            }
        }

        public void RemoveByKey(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
            {
                _fwdDictionary.Remove(key);
                _revDictionary.Remove(value);
            }
        }

        public void RemoveByValue(TValue value)
        {
            TKey key;
            if (TryGetKey(value, out key))
            {
                _fwdDictionary.Remove(key);
                _revDictionary.Remove(value);
            }
        }

        public Boolean TryGetValue(TKey key, out TValue value)
        {
            return _fwdDictionary.TryGetValue(key, out value);
        }

        public Boolean TryGetKey(TValue value, out TKey key)
        {
            return _revDictionary.TryGetValue(value, out key);
        }

        public TValue GetValue(TKey key)
        {
            return _fwdDictionary[key];
        }

        public TKey GetKey(TValue value)
        {
            return _revDictionary[value];
        }

        public Boolean ContainsValue(TValue value)
        {
            return _revDictionary.ContainsKey(value);
        }

        public Boolean ContainsKey(TKey key)
        {
            return _fwdDictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _fwdDictionary.Clear();
            _revDictionary.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _fwdDictionary.GetEnumerator();
        }
    }
}