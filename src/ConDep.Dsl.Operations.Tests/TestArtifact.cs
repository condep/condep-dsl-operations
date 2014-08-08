using System;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Tests
{
    public class TestArtifact : Artifact.Remote
    {
        public override void Configure(IOfferRemoteOperations server, ConDepSettings settings)
        {
            server.Install.Msi("");
            server.Restart();
            server.Install.Executable(new Uri("http://asdfasdf"), "");
        }
    }
}