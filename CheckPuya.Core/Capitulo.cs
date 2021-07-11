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
        public IEnumerable<Link> GetLinks() => new Link[] { new Link() { TextoAntes = "1080p ", Url = Mega1080.GetLinkMega().AbsoluteUri },new Link(){TextoAntes= "720p " ,Url= Mega720.GetLinkMega().AbsoluteUri } };

        public static IEnumerable<Capitulo> GetCapitulos(Uri webPuya)
        {
            string html = webPuya.DownloadString();
            HtmlDocument doc = new HtmlDocument().LoadString(html);
            return doc.GetElementbyId("content").GetByTagName("article").Select(nodoArticle =>
            {
                int indexAnd;
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
                capitulo.Mega1080 = new Uri(nodosLinksArray[1].GetByTagName("a").Last().Attributes["href"].Value);
                capitulo.Mega720 = new Uri(nodosLinksArray[2].GetByTagName("a").Last().Attributes["href"].Value);
                return capitulo;
            });

        }


    }
    public static class ExtensionCapitulo
    {
        public static Uri GetLinkMega(this Uri linkMega)
        {
            return new Uri(linkMega.DecryptUri((html) =>
            {
                string data;
                Regex regex = new Regex("(<INPUT[^>]*>)");
                Match match = regex.Match(html).NextMatch();//quito el primero que es el source
                string key = match.Value.Split('\'')[1];
                match = match.NextMatch();
                data = match.Value.Split("VALUE=\"")[1].Replace("\">", "");
                return (key, data);
            })[0]);
        }
    }
}
