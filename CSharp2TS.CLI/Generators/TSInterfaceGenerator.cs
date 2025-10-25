using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Templates;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Generators {
    public class TSInterfaceGenerator : GeneratorBase<TSInterfaceAttribute> {
        private IList<TSInterfaceProperty> properties;
        private IList<string> genericParameters;
        private Dictionary<string, TSFileInfo> files;

        public TSInterfaceGenerator(TypeDefinition type, Options options, Dictionary<string, TSFileInfo> files) : base(type, options) {
            this.files = files;

            properties = [];
            genericParameters = [];
        }

        public override string Generate() {
            ParseTypes(Type);

            return BuildTsFile();
        }

        private void ParseTypes(TypeDefinition typeDef) {
            if (typeDef == Type && typeDef.HasGenericParameters) {
                ParseGenericParams();
            }

            foreach (var property in typeDef.Properties) {
                if (property.IsSpecialName || property.HasAttribute<TSExcludeAttribute>() || IsRecordEqualityContract(property)) {
                    continue;
                }

                var tsType = GetTSPropertyType(property.PropertyType, Options.ModelOutputFolder!, property.HasAttribute<TSNullableAttribute>());

                properties.Add(new TSInterfaceProperty(property.Name.ToCamelCase(), tsType));
            }

            if (typeDef.BaseType != null) {
                ParseTypes(typeDef.BaseType.Resolve());
            }
        }

        private void ParseGenericParams() {
            foreach (var genericParam in Type.GenericParameters) {
                genericParameters.Add(genericParam.Name);
            }
        }

        private bool IsRecordEqualityContract(PropertyDefinition property) {
            return property.PropertyType.FullName == typeof(Type).FullName && property.FullName.EndsWith("::EqualityContract()");
        }

        public override string GetFileName() {
            return ApplyCasing(GetCleanedTypeName(Type));
        }

        protected override void TryAddTSImport(TSProperty tsType, string? currentFolderRoot, string? targetFolderRoot) {
            if (currentFolderRoot == null || targetFolderRoot == null || Imports.ContainsKey(tsType.GetTypeName()) || !tsType.IsObject || tsType.TypeRef.IsGenericParameter) {
                return;
            }

            var currentType = files[Type.FullName];
            string targetTypeName = tsType.TypeRef.Resolve().FullName;

            if (!files.TryGetValue(targetTypeName, out var targetType)) {
                return;
            }

            string importPath = currentType.GetImportPathTo(targetType);

            Imports.Add(tsType.GetTypeName(), new TSImport(tsType.GetTypeName(), importPath));
        }

        private string BuildTsFile() {
            return new TSInterfaceTemplate {
                TypeName = GetCleanedTypeName(Type),
                Imports = Imports.Select(i => i.Value).ToList(),
                Properties = properties,
                GenericParameters = genericParameters,
            }.TransformText();
        }
    }
}
