using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp.Lambda.Monads
{
    public sealed class Maybe<T>
    {
        private readonly T _value;

        private Maybe(T value)
        {
            _value = value;
            HasValue = true;
        }

        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (HasValue)
                    return _value;

                throw new InvalidOperationException();
            }
        }

        public static Maybe<T> Some(T value) => new Maybe<T>(value);

        public static Maybe<T> None { get; } = new Maybe<T>(default(T));

        // Map
        public Maybe<R> Map<R>(Func<T, R> f)
        {
            return HasValue ? Maybe<R>.Some(f(_value)) : Maybe<R>.None;
        }

        // Bind
        public Maybe<R> Bind<R>(Func<T, Maybe<R>> f)
        {
            return HasValue ? f(_value) : Maybe<R>.None;
        }

        // GetOrElse
        public T GetOrElse(T defaultValue)
        {
            return HasValue ? _value : defaultValue;
        }
    }
}
