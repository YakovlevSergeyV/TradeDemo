namespace TradesStorage.Infrastructure.UpgradeDb.Queries
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public static class QueryManager
    {
        private const string Path = "TradesStorage.Dal.Infrastructure.SQLite.Queries";

        private static readonly Assembly _currentAssembly;

        static QueryManager()
        {
            _currentAssembly = Assembly.GetExecutingAssembly();
        }

        public static string GetQueryUpgradeTo_1_0_1_0()
        {
            return ReadFile("UpgradeTo_1_0_1_0.txt");
        }

        private static string ReadFile(string fileName)
        {
            string result;
            using (var stream = _currentAssembly.GetManifestResourceStream(Path + "." + fileName))
            {
                if (stream == null)
                    throw new InvalidOperationException(string.Format("Не удалось прочитать данные из внедрённого ресурса: '{0}'.", fileName));

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
