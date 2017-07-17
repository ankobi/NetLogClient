using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace NetLogClient
{
	internal class NetLogClientConfig
	{
		#region Members
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static int _defaultCacheSize = 10000;
		private static int _defaultPort = 8080;
		private static int _defaultRefreshIntervalMilliseconds = 8080;
		#endregion Members

		internal int CacheSize
		{
			get
			{
				string valueFromAppSettings = ConfigurationManager.AppSettings["CacheSize"];
				int val = _defaultCacheSize;
				if (Int32.TryParse(valueFromAppSettings, out val))
				{
					return val;
				}
				else
				{
					log.Warn("Using default value for \"CacheSize\".  Could not find a valid setting for this in the configuration file.");
					return _defaultCacheSize;
				}
			}
		}
		internal int Port
		{
			get
			{
				string valueFromAppSettings = ConfigurationManager.AppSettings["Port"];
				int val = _defaultPort;
				if (Int32.TryParse(valueFromAppSettings, out val))
				{
					return val;
				}
				else
				{
					log.Warn("Using default value for \"Port\".  Could not find a valid setting for this in the configuration file.");
					return _defaultPort;
				}
			}
		}
		internal int RefreshIntervalMilliseconds
		{
			get
			{
				string valueFromAppSettings = ConfigurationManager.AppSettings["RefreshIntervalMilliseconds"];
				int val = _defaultRefreshIntervalMilliseconds;
				if (Int32.TryParse(valueFromAppSettings, out val))
				{
					return val;
				}
				else
				{
					log.Warn("Using default value for \"RefreshIntervalMilliseconds\".  Could not find a valid setting for this in the configuration file.");
					return _defaultRefreshIntervalMilliseconds;
				}
			}
		}
	}
}
