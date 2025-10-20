using CSharp2TS.Core.Enums;

namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TSEndpointAttribute : Attribute {
        public ReturnType ReturnType { get; private set; }

        public TSEndpointAttribute(ReturnType returnType) {
            ReturnType = returnType;
        }
    }
}
