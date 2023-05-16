using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp.Lambda.Monads
{
    public abstract class Either<L, R>
    {
        public abstract T Match<T>(Func<L, T> Left, Func<R, T> Right);

        public sealed class Left : Either<L, R>
        {
            private readonly L value;
            public Left(L value) => this.value = value;
            public override T Match<T>(Func<L, T> Left, Func<R, T> Right) => Left(value);
        }

        public sealed class Right : Either<L, R>
        {
            private readonly R value;
            public Right(R value) => this.value = value;
            public override T Match<T>(Func<L, T> Left, Func<R, T> Right) => Right(value);
        }

        // Map
        public Either<L, RR> Map<RR>(Func<R, RR> f)
            => Match<Either<L, RR>>(
                Left: l => new Either<L, RR>.Left(l),
                Right: r => new Either<L, RR>.Right(f(r))
            );

        // Bind
        public Either<L, RR> Bind<RR>(Func<R, Either<L, RR>> f)
            => Match(
                Left: l => new Either<L, RR>.Left(l),
                Right: r => f(r)
            );
    }
}
