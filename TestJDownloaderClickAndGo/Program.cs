using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Gabriel.Cat.S.Extension;

namespace TestJDownloaderClickAndGo
{
    partial class Program
    {
        static void Main(string[] args)
        {
            JDownloaderListener listener = new JDownloaderListener();
            listener.Start();
            listener.ReadOne().Wait();

            listener.Stop();
        }
     

    }
}
