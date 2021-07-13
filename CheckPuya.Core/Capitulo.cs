using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CypherExample;
using Gabriel.Cat.S.Check;
using Gabriel.Cat.S.Extension;
using HtmlAgilityPack;

namespace CheckPuya.Net
{
    public class Capitulo : IFile
    {
        public string Name { get; set; }
        public Uri Page { get; set; }
        public Uri Picture { get; set; }
        public Uri Mega1080 { get; set; }
        public Uri Mega720 { get; set; }
        public bool IsValido=>!Equals(Mega1080,default) || !Equals(Mega720, default);
        public IEnumerable<Link> GetLinks()
        {
            Task<Uri> tLink1080; 
            Task<Uri> tLink720;
            List<Link> links = new List<Link>();

            if (!Equals(Mega1080, default)) {
                tLink1080 = Mega1080.GetLinkMega();
                tLink1080.Wait();
                links.Add(new Link() { TextoAntes = "1080p ", Url = tLink1080.Result.AbsoluteUri.Contains('%') ? tLink1080.Result.AbsoluteUri.Split('%')[0] : tLink1080.Result.AbsoluteUri });
            }
            if (!Equals(Mega720, default))
            {
                tLink720 = Mega720.GetLinkMega();
                tLink720.Wait();
                links.Add(new Link() { TextoAntes = "720p ", Url = tLink720.Result.AbsoluteUri.Contains('%')? tLink720.Result.AbsoluteUri.Split('%')[0]: tLink720.Result.AbsoluteUri });
            }
            return links;
        }
        public static async Task<IEnumerable<Capitulo>> GetCapitulos(Uri webPuya)
        {
            string html = await webPuya.DownloadString();
            HtmlDocument doc = new HtmlDocument().LoadString(html);
            return doc.GetElementbyId("content").GetByTagName("article").Select(nodoArticle =>
            {
                int indexAnd;
                string urlMega1080;
                string urlMega720;
                HtmlNode[] nodosLinksArray;
                Capitulo capitulo = new Capitulo();
                HtmlNode nodoLinks = nodoArticle.GetByClass("entry-content").First().GetByTagName("div").First();
                capitulo.Page = new Uri(nodoArticle.GetByTagName("h2").First().GetByTagName("a").First().Attributes["href"].Value);
                capitulo.Picture = new Uri(nodoArticle.GetByTagName("img").First().Attributes["src"].Value);
                capitulo.Name = nodoArticle.GetByTagName("h2").First().GetByTagName("a").First().InnerText;
                do
                {
                    indexAnd = capitulo.Name.IndexOf('&');
                    if (indexAnd > 0)
                        capitulo.Name = capitulo.Name.Remove(indexAnd, capitulo.Name.IndexOf(';', indexAnd) - indexAnd + 1);
                } while (indexAnd > 0);
                nodosLinksArray = nodoLinks.GetByTagName("div").ToArray();
                if (nodosLinksArray.Length == 3)
                {
                    urlMega1080 = nodosLinksArray[1].GetByTagName("a").Last().Attributes["href"].Value;
                    if (urlMega1080.Contains("/enc/"))
                    {
                        capitulo.Mega1080 = new Uri(urlMega1080);
                    }
                    urlMega720 = nodosLinksArray[2].GetByTagName("a").Last().Attributes["href"].Value;
                    if (urlMega720.Contains("/enc/"))
                    {
                        capitulo.Mega720 = new Uri(urlMega720);
                    }
                }
                return capitulo;
            });

        }


    }
    public static class ExtensionCapitulo
    {
        public static async Task<Uri> GetLinkMega(this Uri linkMega)
        {
            return new Uri((await linkMega.DecryptUri())[0]);
        }
    }
}
