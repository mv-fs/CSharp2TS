using CSharp2TS.CLI.Generators.Entities;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using Mono.Cecil;
using System.Text;

namespace CSharp2TS.CLI.Generators.Services {
    public class TSAxiosServiceGenerator {
        private const string oldAppendedFileName = "Controller";
        private const string newAppendedFileName = "Service";
        private readonly Dictionary<string, TSFileInfo> files;
        private readonly Options options;

        public TSAxiosServiceGenerator(Options options, Dictionary<string, TSFileInfo> files) {
            this.options = options;
            this.files = files;
        }

        public string Generate(TypeDefinition typeDef) {
            TSService service = new(NameUtility.GetName(typeDef));

            ParseTypes(service, typeDef);

            return BuildTsFile(service);
        }

        private void ParseTypes(TSService service, TypeDefinition typeDef) {
            var methods = typeDef.Methods;
            service.ApiClientImportPath = GetApiClientImport(typeDef);

            foreach (var method in methods) {
                if (method == null || method.IsSpecialName || method.HasAttribute<TSExcludeAttribute>()) {
                    continue;
                }

                var httpMethodAttribute = GetHttpAttribute(method);

                if (httpMethodAttribute == null) {
                    continue;
                }

                string name = GetMethodName(service, method.Name.ToCamelCase(), null);
                string route = GetRoute(typeDef, httpMethodAttribute);
                var returnType = GetReturnType(service, typeDef, method);

                var allParams = ParseParams(service, typeDef, method.Parameters.ToArray());
                var routeParams = GetRouteParams(route, allParams);
                var queryParams = GetQueryParams(route, allParams);
                TSServiceMethodParam? bodyParam = null;

                if (httpMethodAttribute.HttpMethod != Consts.HttpGet) {
                    bodyParam = GetBodyParam(route, allParams);
                }

                string queryString = GetQueryString(route, queryParams);

                service.Methods.Add(new TSServiceMethod(
                    name,
                    httpMethodAttribute.HttpMethod,
                    route,
                    returnType,
                    routeParams,
                    queryParams,
                    bodyParam,
                    queryString));
            }

            service.ImportFormFactory = service.Methods.Any(i => i.IsBodyFormObject);
            service.ImportProgressEvent = service.Methods.Any(i => i.IsBodyRawFile);
        }

        private string GetMethodName(TSService service, string name, int? count) {
            if (service.Methods.Any(i => i.MethodName.Equals(name + count, StringComparison.OrdinalIgnoreCase))) {
                return GetMethodName(service, name, count == null ? 2 : count + 1);
            }

            return name + count;
        }

        private List<TSServiceMethodParam> ParseParams(TSService service, TypeDefinition typeDef, ParameterDefinition[] parameterDefinitions) {
            List<TSServiceMethodParam> converted = [];

            foreach (ParameterDefinition param in parameterDefinitions) {
                var tsProperty = GetTSPropertyType(service, typeDef, param.ParameterType, options.ServicesOutputFolder!);
                bool isFormObject = param.HasAttribute<FromFormAttribute>() && tsProperty.TSType != TSType.FormData;
                bool isBodyParam = param.HasAttribute<FromBodyAttribute>() || isFormObject || !tsProperty.TypeRef.Resolve().IsEnum && tsProperty.IsObject;

                converted.Add(new TSServiceMethodParam(param.Name.ToCamelCase(), tsProperty, isBodyParam, isFormObject));
            }

            return converted;
        }

