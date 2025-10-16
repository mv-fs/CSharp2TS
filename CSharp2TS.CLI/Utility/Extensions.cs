using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.CLI.Utility {
    public static class Extensions {
        public static string ToCamelCase(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static bool HasAttribute(this ICustomAttributeProvider entity, Type type) {
            return entity.CustomAttributes
                .Where(a => a.AttributeType.FullName == type.FullName)
                .Any();
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider entity) {
            return entity.HasAttribute(typeof(T));
        }

        public static string? GetCustomFolderLocation(this TypeDefinition typeDef) {
            var attribute = typeDef.CustomAttributes
                .Where(a => a.AttributeType.Resolve().BaseType.FullName == typeof(TSAttributeBase).FullName)
                .FirstOrDefault();

            if (attribute == null) {
                return null;
            }

            return attribute.Properties
                .Where(i => i.Name == nameof(TSAttributeBase.Folder))
                .Select(i => (string)i.Argument.Value)
                .FirstOrDefault();
        }

        public static bool TryGetAttribute<T>(this ICustomAttributeProvider entity, out CustomAttribute? attribute) {
            attribute = entity.CustomAttributes
                .Where(a => a.AttributeType.FullName == typeof(T).FullName)
                .FirstOrDefault();

            return attribute != null;
        }

        public static T? GetAttributeValue<T>(this CustomAttribute attr, string name) {
            return attr.Properties
                .Where(i => i.Name == name)
                .Select(i => (T)i.Argument.Value)
                .FirstOrDefault();
        }

        public static T? GetConstructorArgument<T>(this CustomAttribute attr) {
            if (!attr.HasConstructorArguments) {
                return default;
            }

            return attr.ConstructorArguments[0].Value is T value ? value : default;
        }

        public static bool TryGetHttpAttributeTemplate<T>(this MethodDefinition typeDef, out string template) {
            if (!typeDef.TryGetAttribute<T>(out CustomAttribute? attribute)) {
                template = string.Empty;
                return false;
            }

            if (attribute!.HasConstructorArguments) {
                template = attribute.ConstructorArguments[0].Value.ToString() ?? string.Empty;
            } else {
                template = string.Empty;
            }

            return true;
        }
    }
}
