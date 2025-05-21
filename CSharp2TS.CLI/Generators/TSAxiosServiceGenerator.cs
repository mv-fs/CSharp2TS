using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mono.Cecil;
using System.Text;

namespace CSharp2TS.CLI.Generators {
    public class TSAxiosServiceGenerator : GeneratorBase<TSServiceAttribute> {
        private readonly string oldAppendedFileName = "Controller";
        private readonly string newAppendedFileName = "Service";

        private bool importFormFactory = false;
        private bool importProgressEvent = false;
        private string apiClientImportPath;
        private IList<TSServiceMethod> items;

        public TSAxiosServiceGenerator(TypeDefinition type, Options options) : base(type, options) {
            apiClientImportPath = "./";
            items = [];
        }

        public override string Generate() {
            ParseTypes();

            return BuildTsFile();
        }

        private void ParseTypes() {
            var methods = Type.Methods;
            apiClientImportPath = GetApiClientImport();

            foreach (var method in methods) {
                if (method == null || method.IsSpecialName || method.HasAttribute<TSExcludeAttribute>()) {
                    continue;
                }

                var httpMethodAttribute = GetHttpAttribute(method);

                if (httpMethodAttribute == null) {
                    continue;
                }

                string name = GetMethodName(method.Name.ToCamelCase(), null);
                string route = GetRoute(httpMethodAttribute);
                var returnType = GetReturnType(method);

                var allParams = ParseParams(method.Parameters.ToArray());
                var routeParams = GetRouteParams(route, allParams);
                var queryParams = GetQueryParams(route, allParams);
                TSServiceMethodParam? bodyParam = null;

                if (httpMethodAttribute.HttpMethod != Consts.HttpGet) {
                    bodyParam = GetBodyParam(route, allParams);
                }

                string queryString = GetQueryString(route, queryParams);

                items.Add(new TSServiceMethod(
                    name,
                    httpMethodAttribute.HttpMethod,
                    route,
                    returnType,
                    routeParams,
                    queryParams,
                    bodyParam,
                    queryString));
            }

            importFormFactory = items.Any(i => i.IsBodyFormObject);
            importProgressEvent = items.Any(i => i.IsBodyRawFile);
        }

        private string GetMethodName(string name, int? count) {
            if (items.Any(i => i.MethodName.Equals(name + count, StringComparison.OrdinalIgnoreCase))) {
                return GetMethodName(name, count == null ? 2 : count + 1);
            }

            return name + count;
        }

        private List<TSServiceMethodParam> ParseParams(ParameterDefinition[] parameterDefinitions) {
            List<TSServiceMethodParam> converted = [];

            foreach (ParameterDefinition param in parameterDefinitions) {
                var tsProperty = GetTSPropertyType(param.ParameterType, Options.ServicesOutputFolder!);
                bool isBodyParam = param.HasAttribute<FromBodyAttribute>() || (!tsProperty.TypeRef.Resolve().IsEnum && tsProperty.IsObject);
                bool isFormData = param.HasAttribute<FromFormAttribute>() && tsProperty.TSType != TSType.FormData;

                converted.Add(new TSServiceMethodParam(param.Name.ToCamelCase(), tsProperty, isBodyParam, isFormData));
            }

            return converted;
        }

        private string GetApiClientImport() {
            string currentFolder = Path.Combine(Options.ServicesOutputFolder!, GetFolderLocation());
            return FolderUtility.GetRelativeImportPath(currentFolder, Options.ServicesOutputFolder!);
        }

        private TSServiceMethodParam[] GetRouteParams(string template, List<TSServiceMethodParam> allParams) {
            if (string.IsNullOrWhiteSpace(template)) {
                return [];
            }

            var routeParams = allParams
                .Where(i => template.Contains($"{{{i.Name}}}"))
                .Where(row => !row.IsBodyParam)
                .ToArray();

            foreach (var item in routeParams) {
                allParams.Remove(item);
            }

            return routeParams;
        }

        private TSServiceMethodParam[] GetQueryParams(string template, List<TSServiceMethodParam> allParams) {
            if (string.IsNullOrWhiteSpace(template)) {
                return [];
            }

            var queryParams = allParams
                .Where(row => !row.IsBodyParam)
                .ToArray();

            foreach (var item in queryParams) {
                allParams.Remove(item);
            }

            return queryParams;
        }

        private TSServiceMethodParam? GetBodyParam(string template, List<TSServiceMethodParam> allParams) {
            return allParams
                .Where(row => row.IsBodyParam)
                .FirstOrDefault();
        }

        private HttpAttribute? GetHttpAttribute(MethodDefinition methodDefinition) {
            string template;

            if (methodDefinition.TryGetHttpAttributeTemplate<HttpGetAttribute>(out template)) {
                return new HttpAttribute(Consts.HttpGet, template);
            } else if (methodDefinition.TryGetHttpAttributeTemplate<HttpPostAttribute>(out template)) {
                return new HttpAttribute(Consts.HttpPost, template);
            } else if (methodDefinition.TryGetHttpAttributeTemplate<HttpPutAttribute>(out template)) {
                return new HttpAttribute(Consts.HttpPut, template);
            } else if (methodDefinition.TryGetHttpAttributeTemplate<HttpDeleteAttribute>(out template)) {
                return new HttpAttribute(Consts.HttpDelete, template);
            } else if (methodDefinition.TryGetHttpAttributeTemplate<HttpPatchAttribute>(out template)) {
                return new HttpAttribute(Consts.HttpPatch, template);
            }

            return null;
        }

