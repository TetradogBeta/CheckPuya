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
            const string VERISON = "1.2";
            Cancelation cancelation = new Cancelation();
            Check check = new Check();
            Task tSalir = new Task(new Action(()=> { Console.ReadLine();cancelation.Continue = false; }));
            check.Load(args);
            check.Log($"CheckPuya V{VERISON} Telegram Bot!");
            tSalir.Start();

            check.Publicar((web) =>Capitulo.GetCapitulos(web),cancelation:cancelation);

        }

    }
}
