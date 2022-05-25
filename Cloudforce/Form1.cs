using Newtonsoft.Json;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cloudforce
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            paneldynamicloader();
           
        }
        #region JSON/DYNAMIC Stuff
        int AllAppsCount; 
        public string jsonfile
        {
            get
            {
                WebRequest request = WebRequest.Create("https://www.kahootflooder.me/Zortos/CF-Universal-API/tools.geojson");
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
        }
        public void paneldynamicloader()
        {
            var results = JsonConvert.DeserializeObject<Root>(jsonfile);
            Allappslbl.Text = String.Format("All Apps ({0})", results.Apps.Count);
            AllAppsCount = results.Apps.Count;
            for (int i = 0; i < 24; i++)
            {
                int iOne = i;
                int iTwo = 1;
                if (i == 0)
                {
                    var Panellabel = (Guna.UI2.WinForms.Guna2Panel)Controls.Find("App" + "1" + "Panel", true)[0];
                    Panellabel.Name = results.Apps[i].Appname;
                    var imagebutton = (Guna.UI2.WinForms.Guna2ImageButton)Controls.Find("App" + "1" + "image", true)[0];
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(results.Apps[i].AppBanner);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        imagebutton.Image = Image.FromStream(ms);
                        imagebutton.PressedState.Image = Image.FromStream(ms);
                        imagebutton.HoverState.Image = Image.FromStream(ms);

                    }
                    if (results.Apps[i].Category == "Windows Utilities") { Panellabel.Tag = "Windows Utilities"; }
                    if (results.Apps[i].Category == "Multimedia Utilities") { Panellabel.Tag = "Multimedia Utilities"; }
                    if (results.Apps[i].Category == "Web Browsers") { Panellabel.Tag = "Web Browsers"; }
                }
                else
                {
                    if (results.Apps.Count > i)
                    {
                        if (results.Apps[i].AppBanner == null)
                        {
                            return;
                        }
                        var imagebutton = (Guna.UI2.WinForms.Guna2ImageButton)Controls.Find("App" + (iOne + iTwo).ToString() + "image", true)[0];
                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(results.Apps[i].AppBanner);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            imagebutton.Image = Image.FromStream(ms);
                            imagebutton.PressedState.Image = Image.FromStream(ms);
                            imagebutton.HoverState.Image = Image.FromStream(ms);

                        }

                        var PictureBLoxie = (Guna.UI2.WinForms.Guna2Panel)Controls.Find("App" + (iOne + iTwo).ToString() + "Panel", true)[0];
                        PictureBLoxie.Name = results.Apps[i].Appname;

                        if (results.Apps[i].Category == "Windows Utilities"){ PictureBLoxie.Tag = "Windows Utilities";}
                        if (results.Apps[i].Category == "Multimedia Utilities") { PictureBLoxie.Tag = "Multimedia Utilities"; }
                        if (results.Apps[i].Category == "Web Browsers") { PictureBLoxie.Tag = "Web Browsers"; }
                        
                        

                    }
                    else
                    {
                        return;
                    }


                }



            }
        }

        #endregion

        #region Download Stuff
        public string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        // DefaultDownloadLocation = exe path
        string zippath;        
        public string DefaultDownloadLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Apps\\";
        string downloadurl;
        string downloadextension;
        string DownloadApp;
        string DownloadLocaiton;
        string LaunchLocation;
        Stopwatch sw = new Stopwatch();
        public void Download(int i)
        {
            
            var results = JsonConvert.DeserializeObject<Root>(jsonfile);
            LaunchLocation = results.Apps[i].LaunchLocation;
            DownloadApp = results.Apps[i].Appname;
            if (Portablemodecheck.Checked)
            {
                // Foreach all downloads from results.Apps
                foreach (var download in results.Apps[i].DownloadMain)
                {
                    if (string.IsNullOrEmpty(download.PortableZip) && string.IsNullOrEmpty(download.PortableExe)) { return; }
                    if (!string.IsNullOrEmpty(download.PortableExe))
                    {
                        downloadurl = download.PortableExe;
                    }
                    else if (!string.IsNullOrEmpty(download.PortableZip))
                    {
                        downloadurl = download.PortableZip;
                    }   

                    if (downloadurl.Contains(".zip"))
                    {
                        downloadextension = ".zip";
                    }
                    else if (downloadurl.Contains(".exe"))
                    {
                        downloadextension = ".exe";
                    }



                    // If the download is the one we want to download
                }
            }
            else if (installermodecheck.Checked)
            {
                foreach (var download in results.Apps[i].DownloadMain)
                {
                    if (string.IsNullOrEmpty(download.SetupMSI) && string.IsNullOrEmpty(download.SetupExe)) { return; }
                    if (!string.IsNullOrEmpty(download.SetupMSI))
                    {
                        downloadurl = download.SetupMSI;
                    }
                    else if (!string.IsNullOrEmpty(download.SetupExe))
                    {
                        downloadurl = download.SetupExe;
                    }

                    if (downloadurl.Contains(".zip"))
                    {
                        downloadextension = ".zip";
                    }
                    else if (downloadurl.Contains(".exe"))
                    {
                        downloadextension = ".exe";
                    }



                    // If the download is the one we want to download
                }
            }

           
                using (var client = new WebClient())
                {

                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    sw.Start();
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        guna2ProgressBar1.Value = 0;
                        DownloadFileLBL.Visible = true;
                        guna2ProgressBar1.Visible = true;
                    });
                if (DefaultLocationCheck.Checked)
                {
                    client.DownloadFileAsync(new Uri(downloadurl), DefaultDownloadLocation + Path.GetFileName(downloadurl));
                }
                else if (CustomLocationCheck.Checked)
                {
                    if (string.IsNullOrEmpty(CustomLocationText.Text)) { MessageBox.Show("You dident Specify Custom Location"); return; }
                    client.DownloadFileAsync(new Uri(downloadurl), CustomLocationText.Text + Path.GetFileName(downloadurl));
                }
               
            }

            
        }

        async void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            Thread.Sleep(100);
            
                   this.BeginInvoke((MethodInvoker)delegate
                   {
                       DownloadFileLBL.Text = string.Format("Downloading {1}  {0} MB/s", (e.BytesReceived / 1024d / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00") , DownloadApp);
                       double bytesIn = double.Parse(e.BytesReceived.ToString());
                       double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                       double percentage = bytesIn / totalBytes * 100;
                       guna2ProgressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
                   });
            
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate {
                DownloadFileLBL.Text = string.Format("Downloaded {0}", DownloadApp);
                DownloadFileLBL.Visible = false;
                guna2ProgressBar1.Visible = false;
                
                sw.Reset();
            });

            if (downloadextension.Contains(".zip"))
            {
                
                if (DefaultLocationCheck.Checked)
                {
                    zippath = DefaultDownloadLocation;
                }
                else if (CustomLocationCheck.Checked)
                {
                    zippath = CustomLocationText.Text;
                }
                using (var archive = ZipArchive.Open(zippath + Path.GetFileName(downloadurl)))
                {
                    foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                    {
                        
                        try
                        {
                            entry.WriteToDirectory(DefaultDownloadLocation, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                                              
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Couldent extract file \n An Error Accoured");
                            return;
                        }
                        
                    }
                }
            }

            Process.Start(zippath + LaunchLocation);
        }
        
        #endregion

        #region UI Buttons
        public async void UIBTNPressed(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2ImageButton button = sender as Guna.UI2.WinForms.Guna2ImageButton;
            switch (button.Name)
            {
                case "App1image":

                    await Task.Run(() =>
                    {
                        Download(0);
                    });
                return;
                case "App2image":

                    await Task.Run(() =>
                    {
                        Download(1);
                    });
                    return;
                case "App3image":

                    await Task.Run(() =>
                    {
                        Download(2);
                    });
                    return;
                case "App4image":

                    await Task.Run(() =>
                    {
                        Download(3);
                    });
                    return;
                case "App5image":

                    await Task.Run(() =>
                    {
                        Download(4);
                    });
                    return;

            }
        }
        
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

       

        private void guna2Button10_Click(object sender, EventArgs e)
        {
           
            guna2TabControl1.SelectedIndex = 2;
            guna2Transition1.ShowSync(guna2TabControl1);

        }

        private void guna2Button8_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 1;
            Allappslbl.Text = String.Format("All Apps ({0})", AllAppsCount);
            guna2Transition1.ShowSync(guna2TabControl1);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 1;
            Allappslbl.Text = String.Format("All Apps ({0})", AllAppsCount);
            guna2Transition1.ShowSync(guna2TabControl1);
        }
        #endregion
       

       
        #region Settings

  
      
        private void installermodecheck_CheckedChanged(object sender, EventArgs e)
        {
            Portablemodecheck.Checked = false;
            hiddenmodecheck.Checked = false;
            installermodecheck.Checked = true;
        }

        private void hiddenmodecheck_CheckedChanged(object sender, EventArgs e)
        {
            Portablemodecheck.Checked = false;
            installermodecheck.Checked = false;
            hiddenmodecheck.Checked = true;
        }

        private void Portablemodecheck_CheckedChanged(object sender, EventArgs e)
        {

            installermodecheck.Checked = false;
            hiddenmodecheck.Checked = false;
            Portablemodecheck.Checked = false;
            Portablemodecheck.Checked = true;
        }
       

        private void CustomLocationCheck_CheckedChanged(object sender, EventArgs e)
        {
            DefaultLocationCheck.Checked = false;
            CustomLocationCheck.Checked = true;
            CustomLocationText.ReadOnly = false;
        }
        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            CustomLocationCheck.Checked = false;
            DefaultLocationCheck.Checked = true;
            CustomLocationText.ReadOnly = true;
        }
        #endregion
        #region CHECKS CLOUDSERVICE
        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);


        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (CloudChecker.GFN())
            {
                IntPtr handle = Process.GetCurrentProcess().MainWindowHandle;
                SetWindowText(handle, RandomString(7));
                guna2Panel1.Visible = true;

            }
        }
        #endregion
        int countapps;
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string compareTo = guna2TextBox1.Text.Trim().ToLower();
            foreach (Guna.UI2.WinForms.Guna2Panel c in flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Panel>())
            {
                c.Visible = c.Name.ToLower().Contains(compareTo);
            }
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            foreach (Guna.UI2.WinForms.Guna2Panel c in flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Panel>())
            {
                if (c.Tag == "Windows Utilities")
                {
                    c.Visible = true;
                    countapps += 1;

                }
                else
                {
                    c.Visible = false;
                }
            }
            Allappslbl.Text = string.Format("Windows Utilities ({0})", countapps);
            countapps = 0;
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            
            
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            foreach (Guna.UI2.WinForms.Guna2Panel c in flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Panel>())
            {
                if (c.Tag == "Multimedia Utilities")
                {
                    c.Visible = true;
                    countapps += 1;
                }
                else
                {
                    c.Visible = false;
                }

            }
            Allappslbl.Text = string.Format("Multimedia Utilities ({0})", countapps);
            countapps = 0;
        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {

            foreach (Guna.UI2.WinForms.Guna2Panel c in flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Panel>())
            {
                if (c.Tag == "Web Browsers")
                {
                    c.Visible = true;
                    countapps += 1;
                }
                else
                {
                    c.Visible = false;
                }
                
            }
            Allappslbl.Text = string.Format("Web Browsers ({0})", countapps);
            countapps = 0;
        }
    }
}
