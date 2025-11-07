using CSharp2TS.CLI.Generators.Entities;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public class TSInterface {
        public string Name { get; private set; }
        public IList<TSImport> Imports { get; private set; } = [];
        public IList<TSInterfaceProperty> Properties { get; private set; } = [];
        public IList<string> GenericParameters { get; set; } = [];
        public bool GenerateClass { get; set; }

        public TSInterface(string name) {
            Name = name;
        }
    }
}
