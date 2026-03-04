using System.Text;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public static class TSInterfaceTypeScriptGenerator {
        public static string Generate(TSInterface tsInterface) {
            StringBuilder sb = new();
            string genericString = tsInterface.GenericParameters.Count > 0
                ? $"<{string.Join(", ", tsInterface.GenericParameters)}>"
                : string.Empty;

            sb.AppendLine($"// Auto-generated from {tsInterface.Name}.cs");

            if (tsInterface.Imports.Count > 0) {
                sb.AppendLine();

                foreach (var item in tsInterface.Imports) {
                    sb.AppendLine($"import {item.Name} from '{item.Path}';");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"interface {tsInterface.Name}{genericString} {{");

            foreach (var item in tsInterface.Properties) {
                sb.AppendLine($"  {item};");
            }

            foreach (var method in tsInterface.Methods) {
                sb.AppendLine($"  {method};");
            }

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"export default {tsInterface.Name};");

            if (tsInterface.GenerateClass) {
                sb.AppendLine();
                sb.AppendLine($"export class {tsInterface.Name}Stub{genericString} implements {tsInterface.Name}{genericString} {{");

                foreach (var item in tsInterface.Properties) {
                    sb.AppendLine($"  {item} = {item.GetDefaultValue()};");
                }

                foreach (var method in tsInterface.Methods) {
                    sb.AppendLine();
                    sb.AppendLine($"  {method} {{");
                    sb.AppendLine("    throw new Error('Method not implemented.');");
                    sb.AppendLine("  }");
                }

                sb.AppendLine();
                sb.AppendLine($"  constructor(data?: Partial<{tsInterface.Name}>) {{");
                sb.AppendLine("    if (data) {");
                sb.AppendLine("      Object.assign(this, data);");
                sb.AppendLine("    }");
                sb.AppendLine("  }");
                sb.AppendLine("}");
            }

            return sb.ToString();
        }
    }
}
