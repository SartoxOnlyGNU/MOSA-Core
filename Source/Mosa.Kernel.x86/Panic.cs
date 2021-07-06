// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace Mosa.Kernel.x86
{
	/// <summary>
	/// Panic
	/// </summary>
	public static class Panic
	{
		private static bool firstError = true;

		public static uint EBP = 0;
		public static uint EIP = 0;
		public static uint EAX = 0;
		public static uint EBX = 0;
		public static uint ECX = 0;
		public static uint EDX = 0;
		public static uint EDI = 0;
		public static uint ESI = 0;
		public static uint ESP = 0;
		public static uint Interrupt = 0;
		public static uint ErrorCode = 0;
		public static uint CS = 0;
		public static uint EFLAGS = 0;
		public static uint CR2 = 0;
		public static uint FS = 0;

		public static void Setup()
		{
		}

		public static void Error(string message)
		{
			IDT.SetInterruptHandler(null);

			Console.BackgroundColor = ConsoleColor.Black;

			Console.Clear();
			Console.WriteLine("Kernel Panic !");

			DumpStackTrace();

			while (true)
			{
				Native.Hlt();
			}
		}

		public static void DumpStackTrace()
		{
			DumpStackTrace(0);
		}

		private static void DumpStackTrace(uint depth)
		{
			while (true)
			{
				var entry = Runtime.Internal.GetStackTraceEntry(depth, new Pointer(EBP), new Pointer(EIP));

				if (!entry.Valid)
					return;

				if (!entry.Skip)
				{
					Console.Write(entry.ToString());
					Console.Row++;
					Console.Column = 0;
				}

				depth++;
			}
		}
	}
}
