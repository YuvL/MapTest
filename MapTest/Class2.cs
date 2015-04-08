using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTest
{

    public static class LinqExtensions
    {
        public static IEnumerable<TSource> SelectManyFromTree<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> collectionSelector) where TSource : class
        {
            if (source == null) throw new ArgumentNullException("source");
            if (collectionSelector == null) throw new ArgumentNullException("collectionSelector");
            return LinqExtensionsHelper.SelectManyFromTreeIterator(source, collectionSelector);
        }

        public static TSource FirstOrDefaultFromTree<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, IEnumerable<TSource>> childSourceSelector) where TSource : class
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (childSourceSelector == null) throw new ArgumentNullException("childSourceSelector");
            return LinqExtensionsHelper.FirstOrDefaultFromTreeIterator(source, predicate, childSourceSelector);
        }

        public static IEnumerable<TSource> ExceptBy<TSource, TKey>(this IEnumerable<TSource> sourceFirst, IEnumerable<TSource> sourceSecond, Func<TSource, TKey> key)
        {
            if (sourceFirst == null) throw new ArgumentNullException("sourceFirst");
            if (sourceSecond == null) throw new ArgumentNullException("sourceSecond");
            if (key == null) throw new ArgumentNullException("key");
            return LinqExtensionsHelper.ExceptByIterator(sourceFirst, sourceSecond, key);
        }

        public static IEnumerable<TSource> IntersectBy<TSource, TKey>(this IEnumerable<TSource> sourceFirst, IEnumerable<TSource> sourceSecond, Func<TSource, TKey> key)
        {
            if (sourceFirst == null) throw new ArgumentNullException("sourceFirst");
            if (sourceSecond == null) throw new ArgumentNullException("sourceSecond");
            if (key == null) throw new ArgumentNullException("key");

            return LinqExtensionsHelper.IntersectByIterator(sourceFirst, sourceSecond, key);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> key)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (key == null) throw new ArgumentNullException("key");

            return LinqExtensionsHelper.DistinctByIterator(source, key);
        }
    }

    public static class LinqExtensionsHelper
    {
        internal static IEnumerable<TSource> IntersectByIterator<TSource, TKey>(IEnumerable<TSource> sourceFirst, IEnumerable<TSource> sourceSecond, Func<TSource, TKey> key)
        {
            var secondSourceKeys = new HashSet<TKey>(sourceSecond.Select(key));
            foreach (TSource item in sourceFirst)
                if (secondSourceKeys.Contains(key(item)))
                    yield return item;
        }

        internal static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> key)
        {
            var keys = new HashSet<TKey>();
            foreach (TSource item in source.Where(item => !keys.Contains(key(item))))
            {
                keys.Add(key(item));
                yield return item;
            }
        }

        internal static IEnumerable<TSource> ExceptByIterator<TSource, TKey>(IEnumerable<TSource> sourceFirst, IEnumerable<TSource> sourceSecond, Func<TSource, TKey> key)
        {
            var secondSourceKeys = new HashSet<TKey>(sourceSecond.Select(key));
            foreach (TSource item in sourceFirst)
                if (!secondSourceKeys.Contains(key(item)))
                    yield return item;
        }

        internal static TSource FirstOrDefaultFromTreeIterator<TSource>(IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, IEnumerable<TSource>> childSourceSelector) where TSource : class
        {
            foreach (var item in source)
                if (predicate(item))
                    return item;

            foreach (var item in source)
            {
                var childSource = childSourceSelector(item);
                if (childSource != null)
                {
                    var chItem = childSource.FirstOrDefaultFromTree(predicate, childSourceSelector);
                    if (chItem != null)
                        return chItem;
                }
            }

            return default(TSource);
        }

        internal static IEnumerable<TSource> SelectManyFromTreeIterator<TSource>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> collectionSelector) where TSource : class
        {
            foreach (var item in source)
                yield return item;

            foreach (var item in source)
            {
                var childSource = collectionSelector(item);
                if (childSource != null)
                    foreach (var item1 in childSource.SelectManyFromTree(collectionSelector))
                        yield return item1;
            }
        }
    }
}
