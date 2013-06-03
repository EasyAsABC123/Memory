// <copyright file="Memory.cs" company="SchuCreations">
// Copyright (c) 2009-2010 All Right Reserved
// </copyright>
// <author>Justin Schuhmann</author>
// <email>justin.schuhmann@gmail.com</email>
// <date>2010-1-28</date>
// <summary>Used to Read and Write to memory.</summary>

namespace Voodoo
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Text;

	public static class Memory
	{
		public static bool Write( long Address, byte value)
		{
			byte[] bytes = BitConverter.GetBytes((short)value);
			return Write(Address, bytes, sizeof(byte));
		}

		public static bool Write( long Address, short value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write(Address, bytes, sizeof(short));
		}

		public static bool Write( long Address, long value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write( Address, bytes);
		}

		public static bool Write( long Address, byte[] buffer)
		{
			if (Process.GameRunning)
				return ProcessMemoryReaderApi.WriteProcessMemory(Voodoo.Process.Handle, (UIntPtr)Address, buffer, (UIntPtr)buffer.Length, IntPtr.Zero);
			else
				return false;
		}

		public static bool Write( long Address, double value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write( Address, bytes);
		}

		public static bool Write( long Address, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write(Address, bytes, sizeof(int));
		}

		public static bool Write( long Address, sbyte value)
		{
			byte[] bytes = BitConverter.GetBytes((short)value);
			return Write(Address, bytes, sizeof(sbyte));
		}

		public static bool Write( long Address, float value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write( Address, bytes);
		}

		public static bool Write( long Address, string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			return Write( Address, bytes);
		}

		public static bool Write( long Address, ushort value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write(Address, bytes, sizeof(ushort));
		}

		public static bool Write( long Address, uint value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write(Address, bytes, sizeof(uint));
		}

		public static bool Write( long Address, ulong value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			return Write( Address, bytes);
		}

		public static bool Write( long Address, byte[] buffer, int size)
		{
			if (Process.GameRunning)
				return ProcessMemoryReaderApi.WriteProcessMemory(Voodoo.Process.Handle, (UIntPtr)Address, buffer, (UIntPtr)size, IntPtr.Zero);
			else
				return false;
		}

		public static object Read(UIntPtr lpBaseAddress, Type type)
		{
			return Read(lpBaseAddress, type, (uint)Marshal.SizeOf(type));
		}

		public static object Read(UIntPtr lpBaseAddress, Type type, uint size)
		{
			IntPtr bytesRead = IntPtr.Zero;
			return Read(lpBaseAddress, type, size, out bytesRead);
		}

		/// <summary>This function will read an from memory using the given parameters.</summary>
		/// <param name="lpBaseAddress">The address you want to read.</param>
		/// <param name="type">The type of variable you are wanting to read.</param>
		/// <param name="size">The number of bytes to be read.</param>
		/// <param name="lpNumberOfBytesRead">The number of bytes actually read.</param>
		/// <returns>An object with the results of the read.</returns>
		public static object Read(UIntPtr lpBaseAddress, Type type, uint size, out IntPtr bytesRead)
		{
			// The array of bytes for the returned data to be stored in.
			byte[] buffer = new byte[size];
			bytesRead = IntPtr.Zero;
			// Calls the c api for reading from memory.
			if (Process.GameRunning && Convert.ToBoolean(ProcessMemoryReaderApi.ReadProcessMemory(Voodoo.Process.Handle, lpBaseAddress, buffer, size, out bytesRead)))
			{
				// This handles the different types of variables.
				switch (Type.GetTypeCode(type))
				{
					// Handles if the memory to be read is an int
					case TypeCode.Int32:
						return System.BitConverter.ToInt32(buffer, 0);

					// Handles if the memory to be read is an int16
					case TypeCode.Int16:
						return System.BitConverter.ToInt16(buffer, 0);

					// Handles if the memory to be read is a string
					case TypeCode.String:
						return Encoding.Unicode.GetString(buffer);

					// Handles if the memory to be read is a single (float)
					case TypeCode.Single:
						return System.BitConverter.ToSingle(buffer, 0);

					case TypeCode.UInt32:
						return System.BitConverter.ToUInt32(buffer, 0);

					// Handles if the memory to be read is a byte
					case TypeCode.Byte:
						return buffer[0];

					// Handles if the memory to be read is a type that isn't supported.
					default:
						return new object();
				}
			}
			else
			{
				// This handles the different types of variables.
				switch (Type.GetTypeCode(type))
				{
					// Handles if the memory to be read is an int
					case TypeCode.Int32:
						return int.MaxValue;

					// Handles if the memory to be read is an int16
					case TypeCode.Int16:
						return Int16.MaxValue;

					// Handles if the memory to be read is a string
					case TypeCode.String:
						return string.Empty;

					// Handles if the memory to be read is a single (float)
					case TypeCode.Single:
						return float.MaxValue;

					case TypeCode.UInt32:
						return uint.MaxValue;

					// Handles if the memory to be read is a byte
					case TypeCode.Byte:
						return byte.MaxValue;

					// Handles if the memory to be read is a type that isn't supported.
					default:
						return new object();
				}
			}
		}
	
        /// <summary>
        /// ProcessMemoryReader is a class that enables direct reading a process memory
        /// </summary>
		class ProcessMemoryReaderApi
        {
			/// <summary>
			/// Closes an open object handle
			/// </summary>
			/// <param name="hObject">A valid handle to an open object</param>
			/// <returns>If the function succeeds a non-zero, zero if the function fails.</returns>
            [DllImport("kernel32.dll")]
            public static extern Int32 CloseHandle(IntPtr hObject);

			[DllImport("kernel32.dll")]
			public static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesRead);

			/// <summary>
			/// Reads memory
			/// </summary>
			/// <param name="hProcess">The handle to the process.</param>
			/// <param name="lpBaseAddress">The starting address to read</param>
			/// <param name="buffer">The byte array to hold the results</param>
			/// <param name="size">The number of bytes to read.</param>
			/// <param name="lpNumberOfBytesRead">The number of bytes read.</param>
			/// <returns>If the function succeeds a non-zero, if it fails a zero.</returns>
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out()] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

			/// <summary>
			/// Reads memory
			/// </summary>
			/// <param name="hProcess">The handle to the process.</param>
			/// <param name="lpBaseAddress">The starting address to read</param>
			/// <param name="buffer">The byte array to hold the results</param>
			/// <param name="size">The number of bytes to read.</param>
			/// <param name="lpNumberOfBytesRead">The number of bytes read.</param>
			/// <returns>If the function succeeds a non-zero, if it fails a zero.</returns>
			[DllImport("kernel32.dll")]
			public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

			/// <summary>
			/// Reads memory
			/// </summary>
			/// <param name="hProcess">The handle to the process.</param>
			/// <param name="lpBaseAddress">The starting address to read</param>
			/// <param name="buffer">The byte array to hold the results</param>
			/// <param name="size">The number of bytes to read.</param>
			/// <param name="lpNumberOfBytesRead">The number of bytes read.</param>
			/// <returns>If the function succeeds a non-zero, if it fails a zero.</returns>
			[DllImport("kernel32.dll")]
			public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] float buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);

			/// <summary>
			/// Gets Modules file names
			/// </summary>
			/// <param name="hProcess"></param>
			/// <param name="hModule"></param>
			/// <param name="lpBaseName"></param>
			/// <param name="nSize"></param>
			/// <returns></returns>
			[DllImport("psapi.dll")]
			public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

			/// <summary>Writes data to an area of memory in a specified process. The entire area to be written to must be accessible or the operation fails.</summary>
			/// <param name="hProcess">A handle to the process memory to be modified. The handle must have PROCESS_VM_WRITE and PROCESS_VM_OPERATION access to the process.</param>
			/// <param name="lpBaseAddress">A pointer to the base address in the specified process to which data is written. Before data transfer occurs, the system verifies that all data in the base address and memory of the specified size is accessible for write access, and if it is not accessible, the function fails.</param>
			/// <param name="lpBuffer">A pointer to the buffer that contains data to be written in the address space of the specified process.</param>
			/// <param name="nSize">The number of bytes to be written to the specified process.</param>
			/// <param name="lpNumberOfBytesWritten">A pointer to a variable that receives the number of bytes transferred into the specified process. This parameter is optional. If lpNumberOfBytesWritten is NULL, the parameter is ignored.</param>
			/// <returns>If the function succeeds, the return value is nonzero.
			/// <para>If the function fails, the return value is 0 (zero). To get extended error information, call GetLastError. The function fails if the requested write operation crosses into an area of the process that is inaccessible.</para></returns>
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

			[DllImport("kernel32.dll")]
			public static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesWritten);
		}
	}
}
