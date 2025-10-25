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

            ParseTypes(tsInterface, typeDef);

            return BuildTsFile(tsInterface);
        }

        private void ParseTypes(TSInterface tsInterface, TypeDefinition typeDef) {
            if (typeDef.HasGenericParameters) {
                ParseGenericParams(tsInterface, typeDef);
            }

            foreach (var property in typeDef.Properties) {
                if (property.IsSpecialName || property.HasAttribute<TSExcludeAttribute>() || IsRecordEqualityContract(property)) {
                    continue;
                }

                var currentFolder = files[typeDef.FullName].Folder;

                var tsType = TSTypeMapper.GetTSPropertyType(property.PropertyType, options, property.HasAttribute<TSNullableAttribute>(), (tsProperty) => {
                    if (typeDef != tsProperty.TypeRef) {
                        TryAddTSImport(tsInterface, typeDef, tsProperty, options);
                    }

                    return true;
                });

                tsInterface.Properties.Add(new TSInterfaceProperty(property.Name.ToCamelCase(), tsType));
            }

            if (typeDef.BaseType != null) {
                ParseTypes(tsInterface, typeDef.BaseType.Resolve());
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

        private void TryAddTSImport(TSInterface tsInterface, TypeDefinition typeDef, TSProperty tsType, Options options) {
            if (tsInterface.Imports.Any(i => i.Name == tsType.GetTypeName()) || !tsType.IsObject || tsType.TypeRef.IsGenericParameter) {
                return;
            }

            var currentType = files[typeDef.FullName];
            string targetTypeName = tsType.TypeRef.Resolve().FullName;

            if (!files.TryGetValue(targetTypeName, out var targetType)) {
                return;
            }

            string importPath = currentType.GetImportPathTo(targetType);

            tsInterface.Imports.Add(new TSImport(tsType.GetTypeName(), importPath));
        }

        private string BuildTsFile(TSInterface tsInterface) {
            return new TSInterfaceTemplate {
                TSInterface = tsInterface,
            }.TransformText();
        }
    }
}
