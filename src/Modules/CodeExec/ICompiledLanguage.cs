namespace Assistant.Modules.CodeExec
{
    public interface ICompiledLanguage : ILanguage
    {
        public string CompileCommand { get; }
    }
}
