using System.Collections.Generic;
using System.Threading;
using ConDep.Dsl.Config;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows
{
    public class WindowsFeatureInfrastructureOperation : RemoteOperation
    {
        private readonly List<string> _featuresToAdd = new List<string>();
        private readonly List<string> _featuresToRemove = new List<string>();

        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var removeFeatures = _featuresToRemove.Count > 0 ? string.Join(",", _featuresToRemove) : "$null";
            var addFeatures = string.Join(",", _featuresToAdd);
            return remote.Execute.PowerShell(string.Format("Set-ConDepWindowsFeatures {0} {1}", addFeatures, removeFeatures)).Result;
        }

        public override string Name
        {
            get { return "Windows Feature Installer"; }
        }

        public void AddWindowsFeature(string roleService)
        {
            _featuresToAdd.Add(roleService);            
        }

        public void RemoveWindowsFeature(string roleService)
        {
            _featuresToRemove.Add(roleService);
        }
    }
}