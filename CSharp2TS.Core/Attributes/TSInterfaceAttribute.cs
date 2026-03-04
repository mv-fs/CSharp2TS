namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TSInterfaceAttribute : TSAttributeBase {
        /// <summary>
        /// Generate a method to return an object implementing the interface with all properties set to default values.
        /// </summary>
        public bool GenerateClass { get; set; }

        /// <summary>
        /// Include public methods in generated TypeScript interfaces/classes.
        /// </summary>
        public bool IncludeMethods { get; set; }

        public TSInterfaceAttribute() : base(null) {
        }

        public TSInterfaceAttribute(bool includeMethods) : base(null) {
            IncludeMethods = includeMethods;
        }

        public TSInterfaceAttribute(string typeName) : base(typeName) {
        }
    }
}
