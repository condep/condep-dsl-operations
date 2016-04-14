using System.Threading;
using System.Web.Configuration;
using ConDep.Dsl.Config;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.IIS.MachineKey
{
    public class SetIisMachineKeyOperation : RemoteOperation
    {
        private readonly string _validationKey;
        private readonly string _decryptionKey;
        private readonly MachineKeyValidation _validation;

        public SetIisMachineKeyOperation(string validationKey, string decryptionKey, MachineKeyValidation validation)
        {
            _validationKey = validationKey;
            _decryptionKey = decryptionKey;
            _validation = validation;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Execute.PowerShell(string.Format("Set-ConDepIisMachineKeys {0} {1} {2}", _validationKey, _decryptionKey, _validation)).Result;
        }

        public override string Name
        {
            get { return "Set IIS Machine Key"; }
        }
    }
}