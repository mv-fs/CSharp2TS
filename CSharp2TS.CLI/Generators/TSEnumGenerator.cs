using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Templates;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;
using System.ComponentModel;

namespace CSharp2TS.CLI.Generators {
    public class TSEnumGenerator : GeneratorBase<TSEnumAttribute> {
        private bool generateDescription;
        private bool generateItemArray;
        private IList<TSEnumProperty> items;

        public TSEnumGenerator(TypeDefinition type, Options options) : base(type, options) {
            items = [];
        }

        public override string Generate() {
            if (Type.TryGetAttribute<TSEnumAttribute>(out var attr)) {
                generateItemArray = attr!.GetAttributeValue<bool>(nameof(TSEnumAttribute.GenerateItemsArray));
                generateDescription = generateItemArray || attr!.GetAttributeValue<bool>(nameof(TSEnumAttribute.GenerateDescriptions));
            }

            ParseTypes();

            return BuildTsFile();
        }

        private void ParseTypes() {
            foreach (var item in Type.Fields) {
                if (item.IsSpecialName) {
                    continue;
                }

                int number = Convert.ToInt32(item.Constant);
                string description = item.Name;

                if (generateDescription && item.TryGetAttribute<DescriptionAttribute>(out var descAttr)) {
                    description = descAttr!.GetConstructorArgument<string>() ?? item.Name;
                }

                items.Add(new TSEnumProperty(item.Name, number, description));
            }
        }

        public override string GetFileName() {
            return ApplyCasing(Type.Name);
        }

        private string BuildTsFile() {
            return new TSEnumTemplate {
                Items = items,
                TypeName = Type.Name,
                GenerateDescriptions = generateDescription,
                GenerateItemsArray = generateItemArray,
            }.TransformText();
        }
    }
}
