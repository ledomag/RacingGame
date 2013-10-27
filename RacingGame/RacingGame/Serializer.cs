namespace RacingGame
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    /// <summary>
    /// Сериализует и десериализует объекты
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// XML сериализация
        /// </summary>
        /// <param name="obj">Объект.</param>
        /// <param name="path">Путь файла.</param>
        public static void XmlSerialization(object obj, string path)
        {
            if (obj != null && path != null && path != string.Empty)
            {
                XmlSerializer xs = new XmlSerializer(obj.GetType());

                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    xs.Serialize(fs, obj);
                }
            }
        }

        /// <summary>
        /// XML десериализация
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="path">Путь файла.</param>
        /// <returns>Объект.</returns>
        public static T XmlDeserialization<T>(string path) where T : new()
        {
            if (File.Exists(path))
            {
                T obj = default(T);

                using (FileStream fs = File.OpenRead(path))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    obj = (T)xs.Deserialize(fs);
                }

                return obj;
            }

            return default(T);
        }

        /// <summary>
        /// Бинарная сериализация
        /// </summary>
        /// <param name="obj">Объект.</param>
        /// <param name="path">Путь файла.</param>
        public static void BinarySerialization(object obj, string path)
        {
            if (obj != null && path != null && path != string.Empty)
            {
                BinaryFormatter bf = new BinaryFormatter();

                using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    bf.Serialize(fs, obj);
                }
            }
        }

        /// <summary>
        /// Бинарная десериализация
        /// </summary>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <param name="path">Путь файла.</param>
        /// <returns>Объект.</returns>
        public static T BinaryDeserialization<T>(string path)
        {
            if (File.Exists(path))
            {
                T obj = default(T);

                using (FileStream fs = File.OpenRead(path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    obj = (T)bf.Deserialize(fs);
                }

                return obj;
            }

            return default(T);
        }
    }
}
