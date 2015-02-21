using System.DirectoryServices;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows
{
    public class AddUserToGroupOperation : RemoteServerOperation
    {
        private readonly string _userName;
        private readonly string _groupName;

        public AddUserToGroupOperation(string userName, string groupName)
            : base(userName, groupName)
        {
            _userName = userName;
            _groupName = groupName;
        }

        public override void Execute(ILogForConDep logger)
        {
            try
            {
                var group = new DirectoryEntry("WinNT://./" + _groupName);
                logger.Info(string.Format("Adding user {0} to group {1}", _userName, group.Name));

                group.Invoke("Add", "WinNT://" + _userName.Replace("\\", "/"));
                logger.Info("User added");
            }
            catch
            {
                logger.Warn("Failed to add user to group. Is user allready added to group?");
            }
        }

        public override string Name
        {
            get { return "Add " + _userName + " to " + _groupName; }
        }
    }
}