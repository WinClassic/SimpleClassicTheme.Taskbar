using System;

namespace SimpleClassicThemeTaskbar.Helpers.NativeMethods
{
    /// <summary>
    /// Integer class for use when a WinAPI function declaration specifies a handle
    /// Note that if the value is 64-bit and you cast it to an int/uint this class
    /// will throw and exception because you're incorrectly handling a value and
    /// bits are being lost in the process of doing that.
    /// </summary>
    internal struct Integer
    {
        readonly IntPtr value;

        private Integer(IntPtr value) => this.value = value;

        public static implicit operator long(Integer i) => i.value.ToInt64();
        public static implicit operator Integer(long i) => new(new(i));

        public static implicit operator IntPtr(Integer i) => i.value;
        public static implicit operator Integer(IntPtr i) => new(i);

        public static implicit operator ulong(Integer i) => (ulong)i.value.ToInt64();
        public static implicit operator Integer(ulong i) => new(new((long)i));

        public static implicit operator int(Integer i) => i.value.ToInt32();
        public static implicit operator Integer(int i) => new(new(i));

        public static implicit operator uint(Integer i) => (uint)i.value.ToInt32();
        public static implicit operator Integer(uint i) => new(new(i));

        public static implicit operator Integer(bool i) => new(new(i ? 1 : 0));
        public static implicit operator Integer(Integer32 i) => new(new(i));

        public override string ToString() => value.ToString();
        public string ToString(string? format, IFormatProvider? provider) => value.ToString(format, provider);
        public string ToString(string? format) => value.ToString(format);
        public string ToString(IFormatProvider? provider) => value.ToString(provider);
    }

    /// <summary>
    /// Integer class for use when a WinAPI function declaration specifically specifies a double word (32-bit) value
    /// </summary>
    internal struct Integer32
    {
        readonly uint value;

        private Integer32(uint value) => this.value = value;

        public static implicit operator uint(Integer32 i) => i.value;
        public static implicit operator Integer32(uint i) => new(i);

        public static implicit operator int(Integer32 i) => (int)i.value;
        public static implicit operator Integer32(int i) => new((uint)i);

        public static implicit operator IntPtr(Integer32 i) => new((int)i.value);
        public static implicit operator Integer32(IntPtr i) => new((uint)(i.ToInt64() & uint.MaxValue));

        public static implicit operator Integer32(bool i) => new(i ? 1U : 0U);

        public override string ToString() => value.ToString();
        public string ToString(string? format, IFormatProvider? provider) => value.ToString(format, provider);
        public string ToString(string? format) => value.ToString(format);
        public string ToString(IFormatProvider? provider) => value.ToString(provider);
    }

    /// <summary>
    /// Integer class for use when a WinAPI function declaration specifically specifies a long (64-bit) value
    /// </summary>
    internal struct Integer64
    {
        readonly ulong value;

        private Integer64(ulong value) => this.value = value;

        public static implicit operator ulong(Integer64 i) => i.value;
        public static implicit operator Integer64(ulong i) => new(i);

        public static implicit operator long(Integer64 i) => (long)i.value;
        public static implicit operator Integer64(long i) => new((ulong)i);

        public static implicit operator IntPtr(Integer64 i) => new((long)i.value);
        public static implicit operator Integer64(IntPtr i) => new((ulong)i.ToInt64());

        public static implicit operator Integer64(int i) => new((ulong)i);
        public static implicit operator Integer64(uint i) => new(i);
        public static implicit operator Integer64(bool i) => new(i ? 1UL : 0UL);

        public override string ToString() => value.ToString();
        public string ToString(string? format, IFormatProvider? provider) => value.ToString(format, provider);
        public string ToString(string? format) => value.ToString(format);
        public string ToString(IFormatProvider? provider) => value.ToString(provider);
    }
}
