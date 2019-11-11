using System.Threading.Tasks;

namespace Assistant
{
    class Program
    {
        private static Task Main(string[] args) =>
            new Assistant(AssistantConfig.FromFile("config.json")).StartAndBlock();
    }
}
