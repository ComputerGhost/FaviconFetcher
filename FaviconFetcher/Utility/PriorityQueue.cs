using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaviconFetcher.Utility
{
    // A quick and dirty priority queue class.
    class PriorityQueue<TKey, TValue> : IEnumerable<TValue> where TKey : IComparable
    {
        public int Count { get { return _Data.Count; } }

        public void Add(TKey priority, TValue value)
        {
            _Data.Add(priority, value);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _Data.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Data.Values.GetEnumerator();
        }


        // This should keep the dictionary from complaining about duplicate keys.
        class KeyComparer : Comparer<TKey>
        {
            public override int Compare(TKey x, TKey y)
            {
                if (x.CompareTo(y) < 0)
                    return -1;
                return 1;
            }
        }

        private SortedDictionary<TKey, TValue> _Data = new SortedDictionary<TKey, TValue>(new KeyComparer());
    }
}
