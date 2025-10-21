using CSharp2TS.CLI.Generators.Utilities;
using CSharp2TS.CLI.Templates;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;
using System.ComponentModel;

namespace CSharp2TS.CLI.Generators.Enums {
    public class TSEnumGenerator {
        public string Generate(TypeDefinition typeDef) {
            TSEnum tsEnum = new TSEnum(NameUtility.GetName(typeDef));

            typeDef.TryGetAttribute<TSEnumAttribute>(out var attr);
            bool generateItemArray = attr!.GetAttributeValue<bool>(nameof(TSEnumAttribute.GenerateItemsArray));
            bool generateDescriptions = generateItemArray || attr!.GetAttributeValue<bool>(nameof(TSEnumAttribute.GenerateDescriptions));

            ParseTypes(tsEnum, typeDef, generateDescriptions);

            return BuildTsFile(tsEnum, generateDescriptions, generateItemArray);
        }

        private void ParseTypes(TSEnum tsEnum, TypeDefinition typeDef, bool generateDescriptions) {
            foreach (var item in typeDef.Fields) {
                if (item.IsSpecialName) {
                    continue;
                }

                int number = Convert.ToInt32(item.Constant);
                string description = item.Name;

                if (generateDescriptions && item.TryGetAttribute<DescriptionAttribute>(out var descAttr)) {
                    description = descAttr!.GetConstructorArgument<string>() ?? item.Name;
                }

                tsEnum.Values.Add(new TSEnumValue(item.Name, number, description));
            }
        }

        private string BuildTsFile(TSEnum tsEnum, bool generateDescriptions, bool generateItemArray) {
            return new TSEnumTemplate {
                TSEnum = tsEnum,
                GenerateDescriptions = generateDescriptions,
                GenerateItemsArray = generateItemArray,
            }.TransformText();
        }
    }
}
