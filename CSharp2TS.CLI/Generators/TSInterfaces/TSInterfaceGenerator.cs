using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.CLI.Generators.Entities;
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
            var interfaceAttribute = typeDef.GetAttribute<TSInterfaceAttribute>();
            tsInterface.GenerateClass = interfaceAttribute.GetAttributeValue<bool>(nameof(TSInterfaceAttribute.GenerateClass));
            tsInterface.IncludeMethods = interfaceAttribute.GetAttributeValue<bool>(nameof(TSInterfaceAttribute.IncludeMethods));

            ParseTypes(tsInterface, typeDef, typeDef);

            return BuildTsFile(tsInterface);
        }

        private void ParseTypes(TSInterface tsInterface, TypeDefinition rootTypeDef, TypeDefinition typeDef) {
            if (typeDef.HasGenericParameters) {
                ParseGenericParams(tsInterface, typeDef);
            }

            foreach (var property in typeDef.Properties) {
                if (property.IsSpecialName || !IsPublic(property) || property.HasAttribute<TSExcludeAttribute>() || IsRecordEqualityContract(property)) {
                    continue;
                }

                string propertyName = property.Name.ApplyCasing(options.MemberNameCasingStyle);

                // Skip existing properties to not duplicate inherited properties
                if (tsInterface.Properties.Any(p => p.Name == propertyName)) {
                    continue;
                }

                var tsType = TSTypeMapper.GetTSPropertyType(property.PropertyType, options, (fullName, typeName) => {
                    if (typeDef.FullName != fullName) {
                        TryAddTSImport(tsInterface, rootTypeDef, fullName, typeName);
                    }

                    return true;
                });

                string? defaultValue = null;

                if (tsType.IsEnum) {
                    defaultValue = property.PropertyType.Resolve().Fields
                        .Where(i => !i.IsSpecialName)
                        .Select(i => i.Name)
                        .FirstOrDefault();
                }

                var tsProperty = new TSInterfaceProperty(
                    propertyName,
                    tsType,
                    property.HasAttribute<TSNullableAttribute>(),
                    defaultValue);

                tsInterface.Properties.Add(tsProperty);
            }

            if (tsInterface.IncludeMethods) {
                foreach (var method in typeDef.Methods) {
                    if (method.IsSpecialName || method.IsConstructor || method.IsStatic || !method.IsPublic || method.HasAttribute<TSExcludeAttribute>()) {
                        continue;
                    }

                    string signatureKey = GetMethodSignatureKey(method);

                    if (tsInterface.Methods.Any(i => i.SignatureKey == signatureKey)) {
                        continue;
                    }

                    var returnType = TSTypeMapper.GetTSPropertyType(method.ReturnType, options, (fullName, typeName) => {
                        if (typeDef.FullName != fullName) {
                            TryAddTSImport(tsInterface, rootTypeDef, fullName, typeName);
                        }

                        return true;
                    });

                    List<TSInterfaceMethodParameter> parameters = [];

                    foreach (var parameter in method.Parameters) {
                        var tsType = TSTypeMapper.GetTSPropertyType(parameter.ParameterType, options, (fullName, typeName) => {
                            if (typeDef.FullName != fullName) {
                                TryAddTSImport(tsInterface, rootTypeDef, fullName, typeName);
                            }

                            return true;
                        });

                        parameters.Add(new TSInterfaceMethodParameter(
                            parameter.Name.ApplyCasing(options.MemberNameCasingStyle),
                            tsType,
                            parameter.HasAttribute<TSNullableAttribute>()));
                    }

                    tsInterface.Methods.Add(new TSInterfaceMethod(
                        method.Name.ApplyCasing(options.MemberNameCasingStyle),
                        returnType,
                        parameters,
                        method.GenericParameters.Select(i => i.Name).ToList(),
                        signatureKey,
                        method.HasAttribute<TSNullableAttribute>()));
                }
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

        private bool IsPublic(PropertyDefinition property) {
            return (property.GetMethod?.IsPublic ?? false) || (property.SetMethod?.IsPublic ?? false);
        }

        private string GetMethodSignatureKey(MethodDefinition method) {
            return $"{method.Name}|{method.GenericParameters.Count}|{string.Join("|", method.Parameters.Select(i => i.ParameterType.FullName))}";
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
            return TSInterfaceTypeScriptGenerator.Generate(tsInterface);
        }
    }
}
