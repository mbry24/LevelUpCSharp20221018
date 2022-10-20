using System;
using System.Collections.Generic;
using System.Threading;

namespace LevelUpCSharp.Concurrency
{
    class Program
    {
        private static Random r = new Random();

        static void Main(string[] args)
        {
            var a = new Thread(WorkA);
            var b = new Thread(WorkB);
            var vault = new Vault<int>();

            a.Start(vault);
            b.Start(vault);

            Console.ReadKey(true);
            Console.WriteLine("Closing...");
        }

        private static void WorkB(object? obj)
        {
	        var vault = (Vault<int>) obj;

	        while (true)
	        {
		        var found = r.Next(100);
		        
		        Console.WriteLine("[B] i have: " + found);
		        vault.Put(found);
		        Console.WriteLine("[B] stored: " + found);
	        }
        }

        private static void WorkA(object? obj)
        {
	        var vault = (Vault<int>) obj;

	        while (true)
	        {
		        Console.WriteLine("[A] get attempt");
		        var get = vault.Get();
		        Console.WriteLine("[A] get:" + get);
	        }
        }
    }
}
