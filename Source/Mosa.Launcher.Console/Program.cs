using Mosa.Compiler.Common.Configuration;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using Mosa.Compiler.Framework.Trace;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Diagnostics;
using System.IO;

namespace Mosa.Launcher.Console
{
    class Program
    {
		private static Settings Settings = new Settings();
		private static string[] Arguments;

		private static CompilerHooks CompilerHooks;
		private static MosaLinker Linker;
		private static TypeSystem TypeSystem;

		private static string OutputName
		{
			get
			{
				return Arguments[1];
			}
		}
		private static string SourceName
		{
			get
			{
				return Arguments[0];
			}
		}
		private static string OutputFolder
		{
			get
			{
				return Path.GetDirectoryName(Arguments[1]);
			}
		}
		private static string SourceFolder
		{
			get
			{
				return Path.GetDirectoryName(Arguments[0]);
			}
		}
		private static bool VBEEnable
		{
			get
			{
				return Convert.ToBoolean(Arguments[2]);
			}
		}

		public static string AppFolder = @"C:\Program Files (x86)\MOSA-Project";
		public static string VMRunPath = @"C:\Program Files (x86)\VMware\VMware Workstation\vmrun.exe";
		public static string ISOFilePath
		{
			get
			{
				return Path.Combine(OutputFolder, "MOSA.iso");
			}
		}

		private static DateTime StartTime;

		static void Main(string[] args)
		{
			if (!Environment.Is64BitOperatingSystem)
			{
				System.Console.WriteLine("Fatal! 32-Bit Operating System Is Not Supported");
				System.Console.WriteLine("Press Any Key To Continue...");
				System.Console.ReadKey();
				return;
			}

			//Arguments 1: Source Name
			//Arguments 2: Output Name
			//Arguments 3: VBE Enable
			//If you want to change "main.exe" to other name you have to modify the syslinux.cfg
			Arguments = new string[] { args[0], AppFolder + @"\output\main.exe", args[2] };

			System.Console.WriteLine($"VBE Status: {VBEEnable}");

			DefaultSettings();
			RegisterPlatforms();
			SetFile();

			CompilerHooks = new CompilerHooks();
			CompilerHooks.NotifyEvent += NotifyEvent;

			if (Directory.Exists(OutputFolder))
			{
				Directory.Delete(OutputFolder, true);
			}

			Directory.CreateDirectory(OutputFolder);

			Compile();

			MakeISO();
			System.Console.WriteLine($"Output ISO:{ISOFilePath}");

			RunVMWareWorkstation();

			Finish();
			return;
		}

		private static void Finish()
		{
			System.Console.WriteLine("Press Any Key To Continue...");
			System.Console.ReadKey();
			Environment.Exit(0);
		}

		private static void Compile()
		{
			try
			{
				if (Settings.GetValue("Launcher.HuntForCorLib", false))
				{
					var fileCorlib = Path.Combine(SourceFolder, "mscorlib.dll");

					if (fileCorlib != null)
					{
						Settings.AddPropertyListValue("Compiler.SourceFiles", fileCorlib);
					}
				}

				if (Settings.GetValue("Launcher.PlugKorlib", false))
				{
					var fileKorlib = Path.Combine(SourceFolder, "Mosa.Plug.Korlib.dll");

					if (fileKorlib != null)
					{
						Settings.AddPropertyListValue("Compiler.SourceFiles", fileKorlib);
					}

					var platform = Settings.GetValue("Compiler.Platform", "x86");

					if (platform == "armv8a32")
					{
						platform = "ARMv8A32";
					}

					var fileKorlibPlatform = Path.Combine(SourceFolder, $"Mosa.Plug.Korlib.{platform}.dll");

					if (fileKorlibPlatform != null)
					{
						Settings.AddPropertyListValue("Compiler.SourceFiles", fileKorlibPlatform);
					}
				}

				var compiler = new MosaCompiler(Settings, CompilerHooks);

				compiler.Load();
				compiler.Initialize();
				compiler.Setup();
				compiler.Compile();

				Linker = compiler.Linker;
				TypeSystem = compiler.TypeSystem;

				GC.Collect();
			}
			catch (Exception)
			{
				System.Console.WriteLine("Exception Thrown While Compiling");
				Finish();
			}
		}

		private static void MakeISO()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(AppFolder+@"\Tools\syslinux");
			foreach (var v in directoryInfo.GetFiles())
			{
				v.CopyTo(Path.Combine(OutputFolder, v.Name), true);
			}

			var args = $"-relaxed-filenames -J -R -o \"{ISOFilePath}\" -b isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table \"{OutputFolder}\"";
			Process.Start(AppFolder + @"\Tools\mkisofs\mkisofs.exe", args);
		}

