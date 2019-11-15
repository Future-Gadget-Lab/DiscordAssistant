namespace Assistant.Modules.CodeExec.Languages
{
    public class Java : ICompiledLanguage
    {
        public string Name { get; } = "Java";
        public string[] Aliases { get; } = { };
        public string Extension { get; } = "java";
        public string DockerImage { get; } = "xoltia/java";
        public string CompileCommand { get; } = "javac {0}";
        public string RunCommand { get; } =  "java {1}";
        public string SourceFile { get; } =
            "class {1}" +
            "{{" +
            "   public static void main(String[] args)" +
            "   {{" +
            "        {0}" +
            "   }}" +
            "}}";
    }
}
