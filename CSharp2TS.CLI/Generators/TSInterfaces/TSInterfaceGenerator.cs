using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Templates;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public class TSInterfaceGenerator {
        private IList<TSInterfaceProperty> properties;
        private IList<string> genericParameters;
        private Dictionary<string, TSFileInfo> files;
        private Options options;
        private Dictionary<string, TSImport> imports;

        public TSInterfaceGenerator(Dictionary<string, TSFileInfo> files, Options options) {
            this.files = files;
            this.options = options;

            properties = [];
            genericParameters = [];
            imports = [];
        }

        public string Generate(TypeDefinition typeDef) {
            properties = [];
            genericParameters = [];
            imports = [];

            ParseTypes(typeDef);

            return BuildTsFile(typeDef);
        }

        private void ParseTypes(TypeDefinition typeDef) {
            if (typeDef.HasGenericParameters) {
                ParseGenericParams(typeDef);
            }

            foreach (var property in typeDef.Properties) {
                if (property.IsSpecialName || property.HasAttribute<TSExcludeAttribute>() || IsRecordEqualityContract(property)) {
                    continue;
                }

                var currentFolder = files[typeDef.FullName].Folder;

                var tsType = TSTypeMapper.GetTSPropertyType(property.PropertyType, options, property.HasAttribute<TSNullableAttribute>(), (tsProperty) => {
                    if (typeDef != tsProperty.TypeRef) {
                        TryAddTSImport(typeDef, tsProperty, options);
                    }

                    return true;
                });

                properties.Add(new TSInterfaceProperty(property.Name.ToCamelCase(), tsType));
            }

            if (typeDef.BaseType != null) {
                ParseTypes(typeDef.BaseType.Resolve());
            }
        }

        private void ParseGenericParams(TypeDefinition type) {
            foreach (var genericParam in type.GenericParameters) {
                genericParameters.Add(genericParam.Name);
            }
        }

        private bool IsRecordEqualityContract(PropertyDefinition property) {
            return property.PropertyType.FullName == typeof(Type).FullName && property.FullName.EndsWith("::EqualityContract()");
        }

        private void TryAddTSImport(TypeDefinition typeDef, TSProperty tsType, Options options) {
            if (imports.ContainsKey(tsType.GetTypeName()) || !tsType.IsObject || tsType.TypeRef.IsGenericParameter) {
                return;
            }

            var currentType = files[typeDef.FullName];
            string targetTypeName = tsType.TypeRef.Resolve().FullName;

            if (!files.TryGetValue(targetTypeName, out var targetType)) {
                return;
            }

            string importPath = currentType.GetImportPathTo(targetType);

            imports.Add(tsType.GetTypeName(), new TSImport(tsType.GetTypeName(), importPath));
        }

        private string BuildTsFile(TypeDefinition typeDef) {
            return new TSInterfaceTemplate {
                TypeName = files[typeDef.FullName].TypeName,
                Imports = imports.Select(i => i.Value).ToList(),
                Properties = properties,
                GenericParameters = genericParameters,
            }.TransformText();
        }
    }
}
