namespace CodeZero.Core
{
    public readonly struct UpperBound<T> : IEquatable<UpperBound<T>>, IComparable<UpperBound<T>>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        public UpperBound(T value, bool isIncluded)
        {
            IsIncluded = isIncluded;
            Value = value;
        }

        public T Value { get; }
        public bool IsIncluded { get; }

        public int CompareTo(UpperBound<T> other)
        {
            if (this.Equals(other))
                return 0;
            
            if (Value.IsGreaterThan(other.Value))
                return 1;

            if (Value.IsLessThan(other.Value))
                return -1;
            
            return IsIncluded ? 1 : -1;
        }

        public bool Equals(UpperBound<T> other) => 
            Value.Equals(other.Value) && 
            IsIncluded.Equals(other.IsIncluded);
    }

    public readonly struct LowerBound<T> : IEquatable<LowerBound<T>>, IComparable<LowerBound<T>>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        public LowerBound(T value, bool isIncluded)
        {
            IsIncluded = isIncluded;
            Value = value;
        }

        public T Value { get; }
        public bool IsIncluded { get; }

        public int CompareTo(LowerBound<T> other)
        {
            if (this.Equals(other))
                return 0;
            
            if (Value.IsGreaterThan(other.Value))
                return 1;

            if (Value.IsLessThan(other.Value))
                return -1;
            
            return IsIncluded ? 1 : -1;
        }

        public bool Equals(LowerBound<T> other) => 
            Value.Equals(other.Value) && 
            IsIncluded.Equals(other.IsIncluded);

        public override string ToString() => IsIncluded ? $"[{Value}" : $"({Value}";
    }
}