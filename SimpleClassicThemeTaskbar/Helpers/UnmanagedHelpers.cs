using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleClassicThemeTaskbar.Helpers
{
    public static class UnmanagedHelpers
    {
		public static T CopyStructureFromProcess<T>(IntPtr hProcess, IntPtr baseAddress) where T : struct
		{
			// Read process memory
			var size = Marshal.SizeOf<T>();
			var buffer = new byte[size];
			Kernel32.ReadProcessMemory(hProcess, baseAddress, buffer, buffer.Length, out _);

			// Allocate memory for buffer and it to a managed type.
			var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			var @struct = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
			handle.Free();

			return @struct;
		}

		/// <summary>
		/// Reads a sequence of characters until a null character is encountered from a process.
		/// </summary>
		/// <param name="hProcess">A handle to the process</param>
		/// <param name="baseAddress">A pointer where to start reading from</param>
		/// <returns></returns>
		public static string ReadNullTerminatedString(IntPtr hProcess, IntPtr baseAddress)
		{
			StringBuilder stringBuilder = new();
			byte[] buffer = new byte[2];

			while (true)
			{
				Kernel32.ReadProcessMemory(hProcess, baseAddress, buffer, 2, out _);
				string character = Encoding.Unicode.GetString(buffer);

				if (character == "\0")
				{
					break;
				}

				stringBuilder.Append(character);
				baseAddress += 2;
			}

			return stringBuilder.ToString();
		}
	}
}
