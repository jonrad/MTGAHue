using System;
using System.Threading.Tasks;

namespace MTGADispatcher
{
    public interface ILineReader : IDisposable
    {
        Task<string?> ReadLine();
    }
}
