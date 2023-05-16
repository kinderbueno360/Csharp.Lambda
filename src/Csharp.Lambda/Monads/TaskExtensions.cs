using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp.Lambda.Monads
{
    public static class TaskExtensions
    {
        // Map
        public static Task<TResult> Map<TSource, TResult>(
            this Task<TSource> task,
            Func<TSource, TResult> projection)
        {
            return task.ContinueWith(t => projection(t.Result));
        }

        // Bind
        public static Task<TResult> Bind<TSource, TResult>(
            this Task<TSource> task,
            Func<TSource, Task<TResult>> projection)
        {
            return task.ContinueWith(t => projection(t.Result)).Unwrap();
        }

        // GetOrElse
        public static Task<T> GetOrElse<T>(
            this Task<T> task,
            T defaultValue)
        {
            return task.ContinueWith(t => t.IsFaulted ? defaultValue : t.Result);
        }

        // Recover
        public static Task<T> Recover<T>(
            this Task<T> task,
            Func<Exception, T> recoverFunc)
        {
            return task.ContinueWith(t => t.IsFaulted ? recoverFunc(t.Exception) : t.Result);
        }

        // Select
        public static Task<TResult> Select<TSource, TResult>(
            this Task<TSource> task,
            Func<TSource, TResult> selector)
        {
            return task.Map(selector);
        }

        // SelectMany
        public static Task<TResult> SelectMany<TSource, TResult>(
            this Task<TSource> task,
            Func<TSource, Task<TResult>> selector)
        {
            return task.Bind(selector);
        }

        // Where
        public static Task<TSource> Where<TSource>(
            this Task<TSource> task,
            Func<TSource, bool> predicate)
        {
            return task.ContinueWith(t =>
            {
                if (t.IsFaulted) throw t.Exception;
                return predicate(t.Result) ? t.Result : throw new Exception("Predicate does not hold");
            });
        }


        // Join
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>(
            this Task<TOuter> outer,
            Task<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.ContinueWith(o =>
            {
                return inner.ContinueWith(i =>
                {
                    if (outerKeySelector(o.Result).Equals(innerKeySelector(i.Result)))
                    {
                        return resultSelector(o.Result, i.Result);
                    }
                    throw new Exception("Keys do not match");
                }).Result;
            });
        }
    }
}