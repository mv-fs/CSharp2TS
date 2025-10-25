using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Generators.Enums;
using CSharp2TS.CLI.Templates;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Generators {
    public class Generator {
        private readonly Options options;
        private readonly Dictionary<string, TSFileInfo> files = [];

        public Generator(Options options) {
            this.options = options;
        }

        public void Run() {
            if (options.GenerateModels) {
                GenerateModels();
            }

            if (options.GenerateServices) {
                GenerateServices();
            }
        }

        private void GenerateModels() {
            if (Directory.Exists(options.ModelOutputFolder)) {
                Directory.Delete(options.ModelOutputFolder, true);
            }

            Directory.CreateDirectory(options.ModelOutputFolder!);

            // Gather all imports so we know if we can reference them later on
            foreach (var assemblyPath in options.ModelAssemblyPaths) {
                using (var assembly = LoadAssembly(assemblyPath)) {
                    GatherImports(assembly.MainModule);
                }
            }

            foreach (var assemblyPath in options.ModelAssemblyPaths) {
                using (var assembly = LoadAssembly(assemblyPath)) {
                    GenerateInterfaces(assembly.MainModule, options);
                    GenerateEnums(assembly.MainModule, options);
                }
            }
        }

        private void GatherImports(ModuleDefinition module) {
            var enums = GetTypesByAttribute(module, typeof(TSEnumAttribute));

            foreach (var type in enums) {
                files.Add(type.FullName, NameUtility.GetFileDetails(type, options, options.ModelOutputFolder!));
            }

            var interfaces = GetTypesByAttribute(module, typeof(TSInterfaceAttribute));

            foreach (var type in interfaces) {
                files.Add(type.FullName, NameUtility.GetFileDetails(type, options, options.ModelOutputFolder!));
            }
        }

        private void GenerateServices() {
            if (Directory.Exists(options.ServicesOutputFolder)) {
                Directory.Delete(options.ServicesOutputFolder, true);
            }

            Directory.CreateDirectory(options.ServicesOutputFolder!);

            GenerateApiClient();

            foreach (var assemblyPath in options.ServicesAssemblyPaths) {
                using (var assembly = LoadAssembly(assemblyPath)) {
                    GenerateServices(assembly.MainModule, options);
                }
            }
        }

        private AssemblyDefinition LoadAssembly(string assemblyPath) {
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(Path.GetDirectoryName(assemblyPath)!);

            return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters {
                AssemblyResolver = resolver,
            });
        }

        private void GenerateApiClient() {
            string apiClientTemplate = new TSAxiosApiClientTemplate().TransformText();
            string path = Path.Combine(options.ServicesOutputFolder!, "apiClient.ts");

            File.WriteAllText(path, apiClientTemplate);
        }

        private void GenerateInterfaces(ModuleDefinition module, Options options) {
            var types = GetTypesByAttribute(module, typeof(TSInterfaceAttribute));

            if (!types.Any()) {
                return;
            }

            foreach (TypeDefinition type in types) {
                GenerateFile(options.ModelOutputFolder!, new TSInterfaceGenerator(type, options, files));
            }
        }

        private void GenerateEnums(ModuleDefinition module, Options options) {
            var types = GetTypesByAttribute(module, typeof(TSEnumAttribute));

            if (!types.Any()) {
                return;
            }

            TSEnumGenerator generator = new();

            foreach (TypeDefinition type in types) {
                string fileContents = generator.Generate(type);

                GenerateFile(files[type.FullName], fileContents);
            }
        }

        private void GenerateServices(ModuleDefinition module, Options options) {
            var types = GetTypesByAttribute(module, typeof(TSServiceAttribute));

            foreach (TypeDefinition type in types) {
                GenerateFile(options.ServicesOutputFolder!, new TSAxiosServiceGenerator(type, options));
            }
        }

        private IEnumerable<TypeDefinition> GetTypesByAttribute(ModuleDefinition module, Type attributeType) {
            foreach (var type in module.GetTypes()) {
                if (type.HasAttribute(attributeType)) {
                    yield return type;
                }
            }
        }

        private void GenerateFile<TAttribute>(string outputFolder, GeneratorBase<TAttribute> generator) where TAttribute : TSAttributeBase {
            string output = generator.Generate();
            string folder = Path.Combine(outputFolder, generator.GetFolderLocation());

            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            string file = Path.Combine(folder, $"{generator.GetFileName()}.ts");

            if (File.Exists(file)) {
                throw new InvalidOperationException($"File {file} already exists.");
            }

            File.WriteAllText(file, output);
        }

        private void GenerateFile(TSFileInfo fileInfo, string fileContents) {
            if (!Directory.Exists(fileInfo.Folder)) {
                Directory.CreateDirectory(fileInfo.Folder);
            }

            if (File.Exists(fileInfo.FileFullPath)) {
                throw new InvalidOperationException($"File {fileInfo.FileFullPath} already exists.");
            }

            File.WriteAllText(fileInfo.FileFullPath, fileContents);
        }
    }
}
