using System.Threading.Tasks;

namespace ServerSide.Interfaces
{
    public interface IRequire
    { 
        Task SendALl(string message);
    }
}
