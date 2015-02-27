using System.DirectoryServices;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin
{
    public class AddUserToLocalGroupOperation : RemoteCompositeOperation
    {
        private readonly string _userName;
        private readonly string _groupName;

        public AddUserToLocalGroupOperation(string userName, string groupName)
        {
            _userName = userName;
            _groupName = groupName;
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }

        public override string Name
        {
            get { return "Add " + _userName + " to " + _groupName; }
        }

        public override void Configure(IOfferRemoteComposition server)
        {
            server.Execute.PowerShell(string.Format("Add-ConDepUserToLocalGroup \"{0}\" \"{1}\"", _groupName, _userName.Replace("\\", "/")));
        }
    }
}