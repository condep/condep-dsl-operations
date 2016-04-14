using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Logging;

namespace ConDep.Dsl.Operations.Remote
{
    public class RestartComputerOperation : RemoteOperation
    {
        private readonly int _delayInSeconds;

        public RestartComputerOperation(int delayInSeconds)
        {
            _delayInSeconds = delayInSeconds;
        }

        public enum WaitForStatus
        {
            Success,
            Failure
        }
        public override Result Execute(IOfferRemoteOperations remote, ServerConfig server, ConDepSettings settings, CancellationToken token)
        {
            var canPingServer = CanPingServer(server);

            Logger.Verbose($"Can {(canPingServer ? "" : "NOT ")}use ping for validation");

            Logger.WithLogSection("Restarting", () =>
            {
                Logger.Info($"Executing restart command on server {server.Name}");
                remote.Execute.PowerShell($"cmd /c \"shutdown /r /t {_delayInSeconds}\"");

                if (canPingServer)
                {
                    Logger.Verbose("Waiting for ping to fail");
                    Logger.Info("Waiting for server to stop responding");
                    WaitForPing(WaitForStatus.Failure, server);
                    Logger.Info("Server stopped responding");
                    Logger.Verbose("Waiting for ping to Succeed");
                    Logger.Info("Waiting for server to respond again");
                    WaitForPing(WaitForStatus.Success, server);
                    Logger.Info("Server started to respond");
                }
                else
                {
                    Logger.Verbose("Waiting for WinRM to fail");
                    Logger.Info("Waiting for server to stop responding");
                    WaitForWinRm(WaitForStatus.Failure, server);
                    Logger.Info("Server stopped responding");
                }
                Logger.Verbose("Waiting for WinRM to succeed");
                Logger.Info("Waiting for server to respond to PowerShell commands");
                WaitForWinRm(WaitForStatus.Success, server);
                Logger.Info("Serve successfully responds to PowerShell commands");
                Logger.Info("Computer successfully restarted");
                Logger.WithLogSection("Starting ConDepNode", () => remote.StartConDepNode());
            });

            return Result.SuccessChanged();
        }

        private void WaitForWinRm(WaitForStatus status, ServerConfig server)
        {
            try
            {
                var cmd = server.DeploymentUser.IsDefined()
                    ? $"id -r:{server.Name} -u:{server.DeploymentUser.UserName} -p:\"{server.DeploymentUser.Password}\""
                    : $"id -r:{server.Name}";

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
            catch
            {
                switch (status)
                {
                    case WaitForStatus.Failure:
                        return;
                    case WaitForStatus.Success:
                        Thread.Sleep(5000);
                        WaitForWinRm(status, server);
                        break;
                }
            }
        }

        private void WaitForPing(WaitForStatus failure, ServerConfig server)
        {
            try
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
            catch
            {
                switch (failure)
                {
                    case WaitForStatus.Failure:
                        return;
                    case WaitForStatus.Success:
                        Thread.Sleep(1000);
                        WaitForPing(failure, server);
                        break;
                    default:
                        throw new Exception("Status not supported.");
                }
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

        public override string Name => "Restart Computer";
    }
}