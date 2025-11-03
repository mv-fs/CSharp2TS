using Mono.Cecil;

namespace CSharp2TS.CLI.Utility {
    public static class Extensions {
        public static bool HasAttribute(this ICustomAttributeProvider entity, Type type) {
            return entity.CustomAttributes
                .Where(a => a.AttributeType.FullName == type.FullName)
                .Any();
        }

        public static bool HasAttribute<T>(this ICustomAttributeProvider entity) {
            return entity.HasAttribute(typeof(T));
        }

        public static bool TryGetAttribute<T>(this ICustomAttributeProvider entity, out CustomAttribute attribute) {
            var customAttribute = entity.CustomAttributes
                .Where(a => a.AttributeType.FullName == typeof(T).FullName)
                .FirstOrDefault();

            if (customAttribute == null) {
                attribute = null!;
                return false;
            }

            attribute = customAttribute;

            return true;
        }

        public static bool TryGetBaseAttribute<T>(this ICustomAttributeProvider entity, out CustomAttribute attribute) {
            var customAttribute = entity.CustomAttributes
                .Where(a => a.AttributeType.Resolve().BaseType.FullName == typeof(T).FullName)
                .FirstOrDefault();

            if (customAttribute == null) {
                attribute = null!;
                return false;
            }

            attribute = customAttribute;

            return true;
        }

        public static T? GetAttributeValue<T>(this CustomAttribute attr, string name) {
            return attr.Properties
                .Where(i => i.Name == name)
                .Select(i => (T)i.Argument.Value)
                .FirstOrDefault();
        }

        public static T? GetConstructorArgument<T>(this CustomAttribute attr, int index = 0) {
            if (!attr.HasConstructorArguments || attr.ConstructorArguments.Count - 1 < index) {
                return default;
            }

            return attr.ConstructorArguments[index].Value is T value ? value : default;
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
