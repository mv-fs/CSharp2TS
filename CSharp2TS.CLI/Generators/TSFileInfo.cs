
using CSharp2TS.CLI.Utility;

namespace CSharp2TS.CLI.Generators.Entities {
    public class TSFileInfo {
        public required string TypeName { get; set; }
        public required string Folder { get; set; }
        public required string FileNameWithoutExtension { get; set; }
        public string FileFullPath => Path.Combine(Folder, FileNameWithoutExtension + ".ts");

        public string GetImportPathTo(TSFileInfo targetType) {
            var folderPath = FolderUtility.GetRelativeImportPath(Folder, targetType.Folder);

            return $"{folderPath}{targetType.FileNameWithoutExtension}";
        }
    }
}
