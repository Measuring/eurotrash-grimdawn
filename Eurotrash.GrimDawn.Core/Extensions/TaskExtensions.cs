using System.Threading.Tasks;

namespace Eurotrash.GrimDawn.Core.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        ///     Convenience method to show intend that an async method does not need to be awaited and should continue on its own.
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task)
        {
        }
    }
}