using System.Diagnostics;
using System.IO;
using System.Linq;
using Eurotrash.GrimDawn.Core.Discovery.Game;
using Eurotrash.GrimDawn.Core.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eurotrash.GrimDawn.Test
{
    [TestClass]
    public class GrimDawnArcFileTest
    {
        private string arcFilePath;

        [TestInitialize]
        public void Initialize()
        {
            arcFilePath = Path.Combine(GameFiles.GetResourcesDirectoryAsync().Result.FullName, "UI.arc");
        }


        [TestMethod]
        public void ArcFileLoadTest()
        {
            using (var file = GrimDawnArcFile.Load(arcFilePath))
            {
                Assert.IsNotNull(file.Header);
                Assert.IsTrue(file.Header.Version > 0);
                Assert.IsTrue(file.Header.NumberOfFileEntries > 0);
            }
        }

        [TestMethod]
        public void ArcFileReadFileNamesTest()
        {
            using (var file = GrimDawnArcFile.Load(arcFilePath))
            {
                var filenames = file.ReadFileNamesAsync().Result;
                Assert.IsNotNull(filenames);
                Assert.IsTrue(filenames.Count > 0);
            }
        }
    }
}