using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;
using ConDep.Dsl.Operations;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl
{
    public class RestartComputerOperation : RemoteOperation
    {
        public enum WaitForStatus
        {
            Success,
            Failure
        }
        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            var canPingServer = CanPingServer(server);
            Logger.Verbose(string.Format("Can {0}use ping for validation", canPingServer ? "" : "NOT "));

            var powershellExecutor = new PowerShellExecutor(server);
            powershellExecutor.Execute("Restart-Computer -Force");

            if (canPingServer)
            {
                Logger.Verbose("Waiting for ping to fail");
                WaitForPing(WaitForStatus.Failure, server);
                Logger.Verbose("Waiting for ping to Succeed");
                WaitForPing(WaitForStatus.Success, server);
            }
            else
            {
                Logger.Verbose("Waiting for WinRM to fail");
                WaitForWinRm(WaitForStatus.Failure, server);
            }
            Logger.Verbose("Waiting for WinRM to succeed");
            WaitForWinRm(WaitForStatus.Success, server);
            Logger.Info("Computer restarted");
            Logger.WithLogSection("Starting ConDepNode", () => StartConDepNode(server));
        }

        private void StartConDepNode(ServerConfig server)
        {
            ConDepNodePublisher.StartConDepNode(server);
        }

        private void WaitForWinRm(WaitForStatus status, ServerConfig server)
        {
            var cmd = server.DeploymentUser.IsDefined() ? string.Format("id -r:{0} -u:{1} -p:\"{2}\"", server.Name,
                server.DeploymentUser.UserName, server.DeploymentUser.Password) : string.Format("id -r:{0}", server.Name);

            var path = Environment.ExpandEnvironmentVariables(@"%windir%\system32\WinRM.cmd");
            var startInfo = new ProcessStartInfo(path)
            {
                Arguments = cmd,
                Verb = "RunAs",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var process = Process.Start(startInfo);
            process.WaitForExit();

            switch (status)
            {
                case WaitForStatus.Failure:
                    if (process.ExitCode == 0)
                    {
                        Thread.Sleep(5000);
                        WaitForWinRm(status, server);
                    }
                    break;
                case WaitForStatus.Success:
                    if (process.ExitCode != 0)
                    {
                        Thread.Sleep(5000);
                        WaitForWinRm(status, server);
                    }
                    break;
            }
        }

        private void WaitForPing(WaitForStatus failure, ServerConfig server)
        {
            var pingTool = new Ping();
            var reply = pingTool.Send(server.Name);

            switch (failure)
            {
                case WaitForStatus.Failure:
                    if (reply.Status == IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                        WaitForPing(failure, server);
                    }
                    break;
                case WaitForStatus.Success:
                    if (reply.Status != IPStatus.Success)
                    {
                        Thread.Sleep(1000);
                        WaitForPing(failure, server);
                    }
                    break;
                default:
                    throw new Exception("Status not supported.");
            }
        }

        private bool CanPingServer(ServerConfig server)
        {
            bool result = false;
            var pingTool = new Ping();
            try
            {
                result = pingTool.Send(server.Name).Status == IPStatus.Success;
            }
            catch{}
            return result;
        }

        public override string Name
        {
            get { return "Restart Computer"; }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }

    public static class RestartComputerExtensions
    {
        public static IOfferRemoteOperations Restart(this IOfferRemoteOperations remote)
        {
            var restartOperation = new RestartComputerOperation();
            Configure.Remote(remote, restartOperation);
            return remote;
        }
    }
}