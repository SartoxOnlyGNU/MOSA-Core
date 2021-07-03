// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External;
using Mosa.Kernel.x86;

namespace Mosa.Demo.Experiment
{
    public static class Boot
    {
		public static void Main()
		{
			Mosa.Kernel.x86.Kernel.Setup();

			IDT.SetInterruptHandler(ProcessInterrupt);

			Screen.Clear();
			Screen.Goto(0, 0);
			Screen.Color = ScreenColor.White;

			while (true)
			{
				Screen.Goto(0, 0);
				Screen.Write("Available Memory:"+Memory.GetAvailableMemory().ToString("x2"));
			}
		}

        static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
        }
    }
}
