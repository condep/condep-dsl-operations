using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Tests
{
    public class Impressive : Runbook.Remote
    {
        public override void Configure(IOfferRemoteOperations server, ConDepSettings settings)
        {
            server.Execute.PowerShell("asdlfkj");
        }
    }
}
