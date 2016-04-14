using System;

namespace ConDep.Dsl.Operations.Remote.Installation.Executable
{
    public class ConDepInstallationFailureException : Exception
    {
        public ConDepInstallationFailureException(string message) : base(message)
        {
            
        }
    }
}