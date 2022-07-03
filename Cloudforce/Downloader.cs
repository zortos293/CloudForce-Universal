using Newtonsoft.Json;
using Sentry;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Cloudforce.Form1;

namespace Cloudforce
{
    internal class Downloader
    {
        // DefaultDownloadLocation = exe path
        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text); // Hide Exe that launches

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public string DefaultDownloadLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Apps\\";

        private Stopwatch sw = new Stopwatch();
        private bool isexe = false;
        private bool iszip = false;
        int DownloadNumber;
        string ApplicationName;
        string zippath;
        public static bool IsEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return true;
            }

            return !list.Any();
        }

        public async void Download(int i)
        {
            var results = JsonConvert.DeserializeObject<Root>(Form1.jsonfile);
            DownloadNumber = i;
            ApplicationName = results.Apps[i].Name;
            if (Globals.form.Portablemodecheck.Checked || Globals.form.hiddenmodecheck.Checked)
            {
              
                // Change THIS ^^
                if (!string.IsNullOrEmpty(results.Apps[i].DownloadLinks[0].Exelocation))
                {
                    if (Globals.form.CustomLocationCheck.Checked)
                    {
                        // Checking if file exists 
                        if (File.Exists(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation))
                        {
                            Process p = Process.Start(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation);
                            Thread.Sleep(200);
                            SetWindowText(p.MainWindowHandle, RandomString(9));
                            return;
                        }

                    }
                    else if (Globals.form.DefaultLocationCheck.Checked)
                    {
                        if (File.Exists(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation))
                        {
                            Process p = Process.Start(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation);
                            Thread.Sleep(200);
                            SetWindowText(p.MainWindowHandle, RandomString(9));
                            return;
                        }
                        // Checking if file exists 
                    }
                    await checklinks(i, 0);
                    iszip = true;
                }
                else if (!string.IsNullOrEmpty(results.Apps[i].DownloadLinks[1].Exelocation))
                {
                    if (Globals.form.CustomLocationCheck.Checked)
                    {
                        // Checking if file exists 
                        if (File.Exists(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation))
                        {
                            Process p = Process.Start(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation);
                            Thread.Sleep(100);  
                            SetWindowText(p.MainWindowHandle, RandomString(9));
                            return;
                        }

                    }
                    else if (Globals.form.DefaultLocationCheck.Checked) 
                    {
                        if (File.Exists(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation))
                        {
                            Process p = Process.Start(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation);
                            Thread.Sleep(100);  
                            SetWindowText(p.MainWindowHandle, RandomString(9));
                            return;
                        }
                        // Checking if file exists 
                       
                       
                    }
                   




                    await checklinks(i, 1);
                        isexe = true;
                    
                    
                    
                   
                }

                // If the download is the one we want to download
            }

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                sw.Start();
                Globals.form.BeginInvoke((MethodInvoker)delegate
                {
                    Globals.form.guna2ProgressBar1.Value = 0;
                    Globals.form.DownloadFileLBL.Visible = true;
                    Globals.form.guna2ProgressBar1.Visible = true;
                });
                if (Globals.form.DefaultLocationCheck.Checked)
                {
                    try
                    {
                        client.DownloadFileAsync(new Uri(DownloadLink), DefaultDownloadLocation + Path.GetFileName(DownloadLink));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error : " + e.Message);
                    }
                }
                else if (Globals.form.CustomLocationCheck.Checked)
                {
                    if (string.IsNullOrEmpty(Globals.form.CustomLocationText.Text)) { MessageBox.Show("You dident Specify Custom Location"); return; }
                    try
                    {
                        client.DownloadFileAsync(new Uri(DownloadLink), Globals.form.CustomLocationText.Text + Path.GetFileName(DownloadLink));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Error : " + e.Message);
                    }
                }
            }
        }

        private string DownloadLink;

        private async Task checklinks(int Appint, int linkint)
        {
            var results = JsonConvert.DeserializeObject<Root>(Form1.jsonfile);
            foreach (var links in results.Apps[Appint].DownloadLinks[linkint].Links)
            {
                HttpClient client = new HttpClient();
                var checkingResponse = await client.GetAsync(links);
                if (checkingResponse.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(DownloadLink))
                    {
                        DownloadLink = links;
                    }
                }
            }
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Thread.Sleep(100);

            Globals.form.BeginInvoke((MethodInvoker)delegate
            {
                Globals.form.DownloadFileLBL.Text = string.Format("Downloading {1}  {0} MB/s", (e.BytesReceived / 1024d / 1024d / sw.Elapsed.TotalSeconds).ToString("0.00"), ApplicationName);
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                Globals.form.guna2ProgressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Globals.form.BeginInvoke((MethodInvoker)delegate
            {
                Globals.form.DownloadFileLBL.Text = string.Format("Downloaded {0}", ApplicationName);
                Globals.form.DownloadFileLBL.Visible = false;
                Globals.form.guna2ProgressBar1.Visible = false;

                sw.Reset();
            });
            var results = JsonConvert.DeserializeObject<Root>(Form1.jsonfile);
            if (iszip)
            {
                
                if (Globals.form.DefaultLocationCheck.Checked)
                {
                    zippath = DefaultDownloadLocation;
                }
                else if (Globals.form.CustomLocationCheck.Checked)
                {
                    zippath = Globals.form.CustomLocationText.Text;
                }
               
                string filename;
               
                
                    filename = System.IO.Path.GetFileName(DownloadLink);
                    
                
                using (var archive = ZipArchive.Open(zippath + Path.GetFileName(filename)))
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
                        catch (Exception)
                        {
                            MessageBox.Show("Couldent extract file \n An Error Accoured");
                            iszip = false;
                            return;
                        }
                    }
                }
                
                if (Globals.form.CustomLocationCheck.Checked)
                {
                    try
                    {
                        Process p = Process.Start(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation);
                        Thread.Sleep(100);  // <-- ugly hack
                        SetWindowText(p.MainWindowHandle, RandomString(9));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                        MessageBox.Show("Exe Failed to Launch \nCrash report has been send");
                    }
                }
                else
                {
                    
                    try
                    {
                        Process p = Process.Start(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation);
                        Thread.Sleep(100);  // <-- ugly hack
                        SetWindowText(p.MainWindowHandle, RandomString(9));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                        MessageBox.Show("Exe Failed to Launch \nCrash report has been send");
                    }
                    
                   
                }
                iszip = false;
                DownloadLink = "";
                return;
            }
            else if (isexe)
            {
                DownloadLink = "";
                if (Globals.form.CustomLocationCheck.Checked)
                {
                    try
                    {
                        Process p = Process.Start(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation);
                        Thread.Sleep(100);  // <-- ugly hack
                        SetWindowText(p.MainWindowHandle, RandomString(9));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                        MessageBox.Show("Exe Failed to Launch \nCrash report has been send");
                    }
                }
                else
                {
                    try
                    {
                        Process p = Process.Start(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation);
                        Thread.Sleep(100);  // <-- ugly hack
                        SetWindowText(p.MainWindowHandle, RandomString(9));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                        MessageBox.Show("Exe Failed to Launch \nCrash report has been send");
                    }
                }
                isexe = false;
                return;
            }
        }
    }
}