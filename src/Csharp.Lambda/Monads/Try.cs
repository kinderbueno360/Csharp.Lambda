namespace Csharp.Lambda
{
    public abstract class Try<T>
    {
        public abstract TResult Match<TResult>(Func<T, TResult> onSuccess, Func<System.Exception, TResult> onException);

        public sealed class Success : Try<T>
        {
            private readonly T _value;
            public Success(T value) => _value = value;
            public override TResult Match<TResult>(Func<T, TResult> onSuccess, Func<System.Exception, TResult> onException) => onSuccess(_value);
        }

        public sealed class Exception : Try<T>
        {
            private readonly System.Exception _exception;
            public Exception(System.Exception exception) => _exception = exception;
            public override TResult Match<TResult>(Func<T, TResult> onSuccess, Func<System.Exception, TResult> onException) => onException(_exception);
        }

        public static Try<T> Of(Func<T> f)
        {
            try
            {
                return new Success(f());
            }
            catch (System.Exception ex)
            {
                return new Exception(ex);
            }
        }

        // Map
        public Try<TResult> Map<TResult>(Func<T, TResult> f)
            => Match(
                onSuccess: t => Try<TResult>.Of(() => f(t)),
                onException: ex => new Try<TResult>.Exception(ex)
            );

        // Bind
        public Try<TResult> Bind<TResult>(Func<T, Try<TResult>> f)
            => Match(
                onSuccess: t => f(t),
                onException: ex => new Try<TResult>.Exception(ex)
            );

        // GetOrElse
        public T GetOrElse(T defaultValue)
            => Match(
                onSuccess: t => t,
                onException: _ => defaultValue
            );
    }
}