﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LevelUpCSharp.Helpers;
using LevelUpCSharp.Production.Ingredients;
using LevelUpCSharp.Products;

namespace LevelUpCSharp.Production
{
	public class Vendor
	{
		private static readonly Random Rand;

		private readonly ConcurrentQueue<Sandwich> _warehouse = new ConcurrentQueue<Sandwich>();
		private readonly List<PendingOrder> _orders = new List<PendingOrder>();
		private readonly Thread _production;

		static Vendor()
		{
			Rand = new Random((int) DateTime.Now.Ticks);
		}

		public Vendor(string name)
		{
			Name = name;
			_production = new Thread(DoProduction) {IsBackground = true};
			_production.Start();
		}

		public event Action<Sandwich[]> Produced;

		public string Name { get; }

		public IEnumerable<Sandwich> Buy(int howMuch = 0)
		{
			howMuch = howMuch == 0 ? _warehouse.Count : howMuch;
			int index = 0;
			var toSell = new List<Sandwich>();
			while (_warehouse.TryDequeue(out var sandwich) && howMuch > index)
			{
				toSell.Add(sandwich);
				index++;
			}

			return toSell;
		}

        public IEnumerable<StockItem> GetStock()
        {
            Dictionary<SandwichKind, int> counts = new Dictionary<SandwichKind, int>()
            {
                {SandwichKind.Cheese, 0},
                {SandwichKind.Chicken, 0},
                {SandwichKind.Beef, 0},
                {SandwichKind.Pork, 0},
            };


            var stock = _warehouse.ToArray();
            var result = stock
	            .GroupBy(
		            sandwich => sandwich.Kind,
		            (kind, sandwiches) => new StockItem(kind, sandwiches.Count()))
	            .ToArray();

			return result;
        }

        private Sandwich Produce(SandwichKind kind)
        {
            return kind switch
            {
                SandwichKind.Beef => ProduceSandwich(new Beef(), DateTimeOffset.Now.AddMinutes(3)),
                SandwichKind.Cheese => ProduceSandwich(new Cheese(), DateTimeOffset.Now.AddSeconds(90)),
                SandwichKind.Chicken => ProduceSandwich(new Chicken(), DateTimeOffset.Now.AddMinutes(4)),
                SandwichKind.Pork => ProduceSandwich(new Pork(), DateTimeOffset.Now.AddSeconds(150)),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }

        private Sandwich ProduceSandwich(IKeyIngredient kind, DateTimeOffset addMinutes)
        {
	        return SandwichBuilder.WithButter(false)
		        .Use(kind)
		        .AddVeg(new Tomato())
		        .AddVeg(new Tomato())
		        .AddVeg(new Tomato())
		        .AddVeg(new Olives())
		        .Wrap();
        }

        public void Order(SandwichKind kind, int count)
        {
			lock (_orders)
			{
				_orders.Add(new PendingOrder(count, kind)); 
			}
        } 

		private void DoProduction(object obj)
        {
	        SandwichKind[] sandwichKinds = EnumHelper.GetValues<SandwichKind>();

	        while (true)
	        {
		        var kind = Rand.Next(sandwichKinds.Length);

		        var sandwich = Produce((SandwichKind)kind);

		        _warehouse.Enqueue(sandwich);

		        Produced?.Invoke(new[] { sandwich });

		        IEnumerable<PendingOrder> sideWork;
		        lock (_orders)
		        {
			        sideWork = _orders.ToArray();
			        _orders.Clear();
		        }

		        foreach (var pendingOrder in sideWork)
		        {
			        var sandwiches = new List<Sandwich>();

			        for (int i = 0; i < pendingOrder.Amount; i++)
			        {
				        sandwich = Produce(pendingOrder.Kind);

				        _warehouse.Enqueue(sandwich);

				        sandwiches.Add(sandwich);
				        Thread.Sleep(1 * 500);
			        }

			        Produced?.Invoke(sandwiches.ToArray());

			        Thread.Sleep(1 * 500);
		        }

		        Thread.Sleep(5 * 1000);
	        }
        }

        private class PendingOrder
        {
	        public PendingOrder(int amount, SandwichKind kind)
	        {
		        Amount = amount;
		        Kind = kind;
	        }

	        public int Amount { get; }

	        public SandwichKind Kind { get; }
        }
	}
}
