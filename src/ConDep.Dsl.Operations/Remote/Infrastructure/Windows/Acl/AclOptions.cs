using System.Security.AccessControl;

namespace ConDep.Dsl.Operations.Remote.Infrastructure.Windows.Acl
{
    internal class AclOptions : IOfferAclOptions
    {
        private readonly AclOptionsValues _values = new AclOptionsValues();

        public AclOptions()
        {
            _values.Type = AccessControlType.Allow;
            _values.Inheritance = System.Security.AccessControl.InheritanceFlags.ContainerInherit | System.Security.AccessControl.InheritanceFlags.ObjectInherit;
            _values.Propagation = System.Security.AccessControl.PropagationFlags.None;
        }

        public AclOptionsValues Values
        {
            get { return _values; }
        }

        public IOfferAclOptions InheritanceFlags(InheritanceFlags flags)
        {
            _values.Inheritance = flags;
            return this;
        }

        public IOfferAclOptions PropagationFlags(PropagationFlags flags)
        {
            _values.Propagation = flags;
            return this;
        }

        public IOfferAclOptions Type(AccessControlType type)
        {
            _values.Type = type;
            return this;
        }

        public class AclOptionsValues
        {
            public InheritanceFlags Inheritance { get; set; }
            public PropagationFlags Propagation { get; set; }
            public AccessControlType Type { get; set; }
        }
    }
}