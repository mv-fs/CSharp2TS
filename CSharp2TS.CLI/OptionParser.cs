using System.Text.Json;

namespace CSharp2TS.CLI {
    public static class OptionParser {
        public static bool TryParseConfigFilePath(string[] args, out string configPath) {
            string? parsedPath = TryParseSwitch(args, "--config", "-c");

            if (!string.IsNullOrWhiteSpace(parsedPath)) {
                configPath = parsedPath;
                return true;
            }

            configPath = string.Empty;
            return false;
        }

        public static Options? ParseFromArgs(string[] args) {
            if (args.Length == 0) {
                return null;
            }

            string[] modelAssemblyPaths = [];
            string[] servicesAssemblyPaths = [];

            string? modelAssemblyPath = TryParseSwitch(args, "--model-assembly-path", "-ma");
            string? servicesAssemblyPath = TryParseSwitch(args, "--services-assembly-path", "-sa");

            if (!string.IsNullOrWhiteSpace(modelAssemblyPath)) {
                modelAssemblyPaths = modelAssemblyPath.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            if (!string.IsNullOrWhiteSpace(servicesAssemblyPath)) {
                servicesAssemblyPaths = servicesAssemblyPath.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            var options = new Options {
                ModelOutputFolder = TryParseSwitch(args, "--model-output-folder", "-mo"),
                ModelAssemblyPaths = modelAssemblyPaths,

                ServicesOutputFolder = TryParseSwitch(args, "--services-output-folder", "-so"),
                ServicesAssemblyPaths = servicesAssemblyPaths,
                ServiceGenerator = TryParseSwitch(args, "--service-generator", "-sg") ?? Consts.AxiosService,

                FileNameCasingStyle = ParseCasingStyle(TryParseSwitch(args, "--file-casing", "-fc"), CasingStyle.PascalCase),
                MemberNameCasingStyle = ParseCasingStyle(TryParseSwitch(args, "--member-casing", "-mc"), CasingStyle.CamelCase),
                UseNullableStrings = SwitchExists(args, "--nullable-strings"),
            };

            if (!string.IsNullOrWhiteSpace(options.ModelOutputFolder)) {
                options.GenerateModels = true;
            }

            if (!string.IsNullOrWhiteSpace(options.ServicesOutputFolder)) {
                options.GenerateServices = true;
            }

            return options;
        }

        private static string? TryParseSwitch(string[] args, params string[] switches) {
            for (int i = 0; i < switches.Length; i++) {
                int idx = Array.IndexOf(args, switches[i]);

                if (idx != -1 && args.Length > idx + 1) {
                    return args[idx + 1];
                }
            }

            return null;
        }

        private static bool SwitchExists(string[] args, string @switch) {
            return Array.IndexOf(args, @switch) != -1;
        }

        private static CasingStyle ParseCasingStyle(string? value, CasingStyle defaultValue) {
            return value?.ToLowerInvariant() switch {
                "camel" => CasingStyle.CamelCase,
                "pascal" => CasingStyle.PascalCase,
                null => defaultValue,
                _ => throw new ArgumentException($"Invalid casing style '{value}'. Valid options: 'camel', 'pascal'"),
            };
        }

        public static Options? ParseFromFile(string optionsPath) {
            if (!File.Exists(optionsPath)) {
                throw new FileNotFoundException($"Config file does not exist at path: {optionsPath}");
            }

            Options? options;

            using (var stream = File.OpenRead(optionsPath)) {
                options = JsonSerializer.Deserialize<Options>(stream, new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new CasingStyleJsonConverter() },
                });
            }

            if (options == null) {
                return null;
            }

            string optionsDirectory = Path.GetDirectoryName(Path.GetFullPath(optionsPath))!;

            options.ModelOutputFolder = ResolveRelativePath(optionsDirectory, options.ModelOutputFolder);
            options.ServicesOutputFolder = ResolveRelativePath(optionsDirectory, options.ServicesOutputFolder);

            ResolveRelativePath(optionsDirectory, options.ModelAssemblyPaths);
            ResolveRelativePath(optionsDirectory, options.ServicesAssemblyPaths);

            return options;
        }

        private static void ResolveRelativePath(string optionsDirectory, string[] paths) {
            for (int i = 0; i < paths.Length; i++) {
                paths[i] = ResolveRelativePath(optionsDirectory, paths[i])!;
            }
        }

        private static string? ResolveRelativePath(string root, string? relativePath) {
            if (string.IsNullOrWhiteSpace(relativePath)) {
                return null;
            }

            if (Path.IsPathRooted(relativePath)) {
                return relativePath;
            }

            return Path.GetFullPath(Path.Combine(root, relativePath));
        }

        public static string? Validate(Options? options) {
            if (options == null) {
                return "Failed to parse options";
            }

            if (!options.GenerateModels && !options.GenerateServices) {
                return "No generation tasks specified";
            }

            if (options.GenerateModels) {
                if (string.IsNullOrWhiteSpace(options.ModelOutputFolder)) {
                    return "Models output folder is required";
                }

                if (options.ModelAssemblyPaths.Length == 0) {
                    return "At least one model assembly path is required";
                }

                foreach (var path in options.ModelAssemblyPaths) {
                    if (!File.Exists(path)) {
                        return $"Model assembly does not exist at {path}";
                    }
                }
            }

            if (options.GenerateServices) {
                if (string.IsNullOrWhiteSpace(options.ServicesOutputFolder)) {
                    return "Services output folder is required";
                }

                if (options.ServicesAssemblyPaths.Length == 0) {
                    return "At least one service assembly path is required";
                }

                foreach (var path in options.ServicesAssemblyPaths) {
                    if (!File.Exists(path)) {
                        return $"Service assembly does not exist at {path}";
                    }
                }

                if (options.ServiceGenerator != Consts.AxiosService) {
                    return $"Invalid service generator {options.ServiceGenerator}. Available options: {Consts.AxiosService})";
                }
            }

            return null;
        }
    }
}
