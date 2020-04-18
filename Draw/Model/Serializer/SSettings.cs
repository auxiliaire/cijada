using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CA.Model.Serializer
{
    public class SSettings
    {
        public SSettings()
        {
        }

        public void SerializeObject(string filename, Settings objectToSerialize)
        {
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public Settings DeSerializeObject(string filename)
        {
            Settings objectToSerialize;
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectToSerialize = (Settings)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }
    }
}

