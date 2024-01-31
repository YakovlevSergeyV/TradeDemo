using Microservices.Common;

namespace TradesStorage.Api
{

    public class Program
    {
        private static CommonProgram<SettingsApplication, Startup> commonProgram;

        static Program()
        {
            //commonProgram = new CommonProgram<SettingsApplication, Startup>("http://localhost:5000");
            commonProgram = new CommonProgram<SettingsApplication, Startup>();
        }

        public static void Main(string[] args)
        {
            commonProgram.Main(args);
        }
    }
}
