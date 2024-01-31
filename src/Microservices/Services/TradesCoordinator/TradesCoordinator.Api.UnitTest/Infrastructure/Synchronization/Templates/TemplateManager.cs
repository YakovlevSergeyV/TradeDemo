namespace TradesCoordinator.Infrastructure.UnitTest.Infrastructure.Synchronization.Templates
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;

    public static class TemplateManager
    {
        private const string Path = "TradesCoordinator.Infrastructure.UnitTest.Infrastructure.Synchronization.Templates";

        private static readonly Assembly CurrentAssembly;

        static TemplateManager()
        {
            CurrentAssembly = Assembly.GetExecutingAssembly();
        }

        public static string GetExceptionTooManyRequests()
        {
            return ReadTemplate("Exception_Too_Many_Requests.txt");
        }
       

        private static string ReadTemplate(string fileName)
        {
            string result;
            using (var stream = CurrentAssembly.GetManifestResourceStream(Path + "." + fileName))
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
