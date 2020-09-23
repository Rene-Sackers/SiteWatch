using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace SiteWatch.Extensions
{
	public static class DeepCloneExtension
	{
		public static T Clone<T>(this T source)
		{
			if (source == null)
				return default;

			var type = source.GetType();

			if (type.IsSerializable)
			{
				var formatter = new BinaryFormatter();

				using var stream = new MemoryStream();

				formatter.Serialize(stream, source);

				stream.Seek(0, SeekOrigin.Begin);

				return (T) formatter.Deserialize(stream);
			}

			try
			{
				var data = JsonConvert.SerializeObject(source);

				return (T) JsonConvert.DeserializeObject(data, type);
			}
			catch (Exception)
			{
				throw new ArgumentException("The type must be serializable.", nameof(source));
			}
		}
	}
}