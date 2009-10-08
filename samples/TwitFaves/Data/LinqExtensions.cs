// LinqExtensions.cs
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TwitFaves.Data {

    public class Group<TKey, TElement> : IGrouping<TKey, TElement> {

        private TKey _key;
        private List<TElement> _elements;

        public Group(TElement element, TKey key) {
            _key = key;
            _elements = new List<TElement>() { element };
        }

        public TKey Key {
            get {
                return _key;
            }
        }

        internal void AddElement(TElement element) {
            _elements.Add(element);
        }

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<TElement>)this).GetEnumerator();
        }
        #endregion

        #region IEnumerable<TElement> Members
        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator() {
            return _elements.GetEnumerator();
        }
        #endregion
    }

    public static class LinqExtensions {

        public static IEnumerable<TGroup> GroupByContiguous<TSource, TKey, TGroup>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> keyComparer, Expression<Func<TSource, TKey, TGroup>> groupCreator) where TGroup : Group<TKey, TSource> {
            Func<TSource, TKey> keyFunction = keySelector.Compile();
            Func<TSource, TKey, TGroup> groupFunction = groupCreator.Compile();

            TGroup currentGroup = null;
            foreach (TSource sourceElement in source) {
                TKey key = keyFunction(sourceElement);

                if ((currentGroup != null) && (keyComparer.Equals(currentGroup.Key, key) == false)) {
                    yield return currentGroup;
                    currentGroup = null;
                }

                if (currentGroup == null) {
                    currentGroup = groupFunction(sourceElement, key);
                }
                else {
                    currentGroup.AddElement(sourceElement);
                }
            }

            if (currentGroup != null) {
                yield return currentGroup;
            }
        }
    }
}
