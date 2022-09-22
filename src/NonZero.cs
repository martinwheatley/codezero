namespace category_theory;

public struct NonZero<T>
    where T : struct, INumber<T>
{
    private readonly T _value;

    public NonZero(T value)
    {
        _value = value != T.Zero
            ? value
            : throw new ArgumentException($"{T.Zero} is not allowed in the type {nameof(NonZero<T>)}.", nameof(value));
    }

    public T Value =>
        _value == T.Zero
            ? T.One
            : _value;

    public static implicit operator T(NonZero<T> value) => value.Value;
    public static explicit operator NonZero<T>(T value) => new(value);
}