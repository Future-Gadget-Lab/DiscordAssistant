using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Modules.CodeExec
{
    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; }
        public string StandardError { get; set; }
    }

    public class DockerContainer : IDisposable
    {
        private readonly string _id;
        public DockerContainer(string id)
        {
            _id = id;
        }

        public static async Task<DockerContainer> CreateContainerAsync(string image)
        {
            var result = await RunDockerCommandAsync($"run --network=none -t -d {image}", false);
            if (result.ExitCode != 0)
                throw new Exception($"An error occurred creating a container (exit code ${result.ExitCode}): {result.StandardError}");
            if (string.IsNullOrEmpty(result.StandardOutput))
                throw new Exception("Unable to retrieve container ID");
            return new DockerContainer(result.StandardOutput);
        }

        public Task<ProcessResult> ExecAsync(string command) =>
               RunDockerCommandAsync($"exec {_id} {command}");

        public async Task<string> ExecOutAsync(string command)
        {
            var result = await ExecAsync(command);
            if (result.ExitCode != 0)
                throw new Exception($"An error occurred executing command '{command}': {result.StandardError}");
            return result.StandardOutput;
        }

        public async Task CopyAsync(string src, string dest)
        {
            var result = await RunDockerCommandAsync($"cp {src} {_id}:{dest}");
            if (result.ExitCode != 0)
                throw new Exception($"An error occurred while copying a file into a container (exit code ${result.ExitCode}): {result.StandardError}");
        }

        private static Task<ProcessResult> RunDockerCommandAsync(string command, bool appendNewline = true)
        {
            TaskCompletionSource<ProcessResult> taskSource = new TaskCompletionSource<ProcessResult>();

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = command,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                },
                EnableRaisingEvents = true
            };

            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            process.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (e.Data == null)
                    return;

                if (appendNewline)
                    output.AppendLine(e.Data);
                else
                    output.Append(e.Data);

            };
            process.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if (e.Data == null)
                    return;

                if (appendNewline)
                    error.AppendLine(e.Data);
                else
                    error.Append(e.Data);
            };
            process.Exited += (object sender, EventArgs e) =>
            {
                taskSource.SetResult(new ProcessResult
                {
                    ExitCode = process.ExitCode,
                    StandardOutput = output.ToString(),
                    StandardError = error.ToString(),
                });
                process.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return taskSource.Task;
        }

        public async void Dispose()
        {
            await RunDockerCommandAsync($"kill {_id}");
            await RunDockerCommandAsync($"rm {_id}");
        }
    }
}
