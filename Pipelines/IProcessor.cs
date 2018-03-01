using System.Threading.Tasks;

namespace Pipelines
{
    public interface IProcessor
    {
        Task Execute(object arguments);
    }
}