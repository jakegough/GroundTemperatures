using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundTemperatures
{
    public static class LinqExtensions
    {
        public static double StdDev<T>(this IEnumerable<T> values, Func<T, double> selector)
            => values.Select(selector).StdDev();

        public static double StdDev(this IEnumerable<double> values)
        {
            int count = values.Count();
            if (count == 0)
            {
                throw new ArgumentException("at least one record is required!");
            }

            //Compute the Average
            double avg = values.Average();

            //Perform the Sum of (value-avg)^2
            double sum = values.Sum(d => (d - avg) * (d - avg));

            //Put it all together
            return Math.Sqrt(sum / count);
        }
    }
}
