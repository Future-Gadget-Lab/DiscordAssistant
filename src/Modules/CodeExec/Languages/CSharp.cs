namespace Assistant.Modules.CodeExec.Languages
{
    public class CSharp : ILanguage
    {
        public string Name { get; } = "C#";
        public string[] Aliases { get; } = { "cs", "csharp" };
        public string Extension { get; } = "cs";
        public string DockerImage { get; } = "xoltia/csharp";
        public string RunCommand { get; } = "dotnet run";
        public string SourceFile { get; } =
            "using System;" +
            "using System.Diagnostics;" +
            "class Program" +
            "{{" +
            "   static void Main(string[] args)" +
            "   {{" +
            "        {0}" +
            "   }}" +
            "}}";
    }
}
