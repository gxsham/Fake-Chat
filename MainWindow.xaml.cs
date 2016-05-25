using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Net;

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
			string pattern = "D" + Old_name.Text + "\000";
			send_pattern(pattern);
		}

		private void DND_Status_Checked(object sender, RoutedEventArgs e)
		{
			string pattern = "D" + Old_name.Text + "\010";
			send_pattern(pattern);
		}

		private void Away_Status_Checked(object sender, RoutedEventArgs e)
		{
			string pattern = "D"+ Old_name.Text + "\020";
			send_pattern(pattern); 
		}
		private void Offline_Status_Checked(object sender, RoutedEventArgs e)
		{
			string pattern = "D" + Old_name.Text + "\030";
			send_pattern(pattern);
		}

		private void send_pattern(string pattern)
		{
			byte[] send_buffer = Encoding.ASCII.GetBytes(pattern);
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
			if (e.Key == Key.Enter && Topic.Text!="")
			{
				string pattern = "B" + Topic.Text + "\0";
				send_pattern(pattern);
			}
		}

		private void Old_name_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && Old_name.Text != "")
			{
				global_name.Add(Old_name.Text);
				string pattern = "3" + global_name.ElementAt(0) + "\0" + global_name.ElementAt(1) + "\00";
				send_pattern(pattern);
				global_name.RemoveAt(0);
			}
		}

		private void InputField_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter && InputField.Text != "")
			{
				string pattern = actual_pattern();
				send_pattern(pattern);
				InputField.Clear();
			}
		}

		private string actual_pattern()
		{
			if(Private_radio.IsChecked==true)
			{
				return "6"+private_my_name.Text+"\0"+private_send_to.Text+"\0"+InputField.Text+"\0";
			}
			else if (Channel_radio.IsChecked==true)
			{
				return "2#"+Channel_name.Text+"\0" + Old_name.Text + "\0" + InputField.Text+"\0";
			}
			else
			{
				return "2#Main\0" + Old_name.Text + "\0" + InputField.Text+"\0";
			}
		}

		private void Button_Join(object sender, RoutedEventArgs e)
		{
			string pattern = "4" + Old_name.Text + "\0#" + Channel_name.Text + "\030";
			send_pattern(pattern);
		}

		private void Button_Leave(object sender, RoutedEventArgs e)
		{
			string pattern = "5" + Old_name.Text + "\0#" + Channel_name.Text + "\00";
			send_pattern(pattern);
		}

		
	}
}
