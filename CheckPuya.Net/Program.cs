using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CypherExample;
using Gabriel.Cat.S.Check;

namespace CheckPuya.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            const string VERISON = "2.0";
            Cancelation cancelation = new Cancelation();
            Check check = new Check();
            Task tSalir = new Task(new Action(()=> { Console.ReadLine();cancelation.Continue = false; }));
            check.Load(args);
            check.Log($"CheckPuya V{VERISON} Telegram Bot!");
            tSalir.Start();
            try
            {
                check.Publicar(GetCapitulos, cancelation: cancelation).Wait();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            Console.ReadLine();

        }

        private static async Task<IEnumerable<IFile>> GetCapitulos(Uri uriWeb)
        {
             return (await Capitulo.GetCapitulos(uriWeb)).Where(c=>c.IsValido); 
        }
    }
}
