using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ConDep.Dsl.Config;
using ConDep.Dsl.Operations.Application.Execution.PowerShell;
using ConDep.Dsl.Remote;
using ConDep.Dsl.Validation;

namespace ConDep.Dsl.Operations.Remote.Execution.PowerShell
{
    public class PowerShellOperation : ForEachServerOperation
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

        public PowerShellOperation(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("cmd") && !parameters.ContainsKey("file"))
            {
                _commandType = CommandType.ScriptFile;
            }
            else if (!parameters.ContainsKey("cmd") && parameters.ContainsKey("file"))
            {
                _commandType = CommandType.CmdLine;
            }
            else
            {
                throw new ConDepMissingOptionsException(new [] {"cmd or file"});
            }

            _cmd = parameters.ContainsKey("cmd") ? parameters["cmd"] : null;
            _scriptFile = parameters.ContainsKey("file") ? new FileInfo(parameters["file"]) : null;

            _values = new PowerShellOptions.PowerShellOptionValues();

            if(parameters.ContainsKey("ContinueOnError")) _values.ContinueOnError = Convert.ToBoolean(parameters["ContinueOnError"]);
            if(parameters.ContainsKey("RequireRemoteLib")) _values.RequireRemoteLib = Convert.ToBoolean(parameters["RequireRemoteLib"]);
            if(parameters.ContainsKey("UseCredSSP")) _values.ContinueOnError = Convert.ToBoolean(parameters["UseCredSSP"]);
        }

        public PowerShellOperation(string cmd, PowerShellOptions.PowerShellOptionValues values = null)
        {
            _cmd = cmd;
            _values = values;
            _commandType = CommandType.CmdLine;
        }

        public PowerShellOperation(FileInfo scriptFile, PowerShellOptions.PowerShellOptionValues values = null)
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
            var psExec = new PowerShellExecutor();
            if (_values != null)
            {
                if (_values.UseCredSSP) psExec.UseCredSSP = true;
            }
            psExec.Execute(server, cmd, mod =>
            {
                mod.LoadConDepDotNetLibrary = _values == null || _values.RequireRemoteLib;
            });
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