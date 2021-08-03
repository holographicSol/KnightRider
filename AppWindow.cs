using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Management.Instrumentation;
using System.Management;
using System.Collections.Specialized;

namespace SystemTrayApp
{
    public partial class AppWindow : Form
    {
        bool cpu_spike_bool = false;
        bool cpu_flux_thread_bool = true;
        bool disk_monitor_bool = true;
        bool cpu_percent_util_thread_bool = true;
        ContextMenuStrip menu = new ContextMenuStrip();
        ToolStripMenuItem mi_0;
        ToolStripMenuItem mi_1;
        ToolStripMenuItem mi_2;
        ToolStripMenuItem mi_exit;

        class CustomProfessionalColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            { get { return Color.FromArgb(37, 37, 37); } }

            public override Color MenuBorder
            { get { return Color.Black; } }

            //fill màu item của menu khi mouse enter
            public override Color MenuItemSelectedGradientBegin
            { get { return Color.FromArgb(37, 37, 37); } }

            public override Color MenuItemSelectedGradientEnd
            { get { return Color.FromArgb(37, 37, 37); } }

            // chọn màu viền menu item khi mouse enter
            public override Color MenuItemBorder
            { get { return Color.FromArgb(37, 37, 37); } }

            // fill màu nút item của menu khi dc nhấn
            public override Color MenuItemPressedGradientBegin
            { get { return Color.FromArgb(37, 37, 37); } }

            public override Color MenuItemPressedGradientEnd
            { get { return Color.FromArgb(37, 37, 37); } }

            // fill màu thanh menu strip
            public override Color MenuStripGradientBegin
            { get { return Color.FromArgb(37, 37, 37); } }

            public override Color MenuStripGradientEnd
            { get { return Color.FromArgb(37, 37, 37); } }

            public override Color ToolStripDropDownBackground
            { get { return Color.Black; } }

			public override Color ImageMarginGradientBegin
            { get { return Color.Black; } }

            public override Color ImageMarginGradientEnd
            { get { return Color.Black; } }
        }

        public AppWindow()
        {
            InitializeComponent();
            this.CenterToScreen();

            menu.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());

            this.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.Icon = Properties.Resources.Default;
            this.SystemTrayIcon.ContextMenuStrip = menu;

            // Change the Text property to the name of your application
            this.SystemTrayIcon.Text = "Knight Rider";
            this.SystemTrayIcon.Visible = true;

            this.Resize += WindowResize;
            this.FormClosing += WindowClosing;


            // Run Thread On Program Startup
            if (cpu_flux_thread_bool == true)
            {
                Thread thread = new Thread(new ThreadStart(cpu_flux_thread));
                thread.Start();
            }
            if (cpu_percent_util_thread_bool == true)
            {
                Thread thread_cpu_util_percent = new Thread(new ThreadStart(cpu_percent_util_thread));
                thread_cpu_util_percent.Start();
            }
            if (disk_monitor_bool == true)
            {
                Start_DiskMonitor_Thread();
            }

