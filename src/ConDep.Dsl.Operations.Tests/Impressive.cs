using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Tests
{
    public class Impressive : Runbook
    {
        public override void Execute(IOfferOperations dsl, ConDepSettings settings)
        {
            dsl.Remote(x => x.Execute.PowerShell("asdlfkj"));
        }
    }
}
