namespace CSharp2TS.Core.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class TSImportAttribute : Attribute {
        public string Name { get; }
        public string Path { get; }

        public TSImportAttribute(string name, string path) {
            Name = name;
            Path = path;
        }
    }
}
