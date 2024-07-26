using Newtonsoft.Json;
using System.Security.Policy;
using X_Uploader; // 确保包含命名空间 

namespace X_Uploader
{
    
    public partial class Form1 : Form
    {
        private UploadService uploadService = new UploadService(); // 创建UploadService的实例

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            textBox2.Multiline = true;
        }

        private void aPI设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 实例化Form2  
            Form2 form2 = new Form2();

            // 显示Form2  
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // 显示文件对话框  
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 用户选择了一个文件，将其路径设置为textBox1的Text属性  
                    textBox1.Text = openFileDialog.FileName;
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 1;
            // 读取 INI 中 apikey
            string iniFilePath = Path.Combine(Application.StartupPath, "x_uploader.ini");
            string apiKeyValue = string.Empty; // 初始化为空字符串  
            progressBar1.Value = 5;
            // 检查文件是否存在  
            if (File.Exists(iniFilePath))
            {
                // 读取INI文件内容  
                string[] lines = File.ReadAllLines(iniFilePath);
                progressBar1.Value = 15;
                // 遍历每一行，查找apikey项  
                foreach (var line in lines)
                {
                    // 忽略空行或注释行（假设以;或#开头的行为注释）  
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(";") && !line.StartsWith("#"))
                    {
                        // 检查是否匹配apikey项  
                        if (line.Trim().StartsWith("apikey="))
                        {
                            // 提取apikey的值  
                            apiKeyValue = line.Split('=')[1].Trim();
                            break; // 找到后退出循环  
                        }
                    }
                }
                progressBar1.Value = 30;
            }
            // 判断apikey是否为空
            if (string.IsNullOrWhiteSpace(apiKeyValue))
            {
                // 如果apiKeyValue为空或只包含空白字符  
                MessageBox.Show("API密钥为空，请创建并输入API密钥。");
            }
            else
            {
                progressBar1.Value = 40;
                // TODO: 执行上传文件检测的代码
                string api_url = "https://api.threatbook.cn/v3/file/upload";
                // 获取comboBox1当前选中的内容
                string sandbox_type = comboBox1.SelectedItem.ToString(); // sandbox_type 沙箱运行环境，用户可以指定文件的沙箱运行环境
                progressBar1.Value = 45;
                // AI 文件上传（来自Python Demo的改写）
                // string fileDir = @"你的文件目录";
                // string fileName = "你的文件名.exe";
                string fullPath = textBox1.Text;
                string fileDir = Path.GetDirectoryName(fullPath);
                string fileName = Path.GetFileName(fullPath);
                progressBar1.Value = 50;
                try
                {
                    string response = await uploadService.UploadCheckAsync(apiKeyValue, sandbox_type, fileDir, fileName, api_url);
                    progressBar1.Value = 100;
                    // 需要在await的同时设置progressBar1的进度。
                    // MessageBox.Show(response); // 显示响应结果

                    UploadResponse uploadResponse = JsonConvert.DeserializeObject<UploadResponse>(response);

                    if (uploadResponse != null)
                    {

                        if (uploadResponse.verbose_msg == "OK")
                        {
                            string text = "文件名：" + fileName + "\r\n" + "SHA256：" + uploadResponse.Data.Sha256 + "\r\n" + "分析地址：" + uploadResponse.Data.Permalink + "\r\n";
                            // textBox2.Text = text;
                            textBox2.AppendText(text);
                        }
                        else
                        {
                            MessageBox.Show("出现异常！");
                            textBox2.AppendText("出现异常！\r\n 异常文件名：" + fileName + response + "\r\n");
                        }
                    }

                    progressBar1.Value = 0;
                }
                catch (Exception ex)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show($"发生错误: {ex.Message}"); // 显示错误信息  
                }
            }
        }

        // 用于在后续截取response的内容
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
