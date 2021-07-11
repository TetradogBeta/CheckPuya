using System;
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
            const string VERISON = "1.0";
            Cancelation cancelation = new Cancelation();
            Check check = new Check();
            Task tSalir = new Task(new Action(()=> { Console.ReadLine();cancelation.Continue = false; }));
            check.Load(args);
            Console.WriteLine($"CheckPuya V{VERISON} Telegram Bot!");
            tSalir.Start();

            check.Publicar(() =>Capitulo.GetCapitulos(check.Web),cancelation:cancelation);

        }

    }
}
