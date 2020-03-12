using CommandLine;

namespace FeatureSync
{
    class CommandOptions
    {
        [Option('f', "fearuresPath", Required = true, HelpText = "Путь к feature файлам")]
        public string FearuresPath { get; set; }
        [Option('s', "serverTfs", Required = true, HelpText = "URL TFS сервера")]
        public string ServerTfs { get; set; }
        [Option('t', "token", Required = true, HelpText = "Токен авторизации в TFS")]
        public string Token { get; set; }

        public override string ToString()
        {
            return $"\nfearuresPath = {FearuresPath}\nserverTfs = {ServerTfs}\ntoken = {Token}\n";
        }
    }
}
