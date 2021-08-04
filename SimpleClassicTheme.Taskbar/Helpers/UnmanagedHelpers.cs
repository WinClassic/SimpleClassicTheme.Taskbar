using SimpleClassicThemeTaskbar.Helpers.NativeMethods;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

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

		public static T GetStruct<T>(byte[] bytes)
		{
			var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

			try
            {
				return Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
			}
            finally
			{
				handle.Free();
			}
		}

		public static List<IntPtr> FilterWindows(Func<IntPtr, bool> predicate)
		{
			var windows = new List<IntPtr>();

			bool callback(IntPtr window, int _)
			{
				bool addToList = predicate.Invoke(window);

				if (addToList)
					windows.Add(window);

				return true;
			}

            if (User32.EnumWindows(callback, 0) == 0)
            {
				windows.Clear();
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			return windows;
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
