using CSharp2TS.CLI.Generators.Entities;

namespace CSharp2TS.CLI.Generators.Services {
    public record TSServiceMethod(
        string MethodName,
        string HttpMethod,
        string Route,
        TSProperty ReturnType,
        TSServiceMethodParam[] RouteParams,
        TSServiceMethodParam[] QueryParams,
        TSServiceMethodParam? BodyParam,
        string QueryString) {

        public IList<TSServiceMethodParam> AllParams {
            get {
                List<TSServiceMethodParam> allParams = [.. RouteParams, .. QueryParams];

                if (BodyParam != null) {
                    allParams.Add(BodyParam);
                }

                return allParams;
            }
        }

        public bool IsBodyRawFile => BodyParam?.Property.TSType == TSType.File;
        public bool IsBodyFormData => BodyParam?.Property.TSType == TSType.FormData;
        public bool IsBodyFormObject => BodyParam?.Property.TSType == TSType.Object && BodyParam.IsFormData;
        public bool IsOtherFormObject => BodyParam != null && !IsBodyFormData && !IsBodyFormObject && BodyParam.IsFormData;
        public bool IsResponseFile => ReturnType.TSType == TSType.File;
    }
}
