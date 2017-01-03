using System.Threading.Tasks;

namespace TfsStandalone.UI.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        public static void WaitAndUnwrapException(this Task task)
        {
            task.GetAwaiter().GetResult();
        }

        public static TResult WaitAndUnwrapException<TResult>(this Task<TResult> task)
        {
            return task.GetAwaiter().GetResult();
        }
    }
}
