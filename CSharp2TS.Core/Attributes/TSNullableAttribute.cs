namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class TSNullableAttribute : Attribute {
    }
}
