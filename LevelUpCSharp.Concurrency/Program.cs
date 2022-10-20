using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace LevelUpCSharp.Concurrency
{
    class Program
    {
        private static Random r = new Random();

        static void Main(string[] args)
        {
	        //TasksDemo();
	        //return;
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

        private static async void TasksDemo()
        {
	        int value = await DoWork();
	        var vaulue2 = DoWork2();
            var x = value + vaulue2;

        }

        private static int DoWork2()
        {
            Thread.Sleep(7 * 1000);
	        return 8;
        }

        private static void TasksDemo2()
        {
	        Task<int> valueTask = DoWork();
	        var vaulue2 = DoWork2();
	        var value = valueTask.Result;
            var x = value + vaulue2;
        }

        private static async Task<int> DoWork()
        {
	        return await Task.Run(() => (int) 5);
        }
    }
}
