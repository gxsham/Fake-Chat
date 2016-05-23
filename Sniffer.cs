using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace lab5PR
{
	class Sniffer
	{

		Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.137"), 8167);
		byte[] buffer;
		public Sniffer()
		{
			socket.Bind(endpoint);
			buffer = new byte[65565];
		}

		public Message GetPacket()
		{
			var length = socket.Receive(buffer);
			return filter(buffer.Take(length).ToArray());
		}

		public Message filter(byte[] array)
		{			
			string Msg = System.Text.Encoding.Default.GetString(array);
			Match match = Regex.Match(Msg, "(.+?)\\0(.+?)\\0(.*)\\0");
				if (match.Success)
				{
					if (match.Groups[1].Value[0].Equals('2'))
					{
						return new Message
						{
							Hour = DateTime.Now.ToString("HH:mm:ss"),
							From = match.Groups[2].Value,
							To = match.Groups[1].Value.Substring(2),
							Type = "C",
							Text = match.Groups[3].Value
						};
					}
					else
					{
						return new Message
						{
							Hour = DateTime.Now.ToString("HH:mm:ss"),
							From = match.Groups[1].Value.Substring(1),
							To = match.Groups[2].Value,
							Type = "P",
							Text = match.Groups[3].Value
						};
					}
					
				}
				return null;
		}
		
		public async Task<Message> getAsync() => await Task.Run(() => GetPacket());

	}
}
