using System.Security.AccessControl;

namespace ConDep.Dsl
{
    public interface IOfferAclOptions
    {
        IOfferAclOptions InheritanceFlags(InheritanceFlags flags);
        IOfferAclOptions PropagationFlags(PropagationFlags flags);
        IOfferAclOptions Type(AccessControlType type);
    }
}