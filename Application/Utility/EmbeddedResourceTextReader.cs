using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NetLogClient.Utility
{
	static class EmbeddedResourceTextReader
	{
		internal static string GetFromResources(string resourceName)
		{
			//http://www.codeproject.com/csharp/embeddedresourcestrings.asp

			Assembly assem = typeof(EmbeddedResourceTextReader).Assembly;
			using (Stream stream = assem.GetManifestResourceStream(resourceName))
			{
				try
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						return reader.ReadToEnd();
					}
				}
				catch (Exception e)
				{
					throw new Exception("Error retrieving from Resources. Tried '"
											 + resourceName + "'\r\n" + e.ToString());
				}
			}
		}

	}
}