		private static void RunVMWareWorkstation()
		{
			if (!File.Exists(VMRunPath))
			{
				System.Console.WriteLine("VMWare Workstation Not Found!");
				return;
			}

			DirectoryInfo directoryInfo = new DirectoryInfo(AppFolder+@"\Tools\vmware");
			foreach (var v in directoryInfo.GetFiles())
			{
				v.CopyTo(Path.Combine(OutputFolder, v.Name), true);
			}

			var args = $"start \"{Path.Combine(OutputFolder,"MOSA.vmx")}\"";


			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.UseShellExecute = true;
			processStartInfo.Arguments = args;
			processStartInfo.FileName = VMRunPath;
			Process.Start(processStartInfo);
		}

		private static void NotifyEvent(CompilerEvent compilerEvent,string message,int threadID)
		{
			switch (compilerEvent)
			{
				case CompilerEvent.CompileStart:
					StartTime = DateTime.Now;
					System.Console.WriteLine($"Compile Start");
					break;
				case CompilerEvent.CompilingMethods:
					System.Console.WriteLine($"Compiling Methods");
					break;
				case CompilerEvent.CompilingMethodsCompleted:
					System.Console.WriteLine($"Compiling Methods Completed");
					break;
				case CompilerEvent.CompileEnd:
					TimeSpan timeSpan = DateTime.Now.Subtract(StartTime);
					System.Console.WriteLine($"Compile End {timeSpan}");
					break;
			}
		}

		private static void SetFile()
		{
			Settings.AddPropertyListValue("Compiler.SourceFiles", SourceName);
			Settings.AddPropertyListValue("SearchPaths", SourceFolder);
		}

		private static void RegisterPlatforms()
		{
			PlatformRegistry.Add(new Mosa.Platform.x86.Architecture());
		}

		private static void DefaultSettings()
		{
			Settings.SetValue("Compiler.BaseAddress", 0x00400000);
			Settings.SetValue("Compiler.Binary", true);
			Settings.SetValue("Compiler.MethodScanner", false);
			Settings.SetValue("Compiler.Multithreading", true);
			Settings.SetValue("Compiler.TraceLevel", 0);
			Settings.SetValue("Compiler.Multithreading", true);
			Settings.SetValue("CompilerDebug.DebugFile", string.Empty);
			Settings.SetValue("CompilerDebug.AsmFile", string.Empty);
			Settings.SetValue("CompilerDebug.MapFile", string.Empty);
			Settings.SetValue("CompilerDebug.NasmFile", string.Empty);
			Settings.SetValue("CompilerDebug.InlineFile", string.Empty);
			Settings.SetValue("Optimizations.Basic", true);
			Settings.SetValue("Optimizations.BitTracker", true);
			Settings.SetValue("Optimizations.Inline", true);
			Settings.SetValue("Optimizations.Inline.AggressiveMaximum", 24);
			Settings.SetValue("Optimizations.Inline.ExplicitOnly", false);
			Settings.SetValue("Optimizations.Inline.Maximum", 12);
			Settings.SetValue("Optimizations.LongExpansion", true);
			Settings.SetValue("Optimizations.LoopInvariantCodeMotion", true);
			Settings.SetValue("Optimizations.Platform", true);
			Settings.SetValue("Optimizations.SCCP", true);
			Settings.SetValue("Optimizations.Devirtualization", true);
			Settings.SetValue("Optimizations.SSA", true);
			Settings.SetValue("Optimizations.TwoPass", true);
			Settings.SetValue("Optimizations.ValueNumbering", true);
			Settings.SetValue("Image.BootLoader", "Syslinux3.72");
			//Settings.SetValue("Image.Folder", Path.Combine(Environment.CurrentDirectory, "MOSA"));
			Settings.SetValue("Image.Format", "ISO");
			Settings.SetValue("Image.FileSystem", "FAT16");
			Settings.SetValue("Image.ImageFile", "%DEFAULT%");
			Settings.SetValue("Multiboot.Version", "v1");
			Settings.SetValue("Multiboot.Video", VBEEnable);
			Settings.SetValue("Multiboot.Video.Width", 640);
			Settings.SetValue("Multiboot.Video.Height", 480);
			Settings.SetValue("Multiboot.Video.Depth", 32);
			Settings.SetValue("Emulator", "VMware");
			Settings.SetValue("Emulator.Memory", 128);
			Settings.SetValue("Emulator.Serial", "none");
			Settings.SetValue("Emulator.Serial.Host", "127.0.0.1");
			Settings.SetValue("Emulator.Serial.Port", new Random().Next(11111, 22222));
			Settings.SetValue("Emulator.Serial.Pipe", "MOSA");
			Settings.SetValue("Emulator.Display", true);
			Settings.SetValue("Launcher.Start", true);
			Settings.SetValue("Launcher.Launch", true);
			Settings.SetValue("Launcher.Exit", true);
			Settings.SetValue("Launcher.PlugKorlib", true);
			Settings.SetValue("Launcher.HuntForCorLib", true);
			Settings.SetValue("Linker.Drawf", false);
			Settings.SetValue("OS.Name", "MOSA");
			Settings.SetValue("Compiler.OutputFile", OutputName);
		}
	}
}
