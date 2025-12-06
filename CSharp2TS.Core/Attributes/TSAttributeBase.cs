namespace CSharp2TS.Core.Attributes {
    public abstract class TSAttributeBase : Attribute {
        public string? TypeName { get; }

        /// <summary>
        /// Optional folder path where the generated TypeScript file will be placed.
        /// </summary>
        public string? Folder { get; set; }

        public TSAttributeBase(string? typeName) {
            TypeName = typeName;
        }
    }
}