            ContextMenu_Type_0();
        }

        private bool mshow;
        protected override void SetVisibleCore(bool value)
        {
            if (!mshow) value = false;
            {
                base.SetVisibleCore(value);
            }
        }

        private void SystemTrayIconDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        public void ContextMenu_Type_0()
        {
            menu.Items.Clear();

            if (cpu_flux_thread_bool == true)
            {
                mi_0 = new ToolStripMenuItem("Stop KnightRider", null, new EventHandler(ContextMenu_Toggle_KnightRider));
                mi_0.BackColor = Color.Black;
                mi_0.ForeColor = Color.White;
                mi_0.Image = Image.FromFile("./resources/Default.ico");
                menu.Items.Add(mi_0);
            }
            else if (cpu_flux_thread_bool == false)
            {
                mi_0 = new ToolStripMenuItem("Run KnightRider", null, new EventHandler(ContextMenu_Toggle_KnightRider));
                mi_0.BackColor = Color.Black;
                mi_0.ForeColor = Color.White;
                mi_0.Image = Image.FromFile("./resources/Default.ico");
                menu.Items.Add(mi_0);
            }

            if (disk_monitor_bool == true)
            {
                mi_1 = new ToolStripMenuItem("Stop Disk Monitor", null, new EventHandler(ContextMenu_Toggle_DiskMonitor));
                mi_1.BackColor = Color.Black;
                mi_1.ForeColor = Color.White;
                mi_1.Image = Image.FromFile("./resources/Default.ico");
                menu.Items.Add(mi_1);
            }
            else if (disk_monitor_bool == false)
            {
                mi_1 = new ToolStripMenuItem("Run Disk Monitor", null, new EventHandler(ContextMenu_Toggle_DiskMonitor));
                mi_1.BackColor = Color.Black;
                mi_1.ForeColor = Color.White;
                mi_1.Image = Image.FromFile("./resources/Default.ico");
                menu.Items.Add(mi_1);
            }

            if (cpu_percent_util_thread_bool == true)
            {
                mi_2 = new ToolStripMenuItem("Stop CPU Utilization Monitor", null, new EventHandler(ContextMenu_Toggle_CPU_UtilizationMonitor));
                mi_2.BackColor = Color.Black;
                mi_2.ForeColor = Color.White;
                mi_2.Image = Image.FromFile("./resources/Default.ico");
                menu.Items.Add(mi_2);
            }
            else if (cpu_percent_util_thread_bool == false)
            {
                mi_2 = new ToolStripMenuItem("Run CPU Utilization Monitor", null, new EventHandler(ContextMenu_Toggle_CPU_UtilizationMonitor));
                mi_2.BackColor = Color.Black;
                mi_2.ForeColor = Color.White;
                mi_2.Image = Image.FromFile("./resources/Default.ico");
                menu.Items.Add(mi_2);
            }

            mi_exit = new ToolStripMenuItem("Exit", null, new EventHandler(ContextMenuExit));
            mi_exit.BackColor = Color.Black;
            mi_exit.ForeColor = Color.White;
            menu.Items.Add(mi_exit);
        }

        public void Start_DiskMonitor_Thread()
        {
            Thread thread_0 = new Thread(new ThreadStart(hdd_led_thread_0));
            Thread thread_1 = new Thread(new ThreadStart(hdd_led_thread_1));
            Thread thread_2 = new Thread(new ThreadStart(hdd_led_thread_2));
            Thread thread_3 = new Thread(new ThreadStart(hdd_led_thread_3));
            Thread thread_4 = new Thread(new ThreadStart(hdd_led_thread_4));
            Thread thread_5 = new Thread(new ThreadStart(hdd_led_thread_5));
            Thread thread_6 = new Thread(new ThreadStart(hdd_led_thread_6));
            Thread thread_7 = new Thread(new ThreadStart(hdd_led_thread_7));
            Thread thread_8 = new Thread(new ThreadStart(hdd_led_thread_8));
            Thread thread_9 = new Thread(new ThreadStart(hdd_led_thread_9));
            Thread thread_10 = new Thread(new ThreadStart(hdd_led_thread_10));
            Thread thread_11 = new Thread(new ThreadStart(hdd_led_thread_11));
            Thread thread_12 = new Thread(new ThreadStart(hdd_led_thread_12));
            Thread thread_13 = new Thread(new ThreadStart(hdd_led_thread_13));
            Thread thread_14 = new Thread(new ThreadStart(hdd_led_thread_14));
            Thread thread_15 = new Thread(new ThreadStart(hdd_led_thread_15));
            Thread thread_16 = new Thread(new ThreadStart(hdd_led_thread_16));
            Thread thread_17 = new Thread(new ThreadStart(hdd_led_thread_17));
            Thread thread_18 = new Thread(new ThreadStart(hdd_led_thread_18));
            Thread thread_19 = new Thread(new ThreadStart(hdd_led_thread_19));
            Thread thread_20 = new Thread(new ThreadStart(hdd_led_thread_20));
            Thread thread_21 = new Thread(new ThreadStart(hdd_led_thread_21));
            Thread thread_22 = new Thread(new ThreadStart(hdd_led_thread_22));
            Thread thread_23 = new Thread(new ThreadStart(hdd_led_thread_23));
            thread_0.Start();
            thread_1.Start();
            thread_2.Start();
            thread_3.Start();
            thread_4.Start();
            thread_5.Start();
            thread_6.Start();
            thread_7.Start();
            thread_8.Start();
            thread_9.Start();
            thread_10.Start();
            thread_11.Start();
            thread_12.Start();
            thread_13.Start();
            thread_14.Start();
            thread_15.Start();
            thread_16.Start();
            thread_17.Start();
            thread_18.Start();
            thread_19.Start();
            thread_20.Start();
            thread_21.Start();
            thread_22.Start();
            thread_23.Start();
        }

        public void ContextMenu_Toggle_KnightRider(object sender, EventArgs e)
        {
            if (cpu_flux_thread_bool == false)
            {
                Console.WriteLine("-- starting: KnightRider");
                cpu_flux_thread_bool = true;
                ContextMenu_Type_0();
                Thread thread = new Thread(new ThreadStart(cpu_flux_thread));
                thread.Start();
            }
            else if (cpu_flux_thread_bool == true)
            {
                Console.WriteLine("-- stopping: KnightRider");
                cpu_flux_thread_bool = false;
                ContextMenu_Type_0();
            }
        }

        public void ContextMenu_Toggle_DiskMonitor(object sender, EventArgs e)
        {
            if (disk_monitor_bool == false)
            {
                Console.WriteLine("-- starting: DiskMonitor");
                disk_monitor_bool = true;
                Start_DiskMonitor_Thread();
                ContextMenu_Type_0();
                
            }
            else if (disk_monitor_bool == true)
            {
                Console.WriteLine("-- stopping: DiskMonitor");
                disk_monitor_bool = false;
                ContextMenu_Type_0();
                
            }
        }

        public void ContextMenu_Toggle_CPU_UtilizationMonitor(object sender, EventArgs e)
        {
            if (cpu_percent_util_thread_bool == false)
            {
                Console.WriteLine("-- starting: CPU_UtilizationMonitor");
                cpu_percent_util_thread_bool = true;
                ContextMenu_Type_0();
                Thread thread = new Thread(new ThreadStart(cpu_percent_util_thread));
                thread.Start();
            }
            else if (cpu_percent_util_thread_bool == true)
            {
                Console.WriteLine("-- stopping: CPU_UtilizationMonitor");
                cpu_percent_util_thread_bool = false;
                ContextMenu_Type_0();
            }
        }

        public void cpu_flux_thread()
        {
            string cpu_spike_visor = System.IO.Directory.GetCurrentDirectory() + "\\kb_ms_visor.exe";
            PerformanceCounter cpuCounter;
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            float last_cpu_counter = 0;
            while (cpu_flux_thread_bool == true)
            {
                if (((cpuCounter.NextValue()) > (last_cpu_counter + 45)) && (cpu_spike_bool == false))
                {
                    Console.WriteLine("-- running: cpu_flux_thread subprocess");
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = cpu_spike_visor;
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    using (Process process = Process.Start(start))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            Console.Write(result);
                        }
                    }
                }
                last_cpu_counter = cpuCounter.NextValue();
                Thread.Sleep(400);
            }
		}

        public void cpu_percent_util_thread()
        {
            if (cpu_percent_util_thread_bool == true)
            {
                string cpu_threshold = System.IO.Directory.GetCurrentDirectory() + "\\kb_cpu_util_mon.exe";
                using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\kb_cpu_util_mon.sys"))
                {
                    writer.WriteLine("running: True");
                }
                try
                {
                    ProcessStartInfo start = new ProcessStartInfo();
                    start.FileName = cpu_threshold;
                    start.UseShellExecute = false;
                    start.RedirectStandardOutput = true;
                    Process process = Process.Start(start);
                }
                catch (Exception)
                {
                }
            }
            while (cpu_flux_thread_bool == true)
            {
                Thread.Sleep(100);
                if (cpu_percent_util_thread_bool == false)
                {
                    using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\kb_cpu_util_mon.sys"))
                    {
                        writer.WriteLine("running: False");
                    }

                }
            }
            
        }

        public void hdd_led_thread_0()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "C")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_c.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_1()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "D")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_d.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                        
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_2()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "E")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_e.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_3()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "F")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\led_flash_f.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_4()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "G")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_g.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_5()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "H")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_h.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_6()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "I")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_i.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_7()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "J")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_j.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_8()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "K")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_k.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_9()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "L")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_l.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_10()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "M")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_m.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_11()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "N")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_n.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_12()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "O")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_o.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_13()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "P")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_p.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_14()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "Q")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_q.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_15()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "R")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_r.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_16()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "S")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_s.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_17()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "T")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_t.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_18()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "U")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_u.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_19()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "V")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_v.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_20()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "W")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_w.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_21()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "X")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_x.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_22()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "Y")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_y.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        public void hdd_led_thread_23()
        {
            Console.WriteLine("-- running: hdd indicator thread");
            ManagementClass driveDataClass = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");
            try
            {
                // Main loop where all the magic happens
                while (disk_monitor_bool == true)
                {
                    // Connect to the drive performance instance 
                    ManagementObjectCollection driveDataClassCollection = driveDataClass.GetInstances();
                    foreach (ManagementObject obj in driveDataClassCollection)
                    {
                        if (obj["Name"].ToString() != "_Total")
                        {
                            string name = obj["Name"].ToString();
                            if (name.Count() > 1)
                            {
                                string name_var = name.Split()[1];
                                if (name_var[0].ToString() == "Z")
                                {
                                    if (Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                                    {
                                        try
                                        {
                                            ProcessStartInfo start = new ProcessStartInfo();
                                            start.FileName = System.IO.Directory.GetCurrentDirectory() + "\\kb_alpha_z.exe";
                                            start.UseShellExecute = false;
                                            start.RedirectStandardOutput = true;
                                            using (Process process = Process.Start(start))
                                            {
                                                using (StreamReader reader = process.StandardOutput)
                                                {
                                                    string result = reader.ReadToEnd();
                                                    Console.Write(result);
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (ThreadAbortException tbe)
            {
                driveDataClass.Dispose();
                // Thead was aborted
            }
        }

        private void ContextMenuExit(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(System.IO.Directory.GetCurrentDirectory() + "\\kb_cpu_util_mon.sys"))
            {
                writer.WriteLine("running: False");
            }
            this.SystemTrayIcon.Visible = false;
            Application.Exit();
            Environment.Exit(0);
        }

        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void WindowClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
