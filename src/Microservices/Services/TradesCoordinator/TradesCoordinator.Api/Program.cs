using Microservices.Common;

namespace TradesCoordinator.Api
{
    public class Program
    {
        private static CommonProgram<SettingsApplication, Startup> _commonProgram;

        static Program()
        {
            //commonProgram = new CommonProgram<SettingsApplication, Startup>("http://localhost:5002");
            _commonProgram = new CommonProgram<SettingsApplication, Startup>();
        }

        public static void Main(string[] args)
        {
            _commonProgram.Main(args);
        }
    }
}
