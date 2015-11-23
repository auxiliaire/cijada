using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CA.Model.Serializer
{
	public class SSettings
	{
		public SSettings ()
		{
		}

		public void SerializeObject(string filename, CA.Model.Settings objectToSerialize)
		{
			Stream stream = File.Open(filename, FileMode.Create);
			BinaryFormatter bFormatter = new BinaryFormatter();
			bFormatter.Serialize(stream, objectToSerialize);
			stream.Close();
		}

		public CA.Model.Settings DeSerializeObject(string filename)
		{
			CA.Model.Settings objectToSerialize;
			Stream stream = File.Open(filename, FileMode.Open);
			BinaryFormatter bFormatter = new BinaryFormatter();
			objectToSerialize = (CA.Model.Settings)bFormatter.Deserialize(stream);
			stream.Close();
			return objectToSerialize;
		}
	}
}

