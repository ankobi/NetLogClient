using System;
using System.Net;
using System.Collections;
using System.Text;
using NetLogClient.Log4j;

namespace NetLogClient
{
	internal class LogEntry
	{
		private string _rawMessage;
		private string _senderIP;
		private @event _logEvent;

		internal LogEntry(@event logEvent, string rawMessage)
		{
			_logEvent = logEvent;
			_rawMessage = rawMessage;
		}

		/// <summary>
		/// Gets the message of the entry.
		/// </summary>
		internal string RawMessage
		{
			get
			{
				return _rawMessage;
			}
		}

		internal string SenderIP
		{
			get { return _senderIP; }
			set { _senderIP = value; }
		}

		internal @event LogEvent
		{
			get { return _logEvent; }
		}
	}
}
