using System;

namespace SimpleClassicTheme.Taskbar.Helpers.NativeMethods
{
    /*
     * When to use what integer type:
     * 
     * Integer:
     * Any WINAPI variable that is a handle (the ones that starts with H)
     * Any variable that is a pointer (the ones that start with lp)
     * 
     * Integer32:
     * int, long, DWORD
     * INT_PTR, UINT_PTR
     * LONG_PTR, ULONG_PTR (as crazy as it may sound to a .NET developer, long is 32-bit in C++)
     * WPARAM, LPARAM
     * 
     * Integer64:
     * This one should only be used when you create a definition for a function
     * made only for 64-bit systems. (eg. SetWindowLongPtr or GetWindowClassPtr)
     * 
     * Why take this approach? Because I'm sick of casting integers from one type
     * to the other while they are practically the same value. 
     * 
     * Note: These types should only be used for the P/Invoke methods themselves.
     * 
     * Note: Integer may cause a runtime error when being cast to a 32-bit integer
     *       on 64-bit systems. This is because the value represents a 64-bit value
     *       and therefore the cast would result in the loss of information.
     *       
     * Note: Integer may cause a runtime error when a long is being cast to Integer
     *       on 32-bit systems. This is because Integer can only store a 32-bit value 
     *       on 32-bit systems and because a long is 64-bits the cast would result
     *       in the loss of information.
     * 
     */

    /// <summary>
    /// Integer class for use when a WinAPI function declaration specifies a handle
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
        public static implicit operator Integer(Integer64 i) => new(new(i));

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
