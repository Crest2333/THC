using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TWpf.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> list, bool condition, Func<T, bool> predicate)
        {
            if (condition)
            {
                return list.Where(predicate);
            }
            return list;
        }
    }
}
