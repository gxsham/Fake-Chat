using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Net;
using System.Windows.Controls.Primitives;

namespace lab5PR
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ObservableCollection<Message> list = new ObservableCollection<Message>();

		Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.255"), 8167);

		List<string> global_name = new List<string>();

		public MainWindow()
		{
			InitializeComponent();
			this.Loaded += MainWindow_Loaded;
			global_name.Add("anonymous");
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Sniffer sniff = new Sniffer();
			list_view.ItemsSource = list;
			while (true)
			{
				var sniffed_messages = await sniff.getAsync();
				if(sniffed_messages!=null)
				{
					list.Add(sniffed_messages);
				}
			}
		}

		private void Normal_Status_Checked(object sender, RoutedEventArgs e)
		{
			string name = Old_name.Text;
			string code = "\000";
			change_status(name,code);
		}

		private void DND_Status_Checked(object sender, RoutedEventArgs e)
		{
			string name = Old_name.Text;
			string code = "\010";
			change_status(name,code);
		}

		private void Away_Status_Checked(object sender, RoutedEventArgs e)
		{
			string name = Old_name.Text;
			string code = "\020";
			change_status(name,code); 

		}

		private void change_status(string name,string code)
		{
			string text = "D" + name + code;
			byte[] send_buffer = Encoding.ASCII.GetBytes(text);
			sock.SendTo(send_buffer, endPoint);
		}

		private void RadioButton_Checked(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Visible;
			Private.Visibility = Visibility.Collapsed;
			Channel.Visibility = Visibility.Collapsed;
		}

		private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Collapsed;
			Private.Visibility = Visibility.Visible;
			Channel.Visibility = Visibility.Collapsed;
		}

		private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
		{
			Normal.Visibility = Visibility.Collapsed;
			Private.Visibility = Visibility.Collapsed;
			Channel.Visibility = Visibility.Visible;
		}

		private void Topic_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				sock.SendTo(Encoding.ASCII.GetBytes("B" + Topic.Text + "\0"), endPoint);
			}
		}

		private void Old_name_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				global_name.Add(Old_name.Text);
				string text = "3" + global_name.ElementAt(0) + "\0" + global_name.ElementAt(1) + "\00";
				byte[] send_buffer = Encoding.ASCII.GetBytes(text);
				sock.SendTo(send_buffer, endPoint);
				global_name.RemoveAt(0);
			}
		}

		private void New_Name_Click(object sender, RoutedEventArgs e)
		{

		}



		//private void Status_name_KeyDown(object sender, KeyEventArgs e)
		//{
		//	string name = Status_name.Text;
		//	string text = "3" + name + "\0" + "newnameblin" + "\00";
		//	byte[] send_buffer = Encoding.ASCII.GetBytes(text);
		//	sock.SendTo(send_buffer, endPoint);
		//}
	}
}
