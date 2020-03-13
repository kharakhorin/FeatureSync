using CommandLine;

namespace FeatureSync
{
    class CommandOptions
    {
        [Option('f', "features", Required = true, HelpText = "Feature files directory")]
        public string FearuresPath { get; set; }
        [Option('s', "server", Required = true, HelpText = "TFS server URL")]
        public string ServerTfs { get; set; }
        [Option('t', "token", Required = true, HelpText = "Personal Access Token TFS")]
        public string Token { get; set; }

        public override string ToString()
        {
            return $"\nfearuresPath = {FearuresPath}\nserverTfs = {ServerTfs}\ntoken = {Token}\n";
        }
    }
}
