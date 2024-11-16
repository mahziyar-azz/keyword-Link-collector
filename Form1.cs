using System;
using System.Web;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Threading;
using System.Runtime.InteropServices;

namespace link_collector
{
    public partial class Form1 : Form
    {
       
        // Import SendMessage from the User32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);

        private const uint EM_SETCUEBANNER = 0x1501; // Message to set the placeholder text



        public Form1()
        {
            InitializeComponent();

            SetPlaceholder(input_textBox, "Enter your URL like : https://example.com/ ...");
            SetPlaceholder(Key_textBox, "720p / part / rar / zip / {app name} Etc ...");
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            // Use SendMessage to set the placeholder text
            SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholder);
        }
        
        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void Output_textBox_TextChanged(object sender, EventArgs e)
        {
        }

        private async void btnFetch_Click(object sender, EventArgs e)
        {
            if (input_textBox.Text == "")
            {
                MessageBox.Show("The link field is empty !", "URL required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Key_textBox.Text == "")
            {
                MessageBox.Show("The Key word is empty !", "Key word required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Output_textBox.Clear();
            Output_textBox.ForeColor = SystemColors.WindowFrame;
            Output_textBox.Text = "Loading . . .";
            string url = input_textBox.Text;
            List<string> links = await CollectLinksAsync(url);

            if (links != null && links.Count > 0)
            {
                Output_textBox.Clear();
                foreach (var link in links)
                {
                    Output_textBox.ForeColor = Color.Black;
                    Output_textBox.AppendText(link + "\n");
                }
            }
            else
            {
                Output_textBox.Clear();
                MessageBox.Show("No links to show!", "No link", MessageBoxButtons.OK,MessageBoxIcon.Error);                
            }
        }


        public async Task<List<string>> CollectLinksAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var doc = new HtmlAgilityPack.HtmlDocument(); // Fully qualify the class
                        doc.LoadHtml(content);

                        List<string> links = new List<string>();

                        // Check if there are any <a> tags with href attributes
                        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
                        if (linkNodes != null)
                        {
                            foreach (var a in linkNodes)
                            {
                                var hrefValue = WebUtility.HtmlDecode(a.GetAttributeValue("href", string.Empty));
                                if (hrefValue.Contains(Key_textBox.Text))
                                {
                                    links.Add(hrefValue);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No links were found on the webpage.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        return links;
                    }
                    else
                    {
                        MessageBox.Show($"Failed to retrieve the webpage. Status code: {response.StatusCode}",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return new List<string>();
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP-related errors
                MessageBox.Show($"A network error occurred: {ex.Message}",
                                "Network Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<string>();
            }
            catch (Exception ex)
            {
                // Handle all other errors
                MessageBox.Show($"An error occurred: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<string>();
            }
        }

        // Class-level tooltip instances
        private ToolTip successTooltip = new ToolTip
        {
            InitialDelay = 500,
            UseAnimation = true,
            UseFading = true,
            IsBalloon = false,
            ToolTipTitle = "Success", // Tooltip title
            ToolTipIcon = ToolTipIcon.Info, // Blue info icon
        };

        private ToolTip errorTooltip = new ToolTip
        {
            //InitialDelay = 500,
            UseAnimation = true,
            UseFading = true,
            IsBalloon = false,
            ToolTipTitle = "Error", // Tooltip title
            ToolTipIcon = ToolTipIcon.Error // Red error icon
        };

        private void Copy_button_Click(object sender, EventArgs e)
        {
            string outputText = Output_textBox.Text;
            if (!string.IsNullOrEmpty(outputText))
            {
                Clipboard.SetText(outputText);

                // Hide the tooltip if it's already showing
                successTooltip.Hide(Copy_button);

                // Display the success tooltip near the button for a short duration
                successTooltip.Show("Links copied to clipboard!", Copy_button, 0, -50, 1000); // Tooltip lasts 1 second
            }
            else
            {
                // Hide the error tooltip if it's already showing
                errorTooltip.Hide(Copy_button);

                // Display the error tooltip near the button for a short duration
                errorTooltip.Show("No links to copy!", Copy_button, 0, -50, 1000); // Tooltip lasts 1 second
            }
        }
               
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadIconInBackground();          
        }

        private void LoadIconInBackground()
        {
            pictureBox1.Image = Properties.Resources.git;
            BackgroundWorker backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += BackgroundWorker1_RunWorkerCompleted;

            // Start the background worker
            backgroundWorker1.RunWorkerAsync();
        }
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = "https://img.icons8.com/ios-filled/50/40C057/link--v1.png";

            try
            {
                using (WebClient client = new WebClient())
                {
                    byte[] imageBytes = client.DownloadData(url);
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        e.Result = Image.FromStream(ms); // Pass the downloaded image to the main thread
                    }
                }
            }
            catch
            {
                // Load fallback image from project resources if the URL fails

                e.Result = Properties.Resources.red_link; // Ensure this resource is added in your project
            }
        }
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

                // Handle any exceptions thrown during DoWork

                MessageBox.Show("An error occurred while loading the icon: " + e.Error.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update the PictureBox on the main thread

            if (e.Result is Image image)
            {
                pictureBox1.Image = image;
            }
            else
            {
                MessageBox.Show("Failed to load the icon.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LoadIconInBackground();
        }

        // Class-level tooltip to avoid creating multiple instances
        private ToolTip statusToolTip = new ToolTip
        {
            InitialDelay = 0,
            UseAnimation = true,
            IsBalloon = false,
            UseFading = true,
            ToolTipTitle = "Connection Status", // Tooltip title
            ToolTipIcon = ToolTipIcon.Info // Red error icon
        };

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            // Show the tooltip near the PictureBox
            statusToolTip.Show(" ↑ This icon shows the Internet connection status!\n" +
                "Red means No Connection\n" +
                "Green means You have Connection", pictureBox1, 0, pictureBox1.Height, 1000); // Tooltip lasts 3 seconds
        }

        private void Key_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {            
            System.Diagnostics.Process.Start("https://github.com/mahziyar-azz/keyword-Link-collector");
        }
    }
}
