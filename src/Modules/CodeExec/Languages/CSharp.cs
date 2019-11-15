using System.Collections.Generic;

namespace Assistant.Modules.CodeExec.Languages
{
    public class CSharp : ILanguage
    {
        public string Name { get; } = "CSharp";
        public string[] Aliases { get; } = { "cs" };
        public string Extension { get; } = "cs";
        public string DockerImage { get; } = "mcr.microsoft.com/dotnet/core/sdk:3.0";
        public string RunCommand { get; } = "dotnet run -p {0}";
        public string SourceFile { get; } =
            "using System;" +
            "class Program" +
            "{{" +
            "   static void Main(string[] args)" +
            "   {{" +
            "        {0}" +
            "   }}" +
            "}}";
        public Dictionary<string, string> Environment { get; } = new Dictionary<string, string>
        { 
            { "Project.csproj",
              "<Project Sdk=\"Microsoft.NET.Sdk\">" +
                  "<PropertyGroup>" +
                      "<OutputType>Exe</OutputType>" +
                      "<TargetFramework>netcoreapp3.0</TargetFramework>" +
                  "</PropertyGroup>" +
              "</Project>"
            }
        };
    }
}
