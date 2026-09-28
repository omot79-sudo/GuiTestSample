using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GuiTestKit.Evidence
{
    /// <summary>
    /// record.json の読み書き。.NET Framework 標準の DataContractJsonSerializer を使い、
    /// 追加のライブラリなしで動くようにしている。
    /// </summary>
    public static class JsonFile
    {
        private static readonly DataContractJsonSerializerSettings Settings =
            new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };

        public static void Write<T>(string path, T value)
        {
            using (var stream = File.Create(path))
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, new UTF8Encoding(false), false, true, "  "))
            {
                new DataContractJsonSerializer(typeof(T), Settings).WriteObject(writer, value);
                writer.Flush();
            }
        }

        public static T Read<T>(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return (T)new DataContractJsonSerializer(typeof(T), Settings).ReadObject(stream);
            }
        }
    }
}
