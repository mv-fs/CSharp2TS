using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Mono.Cecil;
using System.Text.Json;

namespace CSharp2TS.CLI.Generators {
    public abstract class GeneratorBase<TAttribute> where TAttribute : TSAttributeBase {
        private static readonly Type[] dateTypes = [typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly)];
        private static readonly Type[] stringTypes = [typeof(char), typeof(string), typeof(Guid), .. dateTypes];
        private static readonly Type[] voidTypes = [typeof(void), typeof(Task), typeof(ActionResult), typeof(IActionResult)];
        private static readonly Type[] fileCollectionTypes = [typeof(FormFileCollection), typeof(IFormFileCollection)];
        private static readonly Type[] fileReturnTypes = [typeof(FileContentResult), typeof(FileStreamResult), typeof(FileResult)];
        private static readonly Type[] fileTypes = [typeof(FormFile), typeof(IFormFile), .. fileCollectionTypes, .. fileReturnTypes];
        private static readonly Type[] formDataTypes = [typeof(IFormCollection)];
        private static readonly Type[] unknownTypes = [typeof(JsonElement)];
        private static readonly Type[] numberTypes = [
            typeof(sbyte), typeof(byte), typeof(short),
            typeof(ushort), typeof(int), typeof(uint),
            typeof(long), typeof(ulong), typeof(float),
            typeof(double), typeof(decimal)
        ];

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
            TSType tsType;
            List<TSProperty> genericArguments = new();

            TryExtractFromGenericIfRequired(typeof(Task<>), ref type);
            TryExtractFromGenericIfRequired(typeof(ActionResult<>), ref type);

            int jaggedCount = 0;

            bool isDictionary = TryExtractFromDictionary(ref type);
            bool isCollection = TryExtractFromCollection(ref type, ref jaggedCount);

            bool isNullable = TryExtractFromGenericIfRequired(typeof(Nullable<>), ref type);
            bool isObject = false;
            string? objectName = null;
            bool requiresImport = false;

            if (type.IsGenericInstance) {
                var generic = (GenericInstanceType)type;

                foreach (var arg in generic.GenericArguments) {
                    genericArguments.Add(GetTSPropertyType(arg, currentFolder));
                }
            }

            if (stringTypes.Any(i => SimpleTypeCheck(type, i))) {
                tsType = TSType.String;

                if (!isNullable && SimpleTypeCheck(type, typeof(string)) && Options.UseNullableStrings) {
                    isNullable = true;
                }
            } else if (numberTypes.Any(i => SimpleTypeCheck(type, i))) {
                tsType = TSType.Number;
            } else if (type.FullName == typeof(bool).FullName) {
                tsType = TSType.Boolean;
            } else if (voidTypes.Any(i => SimpleTypeCheck(type, i))) {
                tsType = TSType.Void;
            } else if (fileTypes.Any(i => SimpleTypeCheck(type, i))) {
                isObject = true;
                tsType = TSType.File;

                if (fileCollectionTypes.Any(i => SimpleTypeCheck(type, i))) {
                    isCollection = true;
                    jaggedCount = 1;
                }
            } else if (formDataTypes.Any(i => SimpleTypeCheck(type, i))) {
                isObject = true;
                tsType = TSType.FormData;
            } else if (unknownTypes.Any(i => SimpleTypeCheck(type, i))) {
                tsType = TSType.Unknown;
            } else {
                isObject = true;
                requiresImport = true;
                tsType = TSType.Object;
                objectName = GetCleanedTypeName(type);
            }

            var generationInfo = new TSProperty {
                TSType = tsType,
                GenericArguments = genericArguments,
                IsCollection = isCollection,
                JaggedCount = jaggedCount,
                IsDictionary = isDictionary,
                IsTypeNullable = isNullable,
                IsPropertyNullable = isNullableProperty,
                IsObject = isObject,
                ObjectName = objectName,
                TypeRef = type,
            };

            if (requiresImport && Type != generationInfo.TypeRef) {
                TryAddTSImport(generationInfo, currentFolder, Options.ModelOutputFolder);
            }

            return generationInfo;
        }

        private bool SimpleTypeCheck(TypeReference typeReference, Type type) {
            return typeReference.FullName == type.FullName;
        }

        private bool TryExtractFromDictionary(ref TypeReference type) {
            if (!type.IsGenericInstance) {
                return false;
            }

            bool isDictionary = HasInterface(type, typeof(IDictionary<,>));

            // If it's a dictionary type, extract the generic arguments
            if (isDictionary && type is GenericInstanceType genericInstance && genericInstance.GenericArguments.Count > 1) {
                type = genericInstance.GenericArguments[1];
                return true;
            }

            return false;
        }

        private bool TryExtractFromCollection(ref TypeReference type, ref int currentIteration) {
            if (type.IsArray) {
                type = ((ArrayType)type).ElementType;
                currentIteration++;

                TryExtractFromCollection(ref type, ref currentIteration);

                return true;
            }

            if (!type.IsGenericInstance) {
                return false;
            }

            bool isCollection = HasInterface(type, typeof(IEnumerable<>));

            // If it's a collection type, extract the generic argument
            if (isCollection && type is GenericInstanceType genericInstance && genericInstance.GenericArguments.Count > 0) {
                type = genericInstance.GenericArguments[0];
                currentIteration++;

                TryExtractFromCollection(ref type, ref currentIteration);

                return true;
            }

            return false;
        }

        private bool HasInterface(TypeReference type, Type implementsType) {
            return type.GetElementType().FullName == implementsType.FullName ||
                type.Resolve().Interfaces
                    .Where(i => i.InterfaceType.IsGenericInstance)
                    .Where(i => SimpleTypeCheck(i.InterfaceType.GetElementType(), implementsType))
                    .Any();
        }

        private bool TryExtractFromGenericIfRequired(Type type, ref TypeReference typeRef) {
            if (typeRef.IsGenericParameter) {
                return false;
            }

            if (typeRef.Resolve().FullName != type.FullName) {
                return false;
            }

            typeRef = ((GenericInstanceType)typeRef).GenericArguments[0];

            return true;
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
            if (type.HasGenericParameters || (type is GenericInstanceType genericType && genericType.GenericArguments.Count > 0)) {
                return type.Name.Split('`')[0];
            }

            return type.Name;
        }
    }
}
