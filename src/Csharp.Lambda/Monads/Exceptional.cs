using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp.Lambda.Monads
{
    public struct Exceptional<T>
    {
        private readonly Exception? _exception;
        private readonly T _value;

        public bool IsSuccess { get; }
        public bool IsException => !IsSuccess;

        public Exceptional(Exception exception)
        {
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));
            _value = default!;
            IsSuccess = false;
        }

        public Exceptional(T value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _exception = null;
            IsSuccess = true;
        }

        public static implicit operator Exceptional<T>(Exception ex) => new Exceptional<T>(ex);
        public static implicit operator Exceptional<T>(T value) => new Exceptional<T>(value);

        public Exceptional<TResult> Apply<TResult>(Exceptional<Func<T, TResult>> exceptionalFunc)
        {
            return exceptionalFunc.IsSuccess ? Map(exceptionalFunc._value!) : new Exceptional<TResult>(exceptionalFunc._exception!);
        }

        public Exceptional<TResult> Map<TResult>(Func<T, TResult> map)
        {
            return IsException ? new Exceptional<TResult>(_exception!) : new Exceptional<TResult>(map(_value));
        }

        public Exceptional<TResult> Bind<TResult>(Func<T, Exceptional<TResult>> bind)
        {
            return IsException ? new Exceptional<TResult>(_exception!) : bind(_value);
        }
        public TResult Match<TResult>(Func<Exception, TResult> onException, Func<T, TResult> onSuccess)
            => IsException ? onException(_exception!) : onSuccess(_value);
    }
}