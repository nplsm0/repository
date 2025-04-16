using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationUtility
{
    internal static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            foreach(var item in items)
            {
                stack.Push(item);
            }
        }
    }
}
