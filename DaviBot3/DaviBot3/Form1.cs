using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DaviBot3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
               

        private async void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            toolStripStatusLabel3.Text = "Waiting...";
            Application.DoEvents();
            Task<string> t = getResponse();
            richTextBox1.Text = await t;
            Cursor.Current = Cursors.Default;
            toolStripStatusLabel3.Text = "None";
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Choice
        {
            public string text { get; set; }
            public int index { get; set; }
            public object logprobs { get; set; }
            public string finish_reason { get; set; }
        }

        public class Root
        {
            public string id { get; set; }
            public string @object { get; set; }
            public int created { get; set; }
            public string model { get; set; }
            public List<Choice> choices { get; set; }
            public Usage usage { get; set; }
        }

        public class Usage
        {
            public int prompt_tokens { get; set; }
            public int completion_tokens { get; set; }
            public int total_tokens { get; set; }
        }


        async private Task<string> getResponse()
        {

            string apiKey = Properties.Settings.Default.APIKEY;
            string endpointUrl = "https://api.openai.com/v1/completions";
            string question = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("Please Enter an API Key to use this program\n\nFile -> Add API Key", "DaviBot3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }

            if (string.IsNullOrEmpty(question))
            {
                MessageBox.Show("Please Ask a Question, It cannot be blank", "DaviBot3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }
           
            string requestBody = "{\"model\": \"text-davinci-003\", \"prompt\": \"-HOLDER-\", \"temperature\": 0, \"max_tokens\": 4000}".Replace("-HOLDER-", question);

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);
            string responseBody = await response.Content.ReadAsStringAsync();

            richTextBox1.Clear();

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseBody);

            return myDeserializedClass.choices[0].text;
        }

        private void aPIKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form fm = new Form2(this);
            fm.Show();

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

       
    }
}
