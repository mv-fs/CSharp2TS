using CSharp2TS.CLI.Generators;
using System.Text.Json;

namespace CSharp2TS.CLI {
    public class Program {
        private static void Main(string[] args) {
            if (args.Length == 0) {
                ShowIntro();
                return;
            }

            Options? options;

            if (args.Length == 1) {
                string[] helpCommands = ["-h", "-help", "--help"];

                if (helpCommands.Contains(args[0])) {
                    ShowHelp();
                    return;
                }

                if (args[0] == "create-config") {
                    CreateDefaultConfig();
                    return;
                }
            }

            if (OptionParser.TryParseConfigFilePath(args, out string configPath)) {
                options = OptionParser.ParseFromFile(configPath);
            } else {
                options = OptionParser.ParseFromArgs(args);
            }

            string? errorMessage = OptionParser.Validate(options);

            if (!string.IsNullOrWhiteSpace(errorMessage)) {
                Console.WriteLine(errorMessage);
                Environment.Exit(-1);
            }

            Generator generator = new Generator(options!);
            generator.Run();
        }

        private static void CreateDefaultConfig() {
            Options options = new Options();

            using (var stream = File.Create("csharp2ts.json")) {
                JsonSerializer.Serialize(stream, options, new JsonSerializerOptions {
                    WriteIndented = true,
                });
            }

            Console.WriteLine("Config created successfully");
        }

        private static void ShowIntro() {
            Console.WriteLine("csharp2ts");
            Console.WriteLine("-------------");
            Console.WriteLine("Run csharp2ts [-h | -help] to see commands");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  cshart2ts -c <path to config>");
            Console.WriteLine("  --- OR ---");
            Console.WriteLine("  cshart2ts <arguments>");
            Console.WriteLine("-------------");
        }

        private static void ShowHelp() {
            Console.WriteLine("Usage:");
            Console.WriteLine("csharp2ts [options]");
            Console.WriteLine();
            Console.WriteLine("From Config File:");
            Console.WriteLine("--config, -c <path to config>        Path to the config file");
            Console.WriteLine();
            Console.WriteLine("CLI Options:");
            Console.WriteLine("--model-assembly-path, -ma <path>    Path(s) to the assembly containing the models (comma-separated for multiple)");
            Console.WriteLine("--model-output-folder, -mo <path>    Path to the output folder for the generated models");
            Console.WriteLine("--services-assembly-path, -sa <path> Path(s) to the assembly containing the services (comma-separated for multiple)");
            Console.WriteLine("--services-output-folder, -so <path> Path to the output folder for the generated services");
            Console.WriteLine("--service-generator, -sg <name>      Service generator to use (axios)");
            Console.WriteLine("--file-casing, -fc <style>           File name casing style (camel | pascal)");
            Console.WriteLine("--nullable-strings                   Mark string properties as nullable in the generated code");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("create-config                        Create a default config file");
        }
    }
}
