using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace X_Uploader
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "https://x.threatbook.com/v5/serviceCenter?tab=myKey";
            try
            {
                // 使用默认浏览器打开URL  
                Process.Start(url);
            }
            catch
            {
                // 处理无法打开URL的情况（例如，没有默认浏览器）  
                MessageBox.Show("无法打开默认浏览器以访问链接。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string iniFilePath = Path.Combine(Application.StartupPath, "x_uploader.ini");

            // 检查文件是否存在  
            if (!File.Exists(iniFilePath))
            {
                // 文件不存在，创建文件并写入apikey项  
                using (StreamWriter writer = new StreamWriter(iniFilePath, false, Encoding.UTF8))
                {
                    writer.WriteLine("apikey=" + textBox1.Text);
                }
                MessageBox.Show("设置成功");
                // textBox1.Text = "已创建并设置apikey"; // 可选：更新UI以反映更改  
            }
            else
            {
                // 文件存在，读取内容  
                bool apiKeyExists = false;
                string[] lines = File.ReadAllLines(iniFilePath);
                StringBuilder sb = new StringBuilder();

                foreach (var line in lines)
                {
                    // 跳过空行或注释行（假设以;或#开头的行为注释）  
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(";") && !line.StartsWith("#"))
                    {
                        if (line.Trim().StartsWith("apikey="))
                        {
                            // apikey项已存在，更新其值  
                            apiKeyExists = true;
                            sb.AppendLine("apikey=" + textBox1.Text);
                        }
                        else
                        {
                            // 保留其他行  
                            sb.AppendLine(line);
                        }
                    }
                }

                if (!apiKeyExists)
                {
                    // apikey项不存在，添加到文件末尾  
                    sb.AppendLine("apikey=" + textBox1.Text);
                }

                // 写入更新后的内容到文件  
                File.WriteAllText(iniFilePath, sb.ToString(), Encoding.UTF8);

                // 可选：更新UI以反映更改（实际上，这里不需要，因为textBox1的内容已经是新的apikey了）  
                // 但如果你希望用户看到文件中的确切内容，你可能需要再次读取并显示它（尽管它是多余的）  
                // 或者简单地通知用户apikey已更新  
                // textBox1.Text = "apikey已更新"; // 或者保持为新的apikey值  
                MessageBox.Show("设置成功");
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string iniFilePath = Path.Combine(Application.StartupPath, "x_uploader.ini");
            string apiKeyValue = string.Empty; // 初始化为空字符串  

            // 检查文件是否存在  
            if (File.Exists(iniFilePath))
            {
                // 读取INI文件内容  
                string[] lines = File.ReadAllLines(iniFilePath);

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
            }

            // 将apikey的值设置到textBox1中  
            textBox1.Text = apiKeyValue;
        }
    }
}
