using System;
using System.IO;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Application.Local.PreCompile;
using ConDep.Dsl.Operations.Local.PreCompile;
using ConDep.Dsl.Validation;
using Moq;
using NUnit.Framework;

namespace ConDep.Dsl.Tests.Operations.Local
{
    [TestFixture]
    public class PreCompileTests
    {
        private Mock<IWrapClientBuildManager> _buildManager;
        private string _validWebAppPath;
        private string _validOutputPath;
        private ConDepSettings _settingsDefault;
        private CancellationToken _token;

        [SetUp]
        public void Setup()
        {
            var tokenSource = new CancellationTokenSource();
            _token = tokenSource.Token;
            _buildManager = new Mock<IWrapClientBuildManager>();
            _buildManager.Setup(x => x.PrecompileApplication(It.IsAny<PreCompileCallback>()));
            _validWebAppPath = Path.GetTempPath();
            _validOutputPath = Environment.CurrentDirectory;
            _settingsDefault = new ConDepSettings
            {
                Options =
                {
                    SuspendMode = LoadBalancerSuspendMethod.Graceful
                }
            };
        }

        [Test]
        public void TestThatPreCompileExecutesSuccessfully()
        {
            var operation = new PreCompileOperation("MyWebApp", @"C:\temp\MyWebApp", @"C:\temp\MyWebAppCompiled", _buildManager.Object);
            
            var status = new StatusReporter();
            operation.Execute(_settingsDefault, _token);

            Assert.That(status.HasErrors, Is.False);
            _buildManager.Verify(manager => manager.PrecompileApplication(It.IsAny<PreCompileCallback>()));
        }
    }
}