using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonsController
{
    class Program
    {
        public static SerialPort Port { get; private set; }

        public static Configuration Config = new Configuration();

        public static ActionsManager Manager { get; private set; }

        static NotifyIcon notifyIcon1;

        static ContextMenuStrip contextMenuStrip1;

        static void Main(string[] args)
        {
            Console.WriteLine("Application Started");

            if (!File.Exists("CustomActions.dll"))
            {
                Console.WriteLine("Could not find CustomActions.dll\nExiting...");
                return;
            }

            Manager = new ActionsManager("CustomActions");

            bool needSetup = true;

            //import configuration

            if (File.Exists("config.xml"))
            {
                try
                {
                    Config = Configuration.FromFile("config.xml");
                    needSetup = false;//this runs only if the assignment of Config runs without errors
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while parsing config file: " + ex.Message + "\nReset config? (Y/N)");
                    ConsoleKeyInfo k = Console.ReadKey();
                    if (k.KeyChar == 'Y' || k.KeyChar == 'y')
                    {
                        Config = new Configuration();
                        try
                        {
                            Config.Save("config.xml");
                        }
                        catch (Exception ex1)
                        {
                            Console.WriteLine("Exception while saving the configuration file: " + ex1.Message + "\nExiting...");
                            return;
                        }
                        //since the assignment of needSetup didn't run, we can omit it
                    }
                    else
                    {
                        Console.WriteLine("Exiting...");
                        return;
                    }
                }
            }
            else
            {
                try
                {
                    //Config.Save("config.xml");
                }
                catch (Exception ex1)
                {
                    Console.WriteLine("Exception while saving the configuration file: " + ex1.Message + "\nExiting...");
                    return;
                }
            }

            //setup configuration if running for the first time

            if (needSetup)
            {
                Application.EnableVisualStyles();
                ConfigurationWindow cfg = new ConfigurationWindow();
                DialogResult res = cfg.ShowDialog();
                if (res == DialogResult.OK)
                {

                }
                else
                {
                    Console.WriteLine("Config file not created\nExiting...");
                    return;
                }
            }

            Console.WriteLine("CustomActions DLL loaded!");

            //import dll and initialize it
            Manager.InitializeLibrary();
            Application.ApplicationExit += Application_ApplicationExit;

            //initialize and open serial port
            try
            {
                Port = new SerialPort(Config.PortName, Config.BaudRate, Parity.None, 8, StopBits.One);
                Port.DataReceived += Port_DataReceived;
                Port.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while opening port {0} with baud rate {1}: {2}\nExiting...", Config.PortName, Config.BaudRate, ex.Message);
                return;
            }

            Console.WriteLine("Serial port opened!");

            Console.WriteLine("Waiting for commands...");

            Application.Run();
        }

        void SetupNotifyIcon()
        {

            //
            // notifyIcon1
            // 
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            configurationToolStripMenuItem,
            exitToolStripMenuItem});
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new System.Drawing.Size(149, 48);
            // 
            // configurationToolStripMenuItem
            // 
            configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            configurationToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            configurationToolStripMenuItem.Text = "Configuration";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            exitToolStripMenuItem.Text = "Exit";
        }

        private static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte actionCode = Convert.ToByte(Port.ReadByte());
            Manager.ExecuteAction(actionCode);
            Port.DiscardInBuffer();
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Manager.Dispose();
        }
    }
}
