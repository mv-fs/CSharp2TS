using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Utility {
    public static class NameUtility {
        public static string GetName(TypeDefinition typeDef) {
            if (!typeDef.TryGetBaseAttribute<TSAttributeBase>(out var attr)) {
                return typeDef.Name.Split('`')[0];
            }

            string? customName = attr.GetConstructorArgument<string>();

            return customName ?? typeDef.Name.Split('`')[0];
        }

        public static TSFileInfo GetFileDetails(TypeDefinition type, Options options, string basePath) {
            string typeName = GetName(type);
            string? customFolder = GetCustomFolderLocation(type);
            string folder = Path.Combine(basePath, customFolder ?? string.Empty);

            return new TSFileInfo {
                TypeName = typeName,
                Folder = folder,
                FileNameWithoutExtension = typeName.ApplyCasing(options),
            };
        }

        private static string? GetCustomFolderLocation(TypeDefinition typeDef) {
            var attribute = typeDef.CustomAttributes
                .Where(a => a.AttributeType.Resolve().BaseType.FullName == typeof(TSAttributeBase).FullName)
                .FirstOrDefault();

            if (attribute == null) {
                return null;
            }

            return attribute.Properties
                .Where(i => i.Name == nameof(TSAttributeBase.Folder))
                .Select(i => (string?)i.Argument.Value)
                .FirstOrDefault();
        }
    }
}
