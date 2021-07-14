using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JDownloaderClickAndGo
{
    public delegate byte[] ResponseDelegate(string links);
    public class JDownloaderListener
    {

        public JDownloaderListener()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add("http://127.0.0.1:9666/flash/addcrypted2/");
            GetResponse = (links) =>
            {
                string html;
                StringBuilder items = new StringBuilder();

                links = links.Trim('\0');
                links = links.Trim(' ');
                if (links.Contains('\n'))
                    foreach (string link in links.Split('\n'))
                    {
                        items.Append($"<li><a href=\"{link}\">{link}</a></li>");
                    }
                else items.Append($"<li><a href=\"{links}\">{links}</a></li>");

                html = $"<html><body><h1>Links</h1><ul>{items}<ul></body></html>";
                return Encoding.UTF8.GetBytes(html);


            };
        }
        public ResponseDelegate GetResponse { get; set; }
        HttpListener Listener { get; set; }

        public void Start() => Listener.Start();
        public void Stop() => Listener.Stop();
        public async Task ReadOne()
        {
            HttpListenerContext context;
            NameValueCollection nameValueCollection;
            byte[] data;
            byte[] buffer = new byte[1024];
            string jk, crypted;
            context = await Listener.GetContextAsync();
            context.Request.InputStream.Read(buffer);
            nameValueCollection = HttpUtility.ParseQueryString(Encoding.UTF8.GetString(buffer));
            crypted = nameValueCollection["crypted"];
            jk = nameValueCollection["jk"].Split('\'')[1];
            data = GetResponse(JDownloadClickNLoad.Decrypt(crypted, jk));
            await context.Response.OutputStream.WriteAsync(data, 0, data.Length);
            context.Response.Close();
        }
    }


}
