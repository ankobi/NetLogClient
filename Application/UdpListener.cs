using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace NetLogClient
{
	/// <summary>
	/// Listens for incoming UDP messages
	/// </summary>
	public class UdpListener : IDisposable
	{

		#region Member Variables
		/************************************************
		 * Member Variables
		 ***********************************************/
		private static readonly log4net.ILog m_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private AdvUdpClient m_client;
		private IPAddress m_multicastAddress;
		private bool m_isListening;
		private Thread m_listenThread;
		private int m_port = 8080;
		
		#endregion Member Variables

		#region Delegates
		private delegate byte[] ReceiveInvoke( ref IPEndPoint remoteEP );
		#endregion Delegates

		internal UdpListener(int port)
		{
			// We are listening to the broadcast group
			m_multicastAddress = IPAddress.Parse("224.0.0.1");
			m_port = port;
		}

		public void Stop()
		{
			m_log.Debug("Attempting to stop the UDP listener...");
			m_isListening = false;
			m_listenThread.Abort();
			m_log.Info("UDP listener stopped.");
		}

		public void Start() {
			m_log.Debug("Attempting to start the UDP listener...");
			try {
				// If listening... whoops
				if (m_isListening)
				{
					string msg = "Cannot start the UDP listener; Already listening.";
					m_log.Error(msg);
					throw new ApplicationException(msg);
				}

				lock(this){
					// Try to connect to the multi-cast address and start 
					// the listening thread
					if(m_client!=null){
						KillClient();
					}

					// Listening now
					m_isListening = true;

					m_client = new AdvUdpClient(m_port);
				
					// Have the client join the multicast group
					m_client.JoinMulticastGroup(m_multicastAddress);
				}
                
				// Start listening on a separate thread
				m_listenThread = new Thread(new ThreadStart(ListenLoop));
				m_listenThread.Start();
				m_log.Info("Successfully started the UDP listener.");
			} 
			catch(SocketException e) {
				// Not listening
				m_isListening = false;

				throw new ApplicationException("Could not start listening",e);
			}
		}

		private void KillClient(){
			try{
				m_isListening = false;
				Thread.Sleep(150);
				int numRetries = 0;
				if(m_listenThread!=null){					
					while( m_listenThread.ThreadState!= System.Threading.ThreadState.Stopped
						& numRetries <= 3){
						Thread.Sleep(333);
						m_listenThread.Abort();
						numRetries++;
					}
				}
				//if(_client!=null) _client.Close();
				m_listenThread=null;
				m_client = null;
			}catch(Exception){
				//ignore
			}
		}
		
		private void ListenLoop() {
			//DO NOT PUT ANY LOGGING HERE!  YOU COULD GET INTO A ENDLESS LOOP SITUATION

			// Store the client locally
			AdvUdpClient LocalClient = m_client;

			// Place to hold data
            byte[] cache;

			// Address of sender
			IPEndPoint sender = null;

			// Log entry data
			string message;

			// Get a pointer to the receive method
			ReceiveInvoke receiver = new ReceiveInvoke(LocalClient.Receive);

			// Automatically drop out when we stop.
			while (m_isListening) {
				// Start the invoke
				IAsyncResult result = receiver.BeginInvoke(ref sender, null, null);

				while (m_isListening) {
					// Wait for data to be available
					result.AsyncWaitHandle.WaitOne(250, false);

					// Did we receive?
					if (result.IsCompleted) break;
				}

				// If we're still listening at the bottom of the while 
				// loop, then we have data to receive. Otherwise, we 
				// are no longer listening and we should bail out of 
				// the loop.
				if (m_isListening) {
					/* We have data to receive */

					// Receive data
                    cache = receiver.EndInvoke(ref sender, result);
                    // cache = LocalClient.Receive(ref Sender);

					// Convert to text message
                    Encoding encoding = DetectEncoding(cache);
                    message = encoding.GetString(cache);
					LogEntryManager.CreateLogEntry(message, sender.Address.ToString());
					// Go for another message!
				}
				else {
					/* We need to shut down */

					// Shut down the client
					LocalClient.Close();

					// Bail
					return;
				}
			}
		}


		private static Encoding DetectEncoding(byte[] bytes)
		{
			Encoding enc = Encoding.ASCII;

			if (
				(bytes[0] == 0x3c && bytes[1] == 0 && bytes[2] == 0x6c && bytes[3] == 0)  // utf-16 or utf-8
				)
			{
				enc = System.Text.Encoding.Unicode;
			}
			else
			{
				enc = System.Text.Encoding.ASCII;
			}

			return enc;
		}


		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if(m_client!=null) m_client.Close();
		}

		#endregion
	} //class

	//Lessens the occurances of this SocketException:
	//"Only one usage of each socket address (protocol/network address/port) is normally permitted"	string
	//Credit: http://www.ondotnet.com/pub/a/dotnet/2003/04/14/clientserver.html
	internal class AdvUdpClient : System.Net.Sockets.UdpClient {
		public AdvUdpClient( int port ) {
			this.Client.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.ReuseAddress,100);
			//this.Client.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.DontLinger,null);
			this.Client.Bind( new IPEndPoint(IPAddress.Any, port) );
		}

		public new void Close(){

			//Uses the protected Active property belonging to the UdpClient base class to determine if a connection is established.
			if (this.Active){
				//Uses the Socket returned by Client to set an option that is not available using UdpClient.
				this.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			}

			base.Client.Shutdown(SocketShutdown.Both);
			base.Close();  
		}
	} //class
}  //ns
