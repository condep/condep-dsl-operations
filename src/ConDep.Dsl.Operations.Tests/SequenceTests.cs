using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;
using ConDep.Dsl.LoadBalancer;
using ConDep.Dsl.Sequence;
using ConDep.Dsl.Validation;
using NUnit.Framework;

namespace ConDep.Dsl.Tests
{
    [TestFixture]
    public class SequenceTests
    {
        private SequenceTestApp _app;

        [SetUp]
        public void Setup()
        {
            _app = new SequenceTestApp();
        }

        [Test]
        public void TestThatExecutionSequenceIsValid()
        {
            var config = new ConDepEnvConfig {EnvironmentName = "bogusEnv"};
            var server = new ServerConfig { Name = "jat-web03" };
            config.Servers = new[] { server };

            var sequenceManager = new ExecutionSequenceManager(config.Servers, new DefaultLoadBalancer());

            var settings = new ConDepSettings();
            settings.Config = config;

            var local = new LocalOperationsBuilder(sequenceManager.NewLocalSequence("Test"));
            //Configure.LocalOperations = local;
            _app.Configure(local, settings);

            var notification = new Notification();
            Assert.That(sequenceManager.IsValid(notification));
        }
    }

    public class SequenceTestApp : Runbook.Local
    {
        public override void Configure(IOfferLocalOperations local, ConDepSettings settings)
        {
            local.HttpGet("http://www.con-dep.net");
            local.ToEachServer(server =>
                {
                    server
                        .Configure.IIS();

                    server
                        .Execute.PowerShell("ipconfig");
                }
            );
            local.HttpGet("http://blog.torresdal.net");
        }
    }
}