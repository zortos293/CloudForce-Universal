using Newtonsoft.Json;

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
        public class Globals
        {
            public static Form1 form;
        }

        Downloader downloader = new Downloader();
        public static string jsonfile  // Need to change < (Important)
        {
            get
            {
                WebRequest request = WebRequest.CreateHttp("https://zortos293.github.io/ZortosMainSite/CF-Universal-API/tools.json");
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
               // string responseFromServer = File.ReadAllText("C:\\test.json");
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
                    Panellabel.Name = results.Apps[i].Name;
                    var imagebutton = (Guna.UI2.WinForms.Guna2ImageButton)Controls.Find("App" + "1" + "image", true)[0];
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(results.Apps[i].Banner);
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
                        if (results.Apps[i].Banner == null)
                        {
                            return;
                        }
                        var imagebutton = (Guna.UI2.WinForms.Guna2ImageButton)Controls.Find("App" + (iOne + iTwo).ToString() + "image", true)[0];
                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(results.Apps[i].Banner);
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            imagebutton.Image = Image.FromStream(ms);
                            imagebutton.PressedState.Image = Image.FromStream(ms);
                            imagebutton.HoverState.Image = Image.FromStream(ms);

                        }

                        var PictureBLoxie = (Guna.UI2.WinForms.Guna2Panel)Controls.Find("App" + (iOne + iTwo).ToString() + "Panel", true)[0];
                        PictureBLoxie.Name = results.Apps[i].Name;

                        if (results.Apps[i].Category == "Windows Utilities") { PictureBLoxie.Tag = "Windows Utilities"; }
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

 

        #region UI Buttons
        public async void UIBTNPressed(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2ImageButton button = sender as Guna.UI2.WinForms.Guna2ImageButton;
            switch (button.Name)
            {
                case "App1image":

                    await Task.Run(() =>
                    {
                        downloader.Download(0);
                    });
                    return;
                case "App2image":

                    await Task.Run(() =>
                    {
                        downloader.Download(1);
                    });
                    return;
                case "App3image":

                    await Task.Run(() =>
                    {
                        downloader.Download(2);
                    });
                    return;
                case "App4image":

                    await Task.Run(() =>
                    {
                        downloader.Download(3);
                    });
                    return;
                case "App5image":

                    await Task.Run(() =>
                    {
                        downloader.Download(4);
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
        private void guna2Button9_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 3;
            var html = Markdig.Markdown.ToHtml(new WebClient().DownloadString(new Uri("https://www.kahootflooder.me/Zortos/CF-Universal-API/News.md")));
            webBrowser1.DocumentText = @"<body style='background - color:#3a4659;'>" + html;
            webBrowser1.Document.ForeColor = Color.Blue;
            guna2Transition1.ShowSync(guna2TabControl1);
        }
        private void guna2Button8_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 1;
            Allappslbl.Text = String.Format("All Apps ({0})", AllAppsCount);
            foreach (Guna.UI2.WinForms.Guna2Panel c in flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Panel>())
            {
                c.Visible = true;

            }
            guna2Transition1.ShowSync(guna2TabControl1);
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (CloudChecker.GFN())
            {
                guna2TabControl1.SelectedIndex = 4;
            }
            else
            {
                MessageBox.Show("ONLY For GFN Session");
            }
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            guna2TabControl1.SelectedIndex = 1;
            Allappslbl.Text = String.Format("All Apps ({0})", AllAppsCount);
            foreach (Guna.UI2.WinForms.Guna2Panel c in flowLayoutPanel1.Controls.OfType<Guna.UI2.WinForms.Guna2Panel>())
            {
               c.Visible = true;

            }
            guna2Transition1.ShowSync(guna2TabControl1);
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
        #endregion



        #region Settings



       

        private void hiddenmodecheck_CheckedChanged(object sender, EventArgs e)
        {
            Portablemodecheck.Checked = false;
           
            hiddenmodecheck.Checked = true;
        }

        private void Portablemodecheck_CheckedChanged(object sender, EventArgs e)
        {

            
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
                Portablemodecheck.Checked = false;
                hiddenmodecheck.Checked = true;
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
       

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void guna2Button11_Click(object sender, EventArgs e)
        {
            WebClient WebDL = new WebClient();
             WebDL.DownloadFile(new Uri(""), downloader.DefaultDownloadLocation + "ZortosDesktop.exe");

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Globals.form = this;
        }
    }
}
