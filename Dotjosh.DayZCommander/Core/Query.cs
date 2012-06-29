using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Dotjosh.DayZCommander.Core
{
	public class Query
	{
		private readonly string _ipAddress;
		private IPEndPoint _ipEndPoint;
		private const int Port = 2302;

		public Query(string ipAddress)
		{
			_ipAddress = ipAddress;
			_ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
		}

		public Server Execute()
		{
			using(var client = new UdpClient(_ipEndPoint))
			{
				client.Client.ReceiveTimeout = 1000*10;
				client.Client.SendTimeout = 1000*10;

				client.Connect(_ipAddress, Port);
				var challengePacket = new byte[] {0xFE, 0xFD, 0x09};
				var basePacket = new byte[] {0xFE, 0xFD, 0x00};
				var idPacket = new byte[] {0x04, 0x05, 0x06, 0x07};
				var fullInfoPacket = new byte[] {0xFF, 0xFF, 0xFF, 0x01};
//
				var firstRequestPacket = challengePacket.Concat(idPacket).ToArray();
//				var firstRequestPacket = StringToByteArray("fe fd 09 00 00 00 00");
				client.Send(firstRequestPacket, firstRequestPacket.Length);

				var challengeResponse = client.Receive(ref _ipEndPoint);
				var firstResponse = challengeResponse[0];

				challengePacket = challengeResponse.Skip(5).ToArray();
				var challengeString = System.Text.Encoding.ASCII.GetString(challengePacket);

				challengePacket = BitConverter.GetBytes(Convert.ToInt32(challengeString)).Reverse().ToArray();
				var secondPacket = basePacket.Concat(idPacket).Concat(challengePacket).Concat(fullInfoPacket).ToArray();
				client.Send(secondPacket, secondPacket.Length);
				var reply = client.Receive(ref _ipEndPoint);
				var response = System.Text.Encoding.ASCII.GetString(reply.Skip(16).ToArray());
				var items = response.Split(new [] {"\0" }, StringSplitOptions.None);

				var settings = new SortedDictionary<string, string>();
				for (int index = 0; index < items.Length; index++)
				{
					if(index == 60)
						break;
					var name = items[index];
					var value = items[index+1];

					settings.Add(name, value);
					index++;
				}

				var players = items
								.Skip(61)
								.TakeWhile(x => x != "team_" && x != "")
								.Select(x => new Player{ Name = x })
								.ToList();

				var scores = items.SkipWhile(x => x != "score_")
					.Skip(2)
					.TakeWhile(x => x != "")
					.ToList();


				var deaths = items.SkipWhile(x => x != "deaths_")
					.Skip(2)
					.TakeWhile(x => x != "")
					.ToList();			


				for (int index = 0; index < players.Count; index++)
				{
					var player = players[index];
					player.Score = int.Parse(scores.ElementAt(index));
					player.Deaths = int.Parse(deaths.ElementAt(index));
				}

				return new Server(players, settings);
			}
		}

		public class Server
		{
			public Server(List<Player> players, SortedDictionary<string, string> settings)
			{
				Settings = settings;
				Players = new ObservableCollection<Player>(players);
			}

			public string Name { get { return Settings["hostname"]; }}

			public SortedDictionary<string, string> Settings { get; private set; } 
			public ObservableCollection<Player> Players { get; private set; }
		}

		public class Player
		{
			public string Name { get; set; }
			public int Score { get; set; }
			public int Deaths { get; set; }
		}
	}
}