using System.Collections.Generic;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.IIS
{
    public class IisInfrastructureOperation : RemoteOperation
    {
        private readonly List<string> _featuresToAdd = new List<string>();
        private readonly List<string> _featuresToRemove = new List<string>();

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            return remote.Configure
                .Windows(win =>
                {
                    win.InstallFeature("Web-Server");
                    win.InstallFeature("Web-WebServer");

                    foreach (var feature in _featuresToAdd)
                    {
                        win.InstallFeature(feature);
                    }

                    foreach (var feature in _featuresToRemove)
                    {
                        win.UninstallFeature(feature);
                    }
                }).Result;
        }

        public override string Name
        {
            get { return "IIS Installer"; }
        }

        public void AddRoleService(string roleService)
        {
            _featuresToAdd.Add(roleService);
        }

        public void RemoveRoleService(string roleService)
        {
            _featuresToRemove.Add(roleService);
        }

    }
}