using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TFS.Client;

namespace FeatureSync
{
    public class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandOptions>(args)
                .WithNotParsed(HandleParseError)
                .WithParsed(Sync);
        }

        static void Sync(CommandOptions opts)
        {
            Console.WriteLine(opts.ToString());

            var TFSClient = new TFSClient(opts.ServerTfs, opts.Token);

            var files = Directory.GetFiles(opts.FearuresPath, "*.feature");
            if (files.Count() == 0)
                throw new Exception($"В папке {opts.FearuresPath} не найдено feature файлов");

            Task.WhenAll(TestCaseParser.ParseFeatures(files).Select(test => TFSClient.UpdateTestCase(test))).Wait();
        }

        static void HandleParseError(IEnumerable<Error> errs)
        {
            if (!errs.IsHelp())
                throw new Exception($"Ошибки чтения параметров: {string.Join(",", errs.Select(e => e.ToString()))}");
        }
    }
}
