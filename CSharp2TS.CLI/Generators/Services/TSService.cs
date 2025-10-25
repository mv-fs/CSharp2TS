using CSharp2TS.CLI.Generators.Entities;

namespace CSharp2TS.CLI.Generators.Services {
    public class TSService {
        public string Name { get; set; }
        public IList<TSImport> Imports { get; private set; } = [];
        public IList<TSServiceMethod> Methods { get; private set; } = [];
        public string ApiClientImportPath { get; set; } = "./";
        public bool ImportFormFactory { get; set; }
        public bool ImportProgressEvent { get; set; }

        public TSService(string name) {
            Name = name;
        }
    }
}
