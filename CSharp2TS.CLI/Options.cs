namespace CSharp2TS.CLI {
    public class Options {
        public bool GenerateModels { get; set; }
        public string? ModelOutputFolder { get; set; }
        public string[] ModelAssemblyPaths { get; set; } = [];

        public bool GenerateServices { get; set; }
        public string? ServicesOutputFolder { get; set; }
        public string[] ServicesAssemblyPaths { get; set; } = [];
        public string ServiceGenerator { get; set; } = Consts.AxiosService;

        public CasingStyle FileNameCasingStyle { get; set; } = CasingStyle.PascalCase;
        public CasingStyle MemberNameCasingStyle { get; set; } = CasingStyle.CamelCase;
        public bool UseNullableStrings { get; set; } = false;
        public Dictionary<string, string> CustomTypeMappings { get; set; } = [];
    }
}
