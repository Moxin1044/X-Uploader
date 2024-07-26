using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace X_Uploader
{
    public class UploadService // 更改类名为UploadService，使其更具描述性  
    {
        // 定义upload_check函数  
        public async Task<string> UploadCheckAsync(string apikey, string sandboxtype, string filedir, string filename, string url)
        {
            using (var client = new HttpClient())
            using (var content = new MultipartFormDataContent())
            {
                // 添加表单字段  
                content.Add(new StringContent(apikey, Encoding.UTF8, "text/plain"), "apikey");
                content.Add(new StringContent(sandboxtype, Encoding.UTF8, "text/plain"), "sandbox_type");

                // 注意：这里我没有添加runTime，因为原始问题中没有提到它  

                // 添加文件  
                var fileStream = File.OpenRead(Path.Combine(filedir, filename));
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = filename
                };
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                content.Add(fileContent, "file", filename);

                // 发送POST请求  
                HttpResponseMessage response = await client.PostAsync(url, content);

                // 确保请求成功  
                response.EnsureSuccessStatusCode();

                // 读取响应内容  
                string responseContent = await response.Content.ReadAsStringAsync();

                // 返回响应的字符串内容  
                return responseContent;
            }
        }
    }
}