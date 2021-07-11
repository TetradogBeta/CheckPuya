using System;
using System.Text;
using CypherExample;
using Gabriel.Cat.S.Check;

namespace CheckPuya.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            Check check = new Check();
            check.Load(args);
            check.Publicar(() =>Capitulo.GetCapitulos(check.Web));

        }

    }
}
