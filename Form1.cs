using Newtonsoft.Json;
using System.Security.Policy;
using X_Uploader; // ȷ�����������ռ� 

namespace X_Uploader
{
    
    public partial class Form1 : Form
    {
        private UploadService uploadService = new UploadService(); // ����UploadService��ʵ��

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            textBox2.Multiline = true;
        }

        private void aPI����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ʵ����Form2  
            Form2 form2 = new Form2();

            // ��ʾForm2  
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // ��ʾ�ļ��Ի���  
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // �û�ѡ����һ���ļ�������·������ΪtextBox1��Text����  
                    textBox1.Text = openFileDialog.FileName;
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 1;
            // ��ȡ INI �� apikey
            string iniFilePath = Path.Combine(Application.StartupPath, "x_uploader.ini");
            string apiKeyValue = string.Empty; // ��ʼ��Ϊ���ַ���  
            progressBar1.Value = 5;
            // ����ļ��Ƿ����  
            if (File.Exists(iniFilePath))
            {
                // ��ȡINI�ļ�����  
                string[] lines = File.ReadAllLines(iniFilePath);
                progressBar1.Value = 15;
                // ����ÿһ�У�����apikey��  
                foreach (var line in lines)
                {
                    // ���Կ��л�ע���У�������;��#��ͷ����Ϊע�ͣ�  
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(";") && !line.StartsWith("#"))
                    {
                        // ����Ƿ�ƥ��apikey��  
                        if (line.Trim().StartsWith("apikey="))
                        {
                            // ��ȡapikey��ֵ  
                            apiKeyValue = line.Split('=')[1].Trim();
                            break; // �ҵ����˳�ѭ��  
                        }
                    }
                }
                progressBar1.Value = 30;
            }
            // �ж�apikey�Ƿ�Ϊ��
            if (string.IsNullOrWhiteSpace(apiKeyValue))
            {
                // ���apiKeyValueΪ�ջ�ֻ�����հ��ַ�  
                MessageBox.Show("API��ԿΪ�գ��봴��������API��Կ��");
            }
            else
            {
                progressBar1.Value = 40;
                // TODO: ִ���ϴ��ļ����Ĵ���
                string api_url = "https://api.threatbook.cn/v3/file/upload";
                // ��ȡcomboBox1��ǰѡ�е�����
                string sandbox_type = comboBox1.SelectedItem.ToString(); // sandbox_type ɳ�����л������û�����ָ���ļ���ɳ�����л���
                progressBar1.Value = 45;
                // AI �ļ��ϴ�������Python Demo�ĸ�д��
                // string fileDir = @"����ļ�Ŀ¼";
                // string fileName = "����ļ���.exe";
                string fullPath = textBox1.Text;
                string fileDir = Path.GetDirectoryName(fullPath);
                string fileName = Path.GetFileName(fullPath);
                progressBar1.Value = 50;
                try
                {
                    string response = await uploadService.UploadCheckAsync(apiKeyValue, sandbox_type, fileDir, fileName, api_url);
                    progressBar1.Value = 100;
                    // ��Ҫ��await��ͬʱ����progressBar1�Ľ��ȡ�
                    // MessageBox.Show(response); // ��ʾ��Ӧ���

                    UploadResponse uploadResponse = JsonConvert.DeserializeObject<UploadResponse>(response);

                    if (uploadResponse != null)
                    {

                        if (uploadResponse.verbose_msg == "OK")
                        {
                            string text = "�ļ�����" + fileName + "\r\n" + "SHA256��" + uploadResponse.Data.Sha256 + "\r\n" + "������ַ��" + uploadResponse.Data.Permalink + "\r\n";
                            // textBox2.Text = text;
                            textBox2.AppendText(text);
                        }
                        else
                        {
                            MessageBox.Show("�����쳣��");
                            textBox2.AppendText("�����쳣��\r\n �쳣�ļ�����" + fileName + response + "\r\n");
                        }
                    }

                    progressBar1.Value = 0;
                }
                catch (Exception ex)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show($"��������: {ex.Message}"); // ��ʾ������Ϣ  
                }
            }
        }

        // �����ں�����ȡresponse������
        public class UploadResponse
        {
            public ResponseData Data { get; set; }
            public int response_code { get; set; }
            public string verbose_msg { get; set; }

            public class ResponseData
            {
                public string Sha256 { get; set; }
                public string Permalink { get; set; }
            }
        }
    }
}
