using System.Threading.Tasks;

namespace Assistant.Services
{
    interface IInitializable
    {
        public Task Initialize();
    }
}
