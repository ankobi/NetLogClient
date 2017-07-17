using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using NetLogClient.Log4j;

namespace NetLogClient
{
	static class LogEntryManager
	{

		#region Member Variables
		internal enum ThresholdLevel:byte
		{
			TRACE,
			DEBUG,
			INFO,
			WARN,
			ERROR,
			FATAL
		}
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static Queue<LogEntry> _logEntryList = new Queue<LogEntry>(_cacheSize);
		private static DateTime _timeCacheLastChanged = DateTime.Now;
		private static int _cacheSize = 10;
		private static bool _collectionIsPaused;
		private static ThresholdLevel _thresholdLevel = ThresholdLevel.TRACE;
		#endregion Member Variables

		#region Methods
		static internal void CreateLogEntry(string xml, string ipAddress)
		{
			if (_collectionIsPaused) return;

			try
			{
				@event logEvent;
				XmlSerializer mySerializer = new XmlSerializer(typeof(@event));
				xml = xml.Replace("log4net:", string.Empty);
				xml = xml.Replace("log4j:", string.Empty);

				TextReader stringReader = new StringReader(xml);
				logEvent = (@event)mySerializer.Deserialize(stringReader);

				lock (_logEntryList)
				{
					LogEntry logEntry = new LogEntry(logEvent, xml);
					logEntry.SenderIP = ipAddress;
					if (IsAboveThreshold(logEntry))
					{
						_logEntryList.Enqueue(logEntry);
						while (_logEntryList.Count > _cacheSize)
						{
							int numberToTruncateTo = ((int)(_cacheSize * 0.01d));
							while (_logEntryList.Count >= (_cacheSize - numberToTruncateTo))
							{
								_logEntryList.Dequeue();
							}
						}

						_timeCacheLastChanged = DateTime.Now;
					}
				}
			}
			catch (InvalidOperationException ex)
			{
				log.Warn(
					string.Format("InvalidOperationException exception caught while trying to deserialize the message: {0}; Message was: {1}",
					ex.Message,
					xml
					)
				);
			}
		}

		private static bool IsAboveThreshold(LogEntry logEntry)
		{
			ThresholdLevel eventLevel = ThresholdLevel.TRACE;
			try
			{
				eventLevel = (ThresholdLevel)Enum.Parse(typeof(ThresholdLevel), logEntry.LogEvent.level);
			}
			catch{}
			return eventLevel.CompareTo(_thresholdLevel) >= 0;
		}

		internal static void Clear()
		{
			_logEntryList.Clear();
		}

		#endregion Methods

		#region Properties

		internal static ThresholdLevel Threshold
		{
			get { return LogEntryManager._thresholdLevel; }
			set { LogEntryManager._thresholdLevel = value; }
		}
		internal static Queue<LogEntry> LogEntryList
		{
			get { return _logEntryList; }
		}

		internal static DateTime TimeCacheLastChanged
		{
			get { return LogEntryManager._timeCacheLastChanged; }
			set { LogEntryManager._timeCacheLastChanged = value; }
		}

		internal static bool CollectionIsPaused
		{
			//get { return _collectionIsPaused; }
			set
			{
				_collectionIsPaused = value;
				if (_collectionIsPaused)
					log.Info("The collection process is paused.  No new events will be added to the cache.");
				else
                    log.Info("The collection process is resumed.  New events will be added to the cache.");
			}
		}

		internal static int CacheSize
		{
			get { return LogEntryManager._cacheSize; }
			set { LogEntryManager._cacheSize = value; }
		}
		#endregion Properties
	}
}
