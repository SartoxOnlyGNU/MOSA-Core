using Mosa.Compiler.Common.Configuration;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class Form1 : Form
    {
		enum Arch
		{
			x86,
			ARMv8A32
		}

		Settings Settings = new Settings();
		static string FileName = @"C:\Users\Fanfa Ni\source\repos\Mosa.Starter.x86\Mosa.Starter.x86\bin\Mosa.Starter.x86.exe";
		static string Dir = FileName.Substring(0, FileName.LastIndexOf(@"\"));


		public Form1()
        {
            InitializeComponent();

			Debug.WriteLine(Dir);

			//UI
			comboBox1.Items.Add(Arch.x86.ToString());
			comboBox1.Items.Add(Arch.ARMv8A32.ToString());

			//Settings
			Settings.AddPropertyListValue("Compiler.SourceFiles", FileName);
			Settings.AddPropertyListValue("SearchPaths", Path.GetDirectoryName(FileName));
			Settings.SetValue("Compiler.BaseAddress", 0x00400000);
			Settings.SetValue("Compiler.Binary", true);
			Settings.SetValue("Compiler.MethodScanner", false);
			Settings.SetValue("Compiler.Multithreading", true);
			Settings.SetValue("Compiler.Platform", "x86");
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
			Settings.SetValue("Image.Folder", Path.Combine(Environment.CurrentDirectory, "MOSA"));
			Settings.SetValue("Image.Format", "ISO");
			Settings.SetValue("Image.FileSystem", "FAT16");
			Settings.SetValue("Image.ImageFile", "%DEFAULT%");
			Settings.SetValue("Multiboot.Version", "v1");
			Settings.SetValue("Multiboot.Video", false);
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

			//RegisterPlatfroms
			RegisterPlatfroms();

		}

		private void RegisterPlatfroms()
		{
			PlatformRegistry.Add(new Mosa.Platform.x86.Architecture());
			PlatformRegistry.Add(new Mosa.Platform.x64.Architecture());
			PlatformRegistry.Add(new Mosa.Platform.ARMv8A32.Architecture());
		}

		CompilerHooks CompilerHooks = new CompilerHooks();
		public MosaLinker Linker;
		public TypeSystem TypeSystem;

		private void button2_Click(object sender, EventArgs e)
		{
			if (Settings.GetValue("Launcher.HuntForCorLib", false))
			{
				var fileCorlib = Path.Combine(Dir, "mscorlib.dll");

				if (fileCorlib != null)
				{
					Settings.AddPropertyListValue("Compiler.SourceFiles", fileCorlib);
				}
			}

			if (Settings.GetValue("Launcher.PlugKorlib", false))
			{
				var fileKorlib = Path.Combine(Dir, "Mosa.Plug.Korlib.dll");

				if (fileKorlib != null)
				{
					Settings.AddPropertyListValue("Compiler.SourceFiles", fileKorlib);
				}

				var platform = Settings.GetValue("Compiler.Platform", "x86");

				if (platform == "armv8a32")
				{
					platform = "ARMv8A32";
				}

				var fileKorlibPlatform = Path.Combine(Dir, $"Mosa.Plug.Korlib.{platform}.dll");

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

			Debug.WriteLine("Finished");
		}
	}
}
