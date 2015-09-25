using ConDep.Dsl.Builders;
using ConDep.Dsl.Config;
using ConDep.Dsl.LoadBalancer;
using ConDep.Dsl.Sequence;
using NUnit.Framework;

namespace ConDep.Dsl.Tests
{
    [TestFixture]
    public class OnlyIfTests
    {
        private ExecutionSequenceManager _sequenceManager;
        private ServerInfo _serverInfo;

        [SetUp]
        public void Setup()
        {
            var app = new OnlyIfTestApp();

            var config = new ConDepEnvConfig { EnvironmentName = "bogusEnv" };
            var server = new ServerConfig { Name = "bogusHost" };
            config.Servers = new[] { server };

            _sequenceManager = new ExecutionSequenceManager(config.Servers, new DefaultLoadBalancer());

            var settings = new ConDepSettings { Config = config };

            var local = new LocalOperationsBuilder(_sequenceManager.NewLocalSequence("Test"));
            app.Configure(local, settings);

            _serverInfo = new ServerInfo {OperatingSystem = new OperatingSystemInfo {Name = "Windows Server 2012"}};
        }

        //[Test]
        //public void TestThat_ConditionIsTrue()
        //{
        //    var remSeq = _sequenceManager._remoteSequences[0];
        //    var compSeq = remSeq._sequence[0] as CompositeConditionalSequence;

        //    Assert.That(compSeq._condition(_serverInfo), Is.True);
        //}
         
    }


    public class OnlyIfTestApp : Runbook.Local
    {
        public override void Configure(IOfferLocalOperations onLocalMachine, ConDepSettings settings)
        {
            onLocalMachine.ToEachServer(server => {

                server
                    .Configure
                        .OnlyIf(x => x.OperatingSystem.Name.StartsWith("Windows"))
                            .IIS();

                server
                    .OnlyIf(x => x.OperatingSystem.Name.StartsWith("Windows"))
                    .Execute.PowerShell("write-host ''");

            }
            );
        }
    }
}