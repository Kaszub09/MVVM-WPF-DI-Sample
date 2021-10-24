using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WPFSampleApplication.Model;
using Microsoft.Extensions.DependencyInjection;
using BackgroundProcessSample;

namespace UnitTestProject {
    [TestClass]
    public class BackgroundModelTest {
        [TestMethod]
        public void TestStartStopInteraction() {
            var bgTestProcessManager = ServiceProvider.Service.GetRequiredService<IBackgroundProcessManager>();
            var bgModel = ServiceProvider.Service.GetRequiredService<IBackgroundProcessModel>();

            bgModel.Start();
            Assert.IsTrue(bgTestProcessManager.IsRunning);
            bgModel.Start();
            Assert.IsTrue(bgTestProcessManager.IsRunning);
            bgModel.Stop();
            Assert.IsFalse(bgTestProcessManager.IsRunning);
        }
    }
}
