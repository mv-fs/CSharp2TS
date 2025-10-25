using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Generators {
    public abstract class GeneratorBase<TAttribute> where TAttribute : TSAttributeBase {
        protected IDictionary<string, TSImport> Imports { get; }

        public TypeDefinition Type { get; }
        public Options Options { get; }

        public abstract string Generate();
        public abstract string GetFileName();

        protected GeneratorBase(TypeDefinition type, Options options) {
            Imports = new Dictionary<string, TSImport>();
            Type = type;
            Options = options;
        }

        public string GetFolderLocation() {
            return Type.GetCustomFolderLocation() ?? string.Empty;
        }

        protected TSProperty GetTSPropertyType(TypeReference type, string currentFolder, bool isNullableProperty = false) {
            return TSTypeMapper.GetTSPropertyType(type, Options, isNullableProperty, (tsProperty) => {
                if (Type != tsProperty.TypeRef) {
                    TryAddTSImport(tsProperty, currentFolder, Options.ModelOutputFolder);
                }

                return true;
            });
        }

        protected string ApplyCasing(string str) {
            if (Options.FileNameCasingStyle == Consts.CamelCase) {
                return str.ToCamelCase();
            }

            // We assume PascalCase for C# types by default
            return str;
        }

        protected virtual void TryAddTSImport(TSProperty tsType, string? currentFolderRoot, string? targetFolderRoot) {
            if (currentFolderRoot == null || targetFolderRoot == null || Imports.ContainsKey(tsType.GetTypeName()) || !tsType.IsObject || tsType.TypeRef.IsGenericParameter) {
                return;
            }

            var targetCustomFolder = tsType.TypeRef.Resolve().GetCustomFolderLocation();

            string currentFolder = Path.Combine(currentFolderRoot, GetFolderLocation());
            string targetFolder = Path.Combine(targetFolderRoot, targetCustomFolder ?? string.Empty);

            string relativePath = FolderUtility.GetRelativeImportPath(currentFolder, targetFolder);
            string importPath = $"{relativePath}{ApplyCasing(tsType.GetTypeName())}";

            Imports.Add(tsType.GetTypeName(), new TSImport(tsType.GetTypeName(), importPath));
        }

        protected string GetCleanedTypeName(TypeReference type) {
            return TSTypeMapper.GetCleanedTypeName(type);
        }
    }
}
