using ConDep.Dsl.Operations.Infrastructure.RestartComputer;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.FileStructure;
using ConDep.Dsl.Operations.Remote.Infrastructure.Windows.UserAdmin;

namespace ConDep.Dsl
{
    public static class RemoteExtensions
    {
        /// <summary>
        /// Creates a directory, if it not already exists.
        /// </summary>
        /// <param name="remote"></param>
        /// <param name="path">Directory path</param>
        /// <returns></returns>
        public static IOfferRemoteOperations CreateDirectory(this IOfferRemoteOperations remote, string path)
        {
            var operation = new CreateDirectoryOperation(path);
            Configure.Operation(remote, operation);
            return remote;
        }

        public static IOfferRemoteOperations Restart(this IOfferRemoteOperations remote, int delayInSeconds = 10)
        {
            var restartOperation = new RestartComputerOperation(delayInSeconds);
            Configure.Operation(remote, restartOperation);
            return remote;
        }

        /// <summary>
        /// Adds user to group
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="userName">Username</param>
        /// <param name="groupName">Group name</param>
        /// <returns></returns>
        public static IOfferRemoteOperations AddUserToLocalGroup(this IOfferRemoteOperations configuration, string userName, string groupName)
        {
            var operation = new AddUserToLocalGroupOperation(userName, groupName);
            Configure.Operation(configuration, operation);
            return configuration;
        }
    }
}