namespace CSharp2TS.CLI.Generators.Enums {
    public class TSEnum {
        public string Name { get; private set; }
        public IList<TSEnumValue> Values { get; private set; } = [];

        public TSEnum(string name) {
            Name = name;
        }
    }
}
