using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin
{
    public class AddUserToLocalGroupOperation : RemoteOperation
    {
        private readonly string _userName;
        private readonly string _groupName;

        public AddUserToLocalGroupOperation(string userName, string groupName)
        {
            _userName = userName;
            _groupName = groupName;
        }

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Execute.PowerShell(string.Format("Add-ConDepUserToLocalGroup \"{0}\" \"{1}\"", _groupName, _userName.Replace("\\", "/"))).Result;
        }

        public override string Name
        {
            get { return "Add " + _userName + " to " + _groupName; }
        }
    }
}