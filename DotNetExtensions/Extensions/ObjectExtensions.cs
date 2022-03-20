using System.Threading.Tasks;

namespace DotNetMore.Extensions
{
    public static class ObjectExtensions
    {
        public static Task<T> AsTask<T>(this T obj)
        {
            return Task.FromResult(obj);
        }
    }
}

