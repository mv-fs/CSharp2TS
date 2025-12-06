using CSharp2TS.CLI.Utility;

namespace CSharp2TS.Tests.Utility {
    public class FolderUtilityTests {
        [Test]
        [TestCase("/path/to/folder", "/path/to/folder", "./")]
        [TestCase("/path/to/folder", "/path/to/folder/subfolder", "./subfolder/")]
        [TestCase("/path/to/folder/subfolder", "/path/to/folder", "../")]
        [TestCase("\\path\\to\\folder", "\\path\\to\\other\\folder", "../other/folder/")]
        [TestCase("C:\\path\\to\\folder", "C:\\path\\to\\other\\folder", "../other/folder/")]
        [TestCase("/base/path", "/other\\path", "../../other/path/")]
        public void GetRelativeImportPath_SameFolder_ReturnsCurrentDirectory(string currentFolder, string targetFolder, string expected) {
            // Act
            string result = FolderUtility.GetRelativeImportPath(currentFolder, targetFolder);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
