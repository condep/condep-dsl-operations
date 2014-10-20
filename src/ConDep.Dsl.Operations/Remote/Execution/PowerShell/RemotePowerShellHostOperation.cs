using System.IO;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Application.Execution.PowerShell;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Execution.PowerShell
{
    public class RemotePowerShellHostOperation : ForEachServerOperation
    {
        private enum CommandType
        {
            CmdLine,
            ScriptFile
        }

        private readonly string _cmd;
        private readonly FileInfo _scriptFile;
        private readonly PowerShellOptions.PowerShellOptionValues _values;
        private readonly CommandType _commandType;

        public RemotePowerShellHostOperation(string cmd, PowerShellOptions.PowerShellOptionValues values = null)
        {
            _cmd = cmd;
            _values = values;
            _commandType = CommandType.CmdLine;
        }

        public RemotePowerShellHostOperation(FileInfo scriptFile, PowerShellOptions.PowerShellOptionValues values = null)
        {
            _scriptFile = scriptFile;
            _values = values;
            _commandType = CommandType.ScriptFile;
        }

        public override void Execute(ServerConfig server, IReportStatus status, ConDepSettings settings, CancellationToken token)
        {
            switch (_commandType)
            {
                case CommandType.CmdLine:
                    ExecuteCommand(_cmd, server);
                    break;
                case CommandType.ScriptFile:
                    ExecuteScriptFile(_scriptFile, server);
                    break;
                default:
                    throw new ConDepInvalidEnumValueException(_commandType);
            }
        }

        private void ExecuteScriptFile(FileInfo scriptFile, ServerConfig server)
        {
            if(!scriptFile.Exists) throw new FileNotFoundException(scriptFile.FullName);

            string script;
            using (var fileStream = File.OpenRead(scriptFile.FullName))
            {
                using (var memStream = GetMemoryStreamWithCorrectEncoding(fileStream))
                {
                    using (var reader = new StreamReader(memStream))
                    {
                        script = reader.ReadToEnd();
                    }
                }
            }

            ExecuteCommand(script, server);
        }

        private static MemoryStream GetMemoryStreamWithCorrectEncoding(Stream stream)
        {
            using (var r = new StreamReader(stream, true))
            {
                var encoding = r.CurrentEncoding;
                return new MemoryStream(encoding.GetBytes(r.ReadToEnd()));
            }
        }

        private void ExecuteCommand(string cmd, ServerConfig server)
        {
            var psExec = new PowerShellExecutor(server);
            if (_values != null)
            {
                if (_values.RequireRemoteLib) psExec.LoadConDepDotNetLibrary = true;
                if (_values.UseCredSSP) psExec.UseCredSSP = true;
            }
            psExec.Execute(cmd);
        }

        public override string Name
        {
            get { return "Remote PowerShell"; }
        }

        public override bool IsValid(Notification notification)
        {
            return true;
        }
    }
}