        private string GetApiClientImport(TypeDefinition typeDef) {
            string currentFolder = Path.Combine(options.ServicesOutputFolder!, files[typeDef.FullName].Folder);
            return FolderUtility.GetRelativeImportPath(currentFolder, options.ServicesOutputFolder!);
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

        private string GetRoute(TypeDefinition typeDef, HttpAttribute httpMethodAttribute) {
            string controllerRoute;
            string controllerName = StripController(typeDef.Name).ToLowerInvariant();

            if (typeDef.TryGetAttribute<RouteAttribute>(out CustomAttribute? attribute)) {
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
                // Add null check for strings to avoid passing "null" in the query string
                bool addNullCheck = param.Property.IsNullable || param.Property.TSType == TSType.String;

                querySections.Add($"{param.Name}=${{{param.Name}{(addNullCheck ? " ?? ''" : string.Empty)}}}");
            }

            if (querySections.Count == 0) {
                return string.Empty;
            }

            return $"?{string.Join('&', querySections)}";
        }

        private TSProperty GetReturnType(TSService service, TypeDefinition typeDef, MethodDefinition method) {
            if (method.TryGetAttribute<TSEndpointAttribute>(out CustomAttribute? attribute)) {
                var customReturnType = attribute!.ConstructorArguments[0].Value as TypeReference;

                if (customReturnType != null) {
                    return GetTSPropertyType(service, typeDef, customReturnType, options.ServicesOutputFolder!);
                }
            }

            return GetTSPropertyType(service, typeDef, method.ReturnType, options.ServicesOutputFolder!);
        }

        public static TSFileInfo GetFileInfo(TypeDefinition typeDef, Options options) {
            string typeName = StripController(typeDef.Name) + newAppendedFileName;

            return new TSFileInfo {
                Folder = options.ServicesOutputFolder!,
                TypeName = typeName,
                FileNameWithoutExtension = NameUtility.ApplyCasing(typeName, options),
            };
        }

        private static string StripController(string str) {
            if (str.EndsWith(oldAppendedFileName, StringComparison.OrdinalIgnoreCase)) {
                str = str[..^oldAppendedFileName.Length];
            }

            return str;
        }

        private TSProperty GetTSPropertyType(TSService service, TypeDefinition typeDef, TypeReference type, string currentFolder, bool isNullableProperty = false) {
            return TSTypeMapper.GetTSPropertyType(type, options, isNullableProperty, (tsProperty) => {
                if (typeDef != tsProperty.TypeRef) {
                    TryAddTSImport(service, tsProperty, currentFolder, options.ModelOutputFolder);
                }

                return true;
            });
        }

        private void TryAddTSImport(TSService service, TSProperty tsType, string? currentFolderRoot, string? targetFolderRoot) {
            if (tsType.IsObject && !string.IsNullOrEmpty(tsType.ObjectName)) {
                string importPath = FolderUtility.GetRelativeImportPath(currentFolderRoot ?? "", targetFolderRoot ?? "");

                if (!service.Imports.Any(i => i.Name == tsType.ObjectName)) {
                    service.Imports.Add(new TSImport(tsType.ObjectName, importPath + tsType.ObjectName));
                }
            }
        }

        private string GetCleanedTypeName(TypeReference type) {
            return type.Name.Replace("`", "").Replace("&", "");
        }

        #region Build File

        private string BuildTsFile(TSService service) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"// Auto-generated from {service.Name}.cs");
            sb.AppendLine();
            sb.AppendLine($"import {{ apiClient{(service.ImportFormFactory ? ", FormDataFactory" : string.Empty)} }} from '{service.ApiClientImportPath}apiClient';");

            if (service.ImportProgressEvent) {
                sb.AppendLine("import type { AxiosProgressEvent } from 'axios';");
            }

            foreach (var import in service.Imports) {
                sb.AppendLine($"import {import.Name} from '{import.Path}';");
            }

            sb.AppendLine();
            sb.AppendLine("export default {");

            for (int i = 0; i < service.Methods.Count; i++) {
                BuildMethod(sb, service.Methods[i]);

                if (i != service.Methods.Count - 1) {
                    sb.AppendLine();
                }
            }

            sb.AppendLine("};");

            return sb.ToString();
        }

        private void BuildMethod(StringBuilder sb, TSServiceMethod method) {
            BuildMethodSignature(sb, method);
            bool useFormData = BuildFormDataCreation(sb, method);

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

            bool shouldAddFormHeader = useFormData || method.IsBodyFormData || method.IsOtherFormObject;

            if (method.IsResponseFile || shouldAddFormHeader || method.IsBodyRawFile) {
                BuildOptions(sb, method, shouldAddFormHeader);
            } else {
                sb.Append(");");
                sb.AppendLine();
            }

            if (method.ReturnType.TSType != TSType.Void) {
                sb.AppendLine($"    return response.data;");
            }

            sb.AppendLine($"  }},");
        }

        private void BuildMethodSignature(StringBuilder sb, TSServiceMethod method) {
            sb.Append($"  async {method.MethodName}(");
            sb.Append(string.Join(", ", method.AllParams.Select(i => $"{i.Name}: {i.Property}")));

            if (method.IsBodyRawFile) {
                sb.Append(", onUploadProgress?: (event: AxiosProgressEvent) => void");
            }

            sb.AppendLine($"): Promise<{method.ReturnType}> {{");
        }

        private void BuildOptions(StringBuilder sb, TSServiceMethod method, bool shouldAddFormHeader) {
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
        }

        private bool BuildFormDataCreation(StringBuilder sb, TSServiceMethod method) {
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

            return useFormData;
        }

        #endregion

        private record HttpAttribute(string HttpMethod, string? Template);
    }
}
