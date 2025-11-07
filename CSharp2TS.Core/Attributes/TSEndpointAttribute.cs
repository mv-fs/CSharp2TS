namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TSEndpointAttribute : Attribute {
        public string? TsReturnType { get; private set; }
        public Type? ReturnType { get; private set; }

        public TSEndpointAttribute(string returnType) {
            TsReturnType = returnType;
        }

        public TSEndpointAttribute(Type returnType) {
            ReturnType = returnType;
        }
    }
}
