using System;

namespace VT.Monads
{
    // 1) The core interface
    public interface IMonad<T>
    {
        // Bind (a.k.a. flatMap)
        IMonad<U> Bind<U>(Func<T, IMonad<U>> binder);
    }

    // 2) Maybe<T> (Option) — represents a value that might be missing
    public readonly struct Maybe<T> : IMonad<T>
    {
        private readonly T value;
        public bool HasValue { get; }
        public T Value => HasValue ? value : throw new InvalidOperationException("No value present");

        private Maybe(T value)
        {
            this.value = value;
            HasValue = true;
        }

        // Return / pure
        public static Maybe<T> Return(T value) => new(value);

        // Nothing
        public static readonly Maybe<T> None = new();

        public IMonad<U> Bind<U>(Func<T, IMonad<U>> binder) => HasValue ? binder(value) : Maybe<U>.None;

        // Support LINQ’s Select
        public Maybe<U> Select<U>(Func<T, U> selector) => HasValue ? Maybe<U>.Return(selector(value)) : Maybe<U>.None;

        // Support LINQ’s SelectMany
        public Maybe<V> SelectMany<U, V>(Func<T, IMonad<U>> binder, Func<T, U, V> projector)
        {
            if (!HasValue)
                return Maybe<V>.None;

            var uCase = binder(value);
            if (!(uCase is Maybe<U> mu) || !mu.HasValue)
                return Maybe<V>.None;

            return Maybe<V>.Return(projector(value, mu.Value));
        }


        public override string ToString() => HasValue ? $"Just({value})" : "Nothing";
    }

    // 3) Either<L,R> — represents a success (Right) or a failure (Left)
    public readonly struct Either<L, R> : IMonad<R>
    {
        private readonly L left;
        private readonly R right;
        public bool IsRight { get; }
        public bool IsLeft => !IsRight;
        public L Left => IsLeft ? left : throw new InvalidOperationException("No Left present");
        public R Right => IsRight ? right : throw new InvalidOperationException("No Right present");

        private Either(L left)
        {
            this.left = left;
            right = default;
            IsRight = false;
        }

        private Either(R right)
        {
            this.right = right;
            left = default;
            IsRight = true;
        }

        // Constructors
        public static Either<L,R> FromLeft(L left)   => new(left);
        public static Either<L,R> FromRight(R right) => new(right);

        // Return: lifts an R into an Either<L,R>
        public static Either<L,R> Return(R value) => FromRight(value);

        public IMonad<U> Bind<U>(Func<R, IMonad<U>> binder)
        {
            return IsRight ? binder(right) : Either<L,U>.FromLeft(left);
        }

        // LINQ support
        public Either<L, U> Select<U>(Func<R, U> selector) => IsRight ?
            Either<L,U>.FromRight(selector(right)) :
            Either<L,U>.FromLeft(left);

        public Either<L, V> SelectMany<U, V>(Func<R, IMonad<U>> binder, Func<R, U, V> projector)
        {
            if (IsLeft)
                return Either<L,V>.FromLeft(left);
            
            var uCase = binder(right);
            
            if (!(uCase is Either<L,U> eu) || eu.IsLeft)
                return Either<L,V>.FromLeft(left);
            
            return Either<L,V>.FromRight(projector(right, eu.Right));
        }

        public override string ToString() => IsRight ? $"Right({right})" : $"Left({left})";
    }
}
