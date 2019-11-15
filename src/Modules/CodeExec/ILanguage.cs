using System.Collections.Generic;

namespace Assistant.Modules.CodeExec
{
    public interface ILanguage
    {
        public string Name { get; }
        public string[] Aliases { get; }
        public string Extension { get; }
        public string DockerImage { get; }
        public string RunCommand { get; }
        public string SourceFile { get; }
    }
}
