using Newtonsoft.Json;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Cloudforce.Form1;

namespace Cloudforce
{
    internal class Downloader
    {
        // DefaultDownloadLocation = exe path
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
                    await checklinks(i, 0);
                    iszip = true;
                }
                else if (!string.IsNullOrEmpty(results.Apps[i].DownloadLinks[1].Exelocation))
                {
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
                    Process.Start(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation);
                }
                else
                {
                    Process.Start(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[0].Exelocation);
                }
                iszip = false;
                return;
            }
            else if (isexe)
            {
                if (Globals.form.CustomLocationCheck.Checked)
                {
                    Process.Start(Globals.form.CustomLocationText.Text + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation);
                }
                else
                {
                    Process.Start(DefaultDownloadLocation + results.Apps[DownloadNumber].DownloadLinks[1].Exelocation);
                }
                isexe = false;
                return;
            }
        }
    }
}