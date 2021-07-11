using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CypherExample;
using Gabriel.Cat.S.Check;

namespace CheckPuya.Net
{
    public class Capitulo:IFileMega
    {
        public string Name { get; set; }
        public Uri Page { get; set; }
        public Uri Picture { get; set; }
        public Uri Mega1080 { get; set; }
        public Uri Mega720 { get; set; }
        public string[] GetLinksMega() => new string[] {Mega1080.GetLinkMega().AbsoluteUri,Mega720.GetLinkMega().AbsoluteUri };

        public static IEnumerable<Capitulo> GetCapitulos(Uri webPuya)
        {
            throw new NotImplementedException();
        }


    }
    public static class ExtensionCapitulo
    {
        public static Uri GetLinkMega(this Uri linkMega)
        {
            return new Uri(linkMega.DecryptUri((html)=> {
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
