using Eurotrash.GrimDawn.Core.Discovery.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Eurotrash.GrimDawn.Test
{
    [TestClass]
    public class GameFilesTest
    {
        [TestMethod]
        public void GetInstallDirectoryAsyncTest()
        {
            Assert.IsNotNull(GameFiles.GetInstallDirectoryAsync().Result, "Installation of Grim Dawn was not found on this computer.");
        }

        [TestMethod]
        public void GetGrimDawnExeAsyncTest()
        {
            Assert.IsNotNull(GameFiles.GetGrimDawnExeAsync().Result);
        }
    }
}