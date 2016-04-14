using System;
using System.IO;
using ConDep.Dsl.Builders;
using ConDep.Dsl.Operations.Application.Execution.PowerShell;
using ConDep.Dsl.Operations.Application.Execution.RunCmd;
using ConDep.Dsl.Operations.Remote.Execution.PowerShell;

namespace ConDep.Dsl
{
    public static class RemoteExecutionExtensions
    {
        /// <summary>
        /// Will execute a DOS command using cmd.exe on remote server.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static IOfferRemoteExecution DosCommand(this IOfferRemoteExecution execute, string cmd)
        {
            var runCmdOperation = new RunCmdPsOperation(cmd);
            OperationExecutor.Execute((RemoteBuilder)execute, runCmdOperation);
            return execute;
        }

        /// <summary>
        /// Will execute a DOS command using cmd.exe on remote server with provided options.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="runCmdOptions"></param>
        /// <returns></returns>
        public static IOfferRemoteExecution DosCommand(this IOfferRemoteExecution execute, string cmd, Action<IOfferRunCmdOptions> runCmdOptions)
        {
            var options = new RunCmdOptions();
            runCmdOptions(options);
            var runCmdOperation = new RunCmdPsOperation(cmd, options.Values);
            OperationExecutor.Execute((RemoteBuilder)execute, runCmdOperation);
            return execute;
        }

        /// <summary>
        /// Will execute a PowerShell command on remote server.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static IOfferRemoteExecution PowerShell(this IOfferRemoteExecution execute, string command)
        {
            var psProvider = new PowerShellOperation(command);
            OperationExecutor.Execute((RemoteBuilder)execute, psProvider);
            return execute;
        }

        /// <summary>
        /// Will execute a PowerShell command on remote server with provided options.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="powerShellOptions"></param>
        /// <returns></returns>
        public static IOfferRemoteExecution PowerShell(this IOfferRemoteExecution execute, FileInfo scriptFile)
        {
            var psProvider = new PowerShellOperation(scriptFile);
            OperationExecutor.Execute((RemoteBuilder)execute, psProvider);
            return execute;
        }

        /// <summary>
        /// Will deploy and execute provided PowerShell script on remote server.
        /// </summary>
        /// <param name="scriptFile"></param>
        /// <returns></returns>
        public static IOfferRemoteExecution PowerShell(this IOfferRemoteExecution execute, string command, Action<IOfferPowerShellOptions> powerShellOptions)
        {
            var options = new PowerShellOptions();
            powerShellOptions(options);
            var operation = new PowerShellOperation(command, options.Values);
            OperationExecutor.Execute((RemoteBuilder)execute, operation);
            return execute;
        }

        /// <summary>
        /// Will deploy and execute provided PowerShell script on remote server with provided options.
        /// </summary>
        /// <param name="scriptFile"></param>
        /// <param name="powerShellOptions"></param>
        /// <returns></returns>
        public static IOfferRemoteExecution PowerShell(this IOfferRemoteExecution execute, FileInfo scriptFile, Action<IOfferPowerShellOptions> powerShellOptions)
        {
            var options = new PowerShellOptions();
            powerShellOptions(options);
            var operation = new PowerShellOperation(scriptFile, options.Values);
            OperationExecutor.Execute((RemoteBuilder)execute, operation);
            return execute;
        }
    
    }
}