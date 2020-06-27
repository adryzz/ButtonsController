using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonsController
{
    public partial class ConfigurationWindow : Form
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        private void ConfigurationWindow_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.PortName = textBox1.Text;
            cfg.BaudRate = Convert.ToInt32(numericUpDown1.Value);
            cfg.RunActionsOnNewThread = checkBox1.Checked;
            Program.Config = cfg;
            Program.Config.Save("config.xml");
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