        private string GetRoute(HttpAttribute httpMethodAttribute) {
            string controllerRoute;
            string controllerName = StripController(Type.Name).ToLowerInvariant();

            if (Type.TryGetAttribute<RouteAttribute>(out CustomAttribute? attribute)) {
                string? controllerTemplate = (string)attribute!.ConstructorArguments[0].Value;
                controllerRoute = controllerTemplate.Replace("[controller]", controllerName, StringComparison.OrdinalIgnoreCase);
            } else {
                controllerRoute = controllerName;
            }

            if (!string.IsNullOrWhiteSpace(httpMethodAttribute.Template)) {
                var template = GeneratorUtility.GetCleanRouteConstraints(httpMethodAttribute.Template);
                controllerRoute += "/" + template;
            }

            return controllerRoute;
        }

        private string GetQueryString(string template, TSServiceMethodParam[] queryParameters) {
            if (string.IsNullOrEmpty(template)) {
                return string.Empty;
            }

            IList<string> querySections = [];

            foreach (var param in queryParameters) {
                querySections.Add($"{param.Name}=${{{param.Name}{(param.Property.IsNullable ? " ?? ''" : string.Empty)}}}");
            }

            if (querySections.Count == 0) {
                return string.Empty;
            }

            return $"?{string.Join('&', querySections)}";
        }

        private TSProperty GetReturnType(MethodDefinition method) {
            if (method.TryGetAttribute<TSEndpointAttribute>(out CustomAttribute? attribute)) {
                var customReturnType = attribute!.ConstructorArguments[0].Value as TypeReference;

                if (customReturnType != null) {
                    return GetTSPropertyType(customReturnType, Options.ServicesOutputFolder!);
                }
            }

            return GetTSPropertyType(method.ReturnType, Options.ServicesOutputFolder!);
        }

        public override string GetFileName() {
            return ApplyCasing(StripController(Type.Name) + newAppendedFileName);
        }

        private string StripController(string str) {
            if (str.EndsWith(oldAppendedFileName, StringComparison.OrdinalIgnoreCase)) {
                str = str[..^oldAppendedFileName.Length];
            }

            return str;
        }

        private string BuildTsFile() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Auto-generated from {Type.Name}.cs");
            sb.AppendLine();
            sb.AppendLine($"import {{ apiClient{(importFormFactory ? ", FormDataFactory" : string.Empty)} }} from '{apiClientImportPath}apiClient';");

            if (importProgressEvent) {
                sb.AppendLine("import type { AxiosProgressEvent } from 'axios';");
            }

            foreach (var import in Imports) {
                sb.AppendLine($"import {import.Value.Name} from '{import.Value.Path}';");
            }

            sb.AppendLine();
            sb.AppendLine("export default {");

            for (int i = 0; i < items.Count; i++) {
                BuildMethod(sb, items[i]);

                if (i != items.Count - 1) {
                    sb.AppendLine();
                }
            }

            sb.AppendLine("};");

            return sb.ToString();
        }

        private void BuildMethod(StringBuilder sb, TSServiceMethod method) {
            sb.AppendLine($"  async {method.MethodName}({string.Join(", ", method.AllParams.Select(i => $"{i.Name}: {i.Property}"))}" +
                $"{(method.IsBodyRawFile ? ", onUploadProgress?: (event: AxiosProgressEvent) => void" : string.Empty)}):" +
                $" Promise<{method.ReturnType}> {{");

            bool useFormData = false;

            if (method.IsBodyRawFile) {
                useFormData = true;

                sb.AppendLine($"    const formData = new FormData();");

                if (method.BodyParam!.Property.IsCollection) {
                    sb.AppendLine($"    for (let i = 0; i < {method.BodyParam.Name}.length; i++) {{");
                    sb.AppendLine($"      const f = {method.BodyParam.Name}[i];");
                    sb.AppendLine($"      formData.append('{method.BodyParam.Name}[' + i + ']', f);");
                    sb.AppendLine($"    }}");
                } else {
                    sb.AppendLine($"    formData.append('{method.BodyParam.Name}', {method.BodyParam.Name});");
                }

                sb.AppendLine();
            } else if (method.IsBodyFormObject) {
                useFormData = true;
                sb.AppendLine($"    const formData = FormDataFactory.Create({method.BodyParam!.Name});");
                sb.AppendLine();
            }

            sb.Append("    ");

            if (method.ReturnType.TSType != TSType.Void) {
                sb.Append("const response = ");
            }

            sb.Append($"await apiClient.instance.{method.HttpMethod}");

            if (method.ReturnType.TSType != TSType.Void) {
                sb.Append($"<{method.ReturnType}>");
            }

            sb.Append($"(`{method.Route}{(method.QueryString.Length > 0 ? method.QueryString : string.Empty)}`");

            if (useFormData) {
                sb.Append(", formData");
            } else if (method.BodyParam != null) {
                sb.Append($", {method.BodyParam.Name}");
            }

            bool shouldAddFormHeader = useFormData || method.IsBodyFormData;

            if (method.IsResponseFile || shouldAddFormHeader || method.IsBodyRawFile) {
                sb.Append(", {");
                sb.AppendLine();

                if (method.IsResponseFile) {
                    sb.AppendLine("      responseType: 'blob',");
                }

                if (shouldAddFormHeader) {
                    sb.AppendLine("      headers: { 'Content-Type': 'multipart/form-data' },");
                }

                if (method.IsBodyRawFile) {
                    sb.AppendLine("      onUploadProgress,");
                }

                sb.AppendLine("    });");
            } else {
                sb.Append(");");
                sb.AppendLine();
            }

            if (method.ReturnType.TSType != TSType.Void) {
                sb.AppendLine($"    return response.data;");
            }

            sb.AppendLine($"  }},");
        }

        private record HttpAttribute(string HttpMethod, string? Template);
    }
}
