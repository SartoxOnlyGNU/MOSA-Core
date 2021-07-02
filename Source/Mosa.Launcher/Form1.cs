using Mosa.Compiler.Common.Configuration;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Linker;
using Mosa.Compiler.Framework.Trace;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Launcher
{
	public partial class Form1 : Form
	{
		enum Arch
		{
			x86,
			x64,
			ARMv8A32
		}

		Settings Settings = new Settings();
		static string FileName;
		static string Dir;

		Arch arch = Arch.x86;

		string configFile = "Launcher.cfg";

		public Form1(string[] args)
		{
			InitializeComponent();

			toolStripStatusLabel1.Text = "";

			//UI
			comboBox1.Items.Add(Arch.x86.ToString());
			comboBox1.Items.Add(Arch.x64.ToString() + "(Experimental)");
			comboBox1.Items.Add(Arch.ARMv8A32.ToString());

			//Default Settings
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
			Settings.SetValue("Compiler.OutputFile", Environment.CurrentDirectory + @"\output\ISO\main.exe");

			//RegisterPlatfroms
			RegisterPlatfroms();

			if(args.Length == 0)
			{
				//SetFile
				if (File.Exists(configFile))
				{
					string[] l = File.ReadAllText(configFile).Split('\n');
					SetFile(l[0]);
				}
			}
			else
			{
				SetFile(Environment.CurrentDirectory + $@"\{args[0]}");
			}
			

			//SetArch
			SetArch(Arch.x86);

			CompilerHooks = new CompilerHooks();
			CompilerHooks.NotifyEvent = NotifyEvent;
		}

		DateTime startTime;

		void NotifyEvent(CompilerEvent e, string message, int threadID)
		{
			switch (e)
			{
				case CompilerEvent.CompileStart:
					startTime = DateTime.Now;
					break;
				case CompilerEvent.CompileEnd:
					MakeISO();
					TimeSpan timeSpan = DateTime.Now.Subtract(startTime);
					this.Invoke(new Action(() =>
					{
						toolStripStatusLabel1.Text = $"Finished {timeSpan.Hours.ToString().PadLeft(2, '0')}:{timeSpan.Minutes.ToString().PadLeft(2, '0')}:{timeSpan.Seconds.ToString().PadLeft(2, '0')} output/MOSA.iso";
					}));
					Launch();
					break;
			}
		}

		private void RegisterPlatfroms()
		{
			PlatformRegistry.Add(new Mosa.Platform.x86.Architecture());
			PlatformRegistry.Add(new Mosa.Platform.x64.Architecture());
			PlatformRegistry.Add(new Mosa.Platform.ARMv8A32.Architecture());
		}

		CompilerHooks CompilerHooks;
		public MosaLinker Linker;
		public TypeSystem TypeSystem;

		private void button2_Click(object sender, EventArgs e)
		{
			if (Directory.Exists("output"))
			{
				Directory.Delete("output", true);
			}

			if (FileName == null)
			{
				MessageBox.Show("Select a file first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
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
			}
			catch (NotImplementedException)
			{
				toolStripStatusLabel1.Text = $"Faild To Compile";
			}
		}

		private void MakeISO()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo("../Tools/syslinux");
			foreach (var v in directoryInfo.GetFiles())
			{
				if (!Directory.Exists(Environment.CurrentDirectory + @"\output\ISO"))
				{
					Directory.CreateDirectory(Environment.CurrentDirectory + @"\output\ISO");
				}
				v.CopyTo(Environment.CurrentDirectory + @"\output\ISO\" + v.Name, true);
			}

			var args = $"-relaxed-filenames -J -R -o \"{Environment.CurrentDirectory + @"\output\MOSA.iso"}\" -b isolinux.bin -no-emul-boot -boot-load-size 4 -boot-info-table \"{Environment.CurrentDirectory + @"\output\ISO"}\"";
			Process.Start("../Tools/mkisofs/mkisofs.exe", args);
		}

		private void Launch()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo("../Tools/vmware");
			foreach (var v in directoryInfo.GetFiles())
			{
				v.CopyTo(Environment.CurrentDirectory + @"\output\" + v.Name, true);
			}

			Process.Start(@"C:\Program Files (x86)\VMware\VMware Workstation\vmrun.exe", $"start {Environment.CurrentDirectory + @"\output.MOSA.vmx"}");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "|*.exe";
			openFileDialog.ShowDialog();

			SetFile(openFileDialog.FileName);
		}

		private void SetFile(string name)
		{
			File.WriteAllText(configFile, $"{name}");

			FileName = name;

			Dir = FileName.Substring(0, FileName.LastIndexOf(@"\"));

			Settings.ClearProperty("Compiler.SourceFiles");
			Settings.ClearProperty("SearchPaths");

			Settings.AddPropertyListValue("Compiler.SourceFiles", FileName);
			Settings.AddPropertyListValue("SearchPaths", Path.GetDirectoryName(FileName));

			label1.Text = $"File: {Path.GetFileName(FileName)}";
		}

		private void SetArch(Arch _arch)
		{
			arch = _arch;
			Settings.SetValue("Compiler.Platform", arch.ToString());
			comboBox1.SelectedIndex = (int)arch;
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetArch((Arch)comboBox1.SelectedIndex);
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}
	}
}
