#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace DataSizeUnits;

public partial struct DataSize {

    /// <exception cref="DivideByZeroException"><paramref name="b"/> is <see cref="Zero"/></exception>
    static DataSize IDivisionOperators<DataSize, DataSize, DataSize>.operator /(DataSize a, DataSize b) {
        if (!b.Bits.Equals(BigInteger.Zero)) {
            return new DataSize(a.Bits / b.Bits);
        } else {
            throw new DivideByZeroException($"Cannot divide {a} by zero");
        }
    }

    static DataSize INumberBase<DataSize>.One => new(BigInteger.One);
    static int INumberBase<DataSize>.Radix => 2;

    /// <inheritdoc />
    public static DataSize Zero => new(BigInteger.Zero);

    static DataSize ISignedNumber<DataSize>.NegativeOne => new(BigInteger.MinusOne);
    static DataSize IAdditiveIdentity<DataSize, DataSize>.AdditiveIdentity => Zero;
    static DataSize IMultiplicativeIdentity<DataSize, DataSize>.MultiplicativeIdentity => new(BigInteger.One);

    /// <inheritdoc />
    public static DataSize operator --(DataSize value) {
        BigInteger bits = value.Bits;
        return new DataSize(--bits);
    }

    /// <inheritdoc />
    public static DataSize operator ++(DataSize value) {
        BigInteger bits = value.Bits;
        return new DataSize(++bits);
    }

    /// <inheritdoc />
    public static DataSize operator %(DataSize left, DataSize right) => new(left.Bits % right.Bits);

    /// <inheritdoc />
    public static DataSize operator *(DataSize left, DataSize right) => new(left.Bits * right.Bits);

    /// <inheritdoc />
    public static DataSize operator -(DataSize value) => new(-value.Bits);

    /// <inheritdoc />
    public static DataSize operator +(DataSize value) => new(+value.Bits);

    /// <inheritdoc />
    public static DataSize Abs(DataSize value) => new(BigInteger.Abs(value.Bits));

    static bool INumberBase<DataSize>.IsCanonical(DataSize value) => true;

    static bool INumberBase<DataSize>.IsComplexNumber(DataSize value) => false;

    static bool INumberBase<DataSize>.IsEvenInteger(DataSize value) => BigInteger.IsEvenInteger(value.Bits);

    static bool INumberBase<DataSize>.IsFinite(DataSize value) => true;

    static bool INumberBase<DataSize>.IsImaginaryNumber(DataSize value) => false;

    static bool INumberBase<DataSize>.IsInfinity(DataSize value) => false;

    static bool INumberBase<DataSize>.IsInteger(DataSize value) => true;

    static bool INumberBase<DataSize>.IsNaN(DataSize value) => false;

    /// <inheritdoc />
    public static bool IsNegative(DataSize value) => BigInteger.IsNegative(value.Bits);

    static bool INumberBase<DataSize>.IsNegativeInfinity(DataSize value) => false;

    static bool INumberBase<DataSize>.IsNormal(DataSize value) => value.Bits != 0;

    static bool INumberBase<DataSize>.IsOddInteger(DataSize value) => BigInteger.IsOddInteger(value.Bits);

    /// <inheritdoc />
    public static bool IsPositive(DataSize value) => BigInteger.IsPositive(value.Bits);

    static bool INumberBase<DataSize>.IsPositiveInfinity(DataSize value) => false;

    static bool INumberBase<DataSize>.IsRealNumber(DataSize value) => true;

    static bool INumberBase<DataSize>.IsSubnormal(DataSize value) => false;

    /// <inheritdoc />
    public static bool IsZero(DataSize value) => value.Bits.IsZero;

    static DataSize INumberBase<DataSize>.MaxMagnitude(DataSize x, DataSize y) => new(BigInteger.MaxMagnitude(x.Bits, y.Bits));
    static DataSize INumberBase<DataSize>.MaxMagnitudeNumber(DataSize x, DataSize y) => new(BigInteger.MaxMagnitude(x.Bits, y.Bits));

    static DataSize INumberBase<DataSize>.MinMagnitude(DataSize x, DataSize y) => new(BigInteger.MinMagnitude(x.Bits, y.Bits));
    static DataSize INumberBase<DataSize>.MinMagnitudeNumber(DataSize x, DataSize y) => new(BigInteger.MinMagnitude(x.Bits, y.Bits));

    /// <inheritdoc />
    public readonly bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) {
        if (!Bits.TryFormat(destination, out charsWritten, format, provider) || destination.Length < charsWritten + 2) {
            return false;
        }
        destination[charsWritten++] = ' ';
        destination[charsWritten++] = 'b';
        return true;
    }

    /// <inheritdoc />
    public static bool TryConvertFromChecked<TOther>(TOther value, out DataSize result) where TOther: INumberBase<TOther> {
        if (typeof(TOther) == typeof(BigInteger)) {
            result = new DataSize((BigInteger) (object) value);
        } else {
            try {
                result = new DataSize(BigInteger.CreateChecked(value));
            } catch (OverflowException) {
                result = Zero;
                return false;
            }
        }
        return true;
    }

    /// <inheritdoc />
    public static bool TryConvertFromSaturating<TOther>(TOther value, out DataSize result) where TOther: INumberBase<TOther> {
        result = typeof(TOther) == typeof(BigInteger)
            ? new DataSize((BigInteger) (object) value)
            : new DataSize(BigInteger.CreateSaturating(value));
        return true;
    }

    /// <inheritdoc />
    public static bool TryConvertFromTruncating<TOther>(TOther value, out DataSize result) where TOther: INumberBase<TOther> {
        result = typeof(TOther) == typeof(BigInteger)
            ? new DataSize((BigInteger) (object) value)
            : new DataSize(BigInteger.CreateTruncating(value));
        return true;
    }

    /// <inheritdoc />
    public static bool TryConvertToChecked<TOther>(DataSize value, [MaybeNullWhen(false)] out TOther result) where TOther: INumberBase<TOther> {
        try {
            result = TOther.CreateChecked(value.Bits);
        } catch (OverflowException) {
            result = default;
            return false;
        }
        return true;
    }

    /// <inheritdoc />
    public static bool TryConvertToSaturating<TOther>(DataSize value, [MaybeNullWhen(false)] out TOther result) where TOther: INumberBase<TOther> {
        result = TOther.CreateSaturating(value.Bits);
        return true;
    }

    /// <inheritdoc />
    public static bool TryConvertToTruncating<TOther>(DataSize value, [MaybeNullWhen(false)] out TOther result) where TOther: INumberBase<TOther> {
        result = TOther.CreateTruncating(value.Bits);
        return true;
    }

}
#endif