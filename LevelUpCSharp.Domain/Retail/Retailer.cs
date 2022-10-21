using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using LevelUpCSharp.Collections;
using LevelUpCSharp.Products;
using Newtonsoft.Json;

namespace LevelUpCSharp.Retail
{
    public class Retailer
    {
        private static Retailer _instance;
        private readonly ISandwichesRack<SandwichKind, Sandwich> _shelf;

        protected Retailer(string name)
        {
            Name = name;
            _shelf = new Rack();
        }

        public Retailer(string name, IEnumerable<Sandwich> firstPackage)
		{
			Name = name;
			_shelf = new Rack(firstPackage);
        }

        public static Retailer Instance => _instance ?? (_instance = new Retailer("Build-in"));

        public event Action<PackingSummary> Packed;
        public event Action<DateTimeOffset, Sandwich> Purchase;

        public string Name { get; }

        public Result<Sandwich> Sell(SandwichKind kind)
        {
            return SellImpl(kind);
        }

        public Task<Result<Sandwich>> SellAsync(SandwichKind kind)
        {
	        return Task.Run(() => SellImpl(kind));
        }

        public void Pack(IEnumerable<Sandwich> package, string deliver)
        {
	        var summary = PackImpl(package, deliver);
	        OnPacked(summary);
        }
        
        public Task PackAsync(IEnumerable<Sandwich> package, string deliver)
        {
	        return Task.Run(() =>
	        {
		        var summary = PackImpl(package, deliver);
		        OnPacked(summary);
	        });
        }

        public void Pickup()
        {
	        try
	        {
		        IEnumerable<Sandwich> sandwiches;

		        using (var connection = BuildConnection())
		        {
			        using (var stream = connection.GetStream())
			        {
				        SendCommand(stream);

				        sandwiches = ReadResponse(stream);
			        }
		        }

		        Pack(sandwiches, "remote");

	        }
	        catch (SocketException)
	        {
	        }
        }

        protected virtual void OnPacked(PackingSummary summary)
        {
            Packed?.Invoke(summary);
        }

        protected virtual void OnPurchase(DateTimeOffset time, Sandwich product)
        {
            Purchase?.Invoke(time, product);
        }

        private void PopulateRack(IEnumerable<Sandwich> package)
        {
	        package.ForEach(p =>
	        {
		        lock (_shelf)
		        {
			        _shelf.Add(p);
		        }
	        });
        }

        private static PackingSummary ComputeReport(IEnumerable<Sandwich> package, string deliver)
        {
	        var summaryPositions = package
		        .GroupBy(
			        p => p.Kind,
			        (kind, sandwiches) => new LineSummary(kind, sandwiches.Count()))
		        .ToArray();

	        var summary = new PackingSummary(summaryPositions, deliver);
	        return summary;
        }

        private Result<Sandwich> SellImpl(SandwichKind kind)
        {
	        Sandwich sandwich;
	        lock (_shelf)
	        {
		        var dontHave = _shelf.Contains(kind);
		        if (dontHave)
		        {
			        return Result<Sandwich>.Failed();
		        }

		        sandwich = _shelf.Get(kind);
            }

	        OnPurchase(DateTimeOffset.Now, sandwich);

	        return sandwich.AsSuccess();
        }

        private PackingSummary PackImpl(IEnumerable<Sandwich> package, string deliver)
        {
	        package = package.ToArray();
	        PopulateRack(package);
	        return ComputeReport(package, deliver);
        }

        #region networking
        private TcpClient BuildConnection()
        {
	        TcpClient client = new TcpClient();
	        client.Connect("localhost", 13000);
	        return client;
        }

        private IEnumerable<Sandwich> ReadResponse(NetworkStream stream)
        {
	        using (var sr = new StreamReader(stream))
	        {
		        using (var jsonReader = new JsonTextReader(sr))
		        {
			        return new JsonSerializer().Deserialize<IEnumerable<Sandwich>>(jsonReader);
		        }
	        }
        }

        private void SendCommand(NetworkStream stream)
        {
	        var data = System.Text.Encoding.ASCII.GetBytes("v.s");
	        stream.Write(data, 0, data.Length);
        }
        #endregion
    }
}
