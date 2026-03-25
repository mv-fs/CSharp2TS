namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TSDefaultAttribute : Attribute {
        public string Value { get; }

        public TSDefaultAttribute(string value) {
            Value = value;
        }
    }
}
