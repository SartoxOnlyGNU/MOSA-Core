using Mosa.Compiler.Common.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

		Settings settings = new Settings();

        public Form1()
        {
            InitializeComponent();
			comboBox1.Items.Add(Arch.x86.ToString());
			comboBox1.Items.Add(Arch.ARMv8A32.ToString());
        }

    }
}
