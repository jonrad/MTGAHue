using System.Threading.Tasks;

namespace MTGADispatcher
{
    public interface IMagicService
    {
        void Start();

        Task Stop();
    }
}
