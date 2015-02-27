using System.Web.Configuration;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.IIS.MachineKey
{
    public class SetIisMachineKeyOperation : RemoteCompositeOperation
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

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Set IIS Machine Key"; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            server.Execute.PowerShell(string.Format("Set-ConDepIisMachineKeys {0} {1} {2}", _validationKey, _decryptionKey, _validation));
        }
    }
}