using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows.Threading;

namespace Bejko_SocketAsyncLib
{
	public class AsyncSocketServer
	{
		IPAddress mIP;
		int mPort;
		TcpListener mServer;
		List<TcpClient> mClients;


		public AsyncSocketServer()
		{
			mClients = new List<TcpClient>();
			
		}


		public async void In_Ascolto(IPAddress ipaddr = null, int port = 23000)
		{
			
			//Controllo sull'IP
			if (ipaddr == null)
			{
				ipaddr = IPAddress.Any;
			}

			if (port < 0 || port > 65535)
			{
				port = 23000;
			}

			mIP = ipaddr;
			mPort = port;

			mServer = new TcpListener(mIP, mPort);
			Debug.WriteLine("Server in ascolto su IP: {0} - Porta: {1}", mIP.ToString(), mPort.ToString());

			mServer.Start();

			Debug.WriteLine("Server avviato!");


			DateTime thisDay = DateTime.Today;
			// mi attrezzo per ricevere un messaggio dal client
			// siccome è di tipo stream io riceverò dei byte, o meglio un byte array
			// riceverò anche il numero di byte.
			byte[] buff = new byte[128];
			string bot = "ChatBot :  Benvenuto client\n\r\n\r" +
						 "ChatBot : Se ti server aiuta basta inviare \n\r" +
						 "#CMD  ( Per vederi tutti i commandi )\n\r\n \r" +
						 thisDay.ToString("g") + "\n\r";







			//invio al client il messaggio del benvenuto
			buff = Encoding.ASCII.GetBytes(bot);

			while (true)
			{

				TcpClient client = await mServer.AcceptTcpClientAsync();
				buff = Encoding.ASCII.GetBytes(bot);
				await client.GetStream().WriteAsync(buff, 0, buff.Length);
				mClients.Add(client);

				//Con il comando "client.Client.RemoteEndPoint" ridarà l'IP e la porta del client che si è appena connesso
				Debug.WriteLine("Client connessi: {0}. Client attualmente connesso: {1}", mClients.Count, client.Client.RemoteEndPoint);
				RiceviMessaggio(client);
				
	
			}
		}

		public async void RiceviMessaggio(TcpClient client)
		{

			NetworkStream stream = null;
			StreamReader reader = null;

			try
			{
				stream = client.GetStream();
				reader = new StreamReader(stream);
				char[] buff = new char[512];
				int nBytes = 0;
				DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
				dispatcherTimer.Tick += new EventHandler(Inviaognisec);
				dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
				dispatcherTimer.Start();
				int c = 0;
				while (true)
				{
					
					Debug.WriteLine("In attesa di un messaggio");

					
					//Ricreazione messaggio asincrono
					nBytes = await reader.ReadAsync(buff, 0, buff.Length);
					if (nBytes == 0)
					{
						Debug.WriteLine("Client Disconnesso!");
						break;
					}
                    if (c==1)
                    {
						c = 0;
					}
					else if (nBytes != 0 && c == 0)
					{
						string recvText = new string(buff);

						Debug.WriteLine("N° byte: {0};\nMessaggio: {1}", nBytes, recvText);

						c = InviaATutti(buff, client);

					}




					Array.Clear(buff, 0, buff.Length);

				}
			}
			catch (Exception ex)
			{

				Debug.WriteLine("Errore: " + ex.Message);
			}
		}

		public void RimuoviClient(TcpClient client)
		{
			if (mClients.Contains(client))
			{
				mClients.Remove(client);
			}

		}

		private void Inviaognisec(object sender, EventArgs e)
		{
			try
			{
				foreach (TcpClient client in mClients)
				{
					DateTime thisDay = DateTime.Today;
					byte[] buff = Encoding.ASCII.GetBytes("\r\n" + thisDay.ToString("g") + " \r\n");
					client.GetStream().WriteAsync(buff, 0, buff.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			

		}
		public int InviaATutti(char[] messagge, TcpClient client)
		{
			StringBuilder construtore = new StringBuilder();
			bool flags = false;
			int conut = 1;
			try
			{

				foreach (char c in messagge)
				{
					if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
					{
						construtore.Append(c);
					}
				}


				if (construtore.ToString() == "time" || construtore.ToString() == "data")
				{
					DateTime thisDay = DateTime.Today;
					byte[] buff = Encoding.ASCII.GetBytes(thisDay.ToString("g") + " \r\n");
					client.GetStream().WriteAsync(buff, 0, buff.Length);
					flags = true;

				}

				if (construtore.ToString() == "cmd")
				{
					
					byte[] buff = Encoding.ASCII.GetBytes("ChatBot: Posso rispondere solo alle seguenti domande \n\r" +
								  "#Time \n\r" +
								  "#Data\n\r" +
								  "ChatBot: Buon divertimento \n\r");
					client.GetStream().WriteAsync(buff, 0, buff.Length);
					flags = true;

				}

				if (flags == false)
				{
					byte[] buff = Encoding.ASCII.GetBytes("Non ho capito \n\r");
					client.GetStream().WriteAsync(buff, 0, buff.Length);
					flags = true;
				}
				return conut;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Errore: " + ex.Message);
				return conut;

			}

		}

		public void Dissconetti()
		{
			
			try
			{
				foreach (TcpClient client in mClients)
				{
					client.Close();
					RimuoviClient(client);
				}
				mServer.Stop();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Errore: " + ex.Message);
			}
		}

		public void InviaATuttiClient(string messagge)
		{
			try
			{
				foreach (TcpClient client in mClients)
				{
					byte[] buff = Encoding.ASCII.GetBytes(messagge);
					client.GetStream().WriteAsync(buff, 0, buff.Length);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Errore: " + ex.Message);
			}

		}

	}
}
