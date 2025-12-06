using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.CLI.Generators.Entities;

namespace CSharp2TS.CLI.Generators.TSServices {
    public record TSServiceMethod(
        string MethodName,
        string HttpMethod,
        string Route,
        TSType ReturnType,
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

        public bool IsBodyRawFile => BodyParam?.Type.TypeName == TSTypeConsts.File;
        public bool IsBodyFormData => BodyParam?.Type.TypeName == TSTypeConsts.FormData;
        public bool IsBodyFormObject => BodyParam != null && BodyParam.Type.IsObject() && BodyParam.IsFormData;
        public bool IsOtherFormObject => BodyParam != null && !IsBodyFormData && !IsBodyFormObject && BodyParam.IsFormData;
        public bool IsResponseFile => ReturnType.TypeName == TSTypeConsts.File;
    }
}
