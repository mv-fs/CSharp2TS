using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Templates;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public class TSInterfaceGenerator {
        private Dictionary<string, TSFileInfo> files;
        private Options options;

        public TSInterfaceGenerator(Dictionary<string, TSFileInfo> files, Options options) {
            this.files = files;
            this.options = options;
        }

        public string Generate(TypeDefinition typeDef) {
            TSInterface tsInterface = new(NameUtility.GetName(typeDef));

            ParseTypes(tsInterface, typeDef, typeDef);

            return BuildTsFile(tsInterface);
        }

        private void ParseTypes(TSInterface tsInterface, TypeDefinition rootTypeDef, TypeDefinition typeDef) {
            if (typeDef.HasGenericParameters) {
                ParseGenericParams(tsInterface, typeDef);
            }

            foreach (var property in typeDef.Properties) {
                if (property.IsSpecialName || property.HasAttribute<TSExcludeAttribute>() || IsRecordEqualityContract(property)) {
                    continue;
                }

                var tsType = TSTypeMapper2.GetTSPropertyType(property.PropertyType, options, (fullName, typeName) => {
                    if (typeDef.FullName != fullName) {
                        TryAddTSImport(tsInterface, rootTypeDef, fullName, typeName);
                    }

                    return true;
                });

                tsInterface.Properties.Add(new TSInterfaceProperty(property.Name.ToCamelCase(), tsType, property.HasAttribute<TSNullableAttribute>()));
            }

            if (typeDef.BaseType != null && typeDef.BaseType.FullName != "System.Object") {
                ParseTypes(tsInterface, rootTypeDef, typeDef.BaseType.Resolve());
            }
        }

        private void ParseGenericParams(TSInterface tsInterface, TypeDefinition type) {
            foreach (var genericParam in type.GenericParameters) {
                tsInterface.GenericParameters.Add(genericParam.Name);
            }
        }

        private bool IsRecordEqualityContract(PropertyDefinition property) {
            return property.PropertyType.FullName == typeof(Type).FullName && property.FullName.EndsWith("::EqualityContract()");
        }

        private void TryAddTSImport(TSInterface tsInterface, TypeDefinition typeDef, string targetFullName, string targetName) {
            if (tsInterface.Imports.Any(i => i.FullName == targetFullName)) {
                return;
            }

            var currentType = files[typeDef.FullName];

            if (!files.TryGetValue(targetFullName, out var targetType)) {
                return;
            }

            string importPath = currentType.GetImportPathTo(targetType);

            tsInterface.Imports.Add(new TSImport(targetFullName, targetName, importPath));
        }

        private string BuildTsFile(TSInterface tsInterface) {
            return new TSInterfaceTemplate {
                TSInterface = tsInterface,
            }.TransformText();
        }
    }
}
