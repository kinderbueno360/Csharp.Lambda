using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp.Lambda
{
    public abstract class Validation<T>
    {
        public sealed class Valid : Validation<T>
        {
            public T Value { get; }
            public Valid(T value) => Value = value;
        }

        public sealed class Invalid : Validation<T>
        {
            public string[] Errors { get; }
            public Invalid(params string[] errors) => Errors = errors;
        }

        public bool IsValid => this is Valid;
        public bool IsInvalid => !IsValid;

        // Map
        public Validation<U> Map<U>(Func<T, U> map)
        {
            return this switch
            {
                Valid valid => new Validation<U>.Valid(map(valid.Value)),
                Invalid invalid => new Validation<U>.Invalid(invalid.Errors),
                _ => throw new NotImplementedException()
            };
        }

        // Bind
        public Validation<U> Bind<U>(Func<T, Validation<U>> bind)
        {
            return this switch
            {
                Valid valid => bind(valid.Value),
                Invalid invalid => new Validation<U>.Invalid(invalid.Errors),
                _ => throw new NotImplementedException()
            };
        }

        // Match
        public U Match<U>(Func<T, U> onValid, Func<string[], U> onInvalid)
        {
            return this switch
            {
                Valid valid => onValid(valid.Value),
                Invalid invalid => onInvalid(invalid.Errors),
                _ => throw new NotImplementedException()
            };
        }
    }

    public static class ValidationExtensions
    {
        public static Validation<V> SelectMany<T, U, V>(
            this Validation<T> validation,
            Func<T, Validation<U>> bind,
            Func<T, U, V> project)
        {
            return validation switch
            {
                Validation<T>.Valid valid =>
                    bind(valid.Value) switch
                    {
                        Validation<U>.Valid u => new Validation<V>.Valid(project(valid.Value, u.Value)),
                        Validation<U>.Invalid invalid => new Validation<V>.Invalid(invalid.Errors),
                        _ => throw new NotImplementedException()
                    },
                Validation<T>.Invalid invalid => new Validation<V>.Invalid(invalid.Errors),
                _ => throw new NotImplementedException()
            };
        }
    }
}
