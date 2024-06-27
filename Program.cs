using Xunit;
using System.Threading;


var X = (double x) => x;
var SIN = (double x) => Math.Sin(x);

Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2), 1e-4);
Assert.Equal(0, DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8), 1e-4);
Assert.Equal(50, DefiniteIntegral.Solve(0, 10, X, 1e-6, 8), 1e-5);


class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        int steps = (int)((b - a) / step);

        double result = 0;
        double[] results = new double[threadsnumber];
        Thread[] threads = new Thread[threadsnumber];

        double width = (b - a) / threadsnumber;

        for (int i = 0; i < threadsnumber; i++)
        {
            int index = i;
            Func<double, double> func = function;
            double lower = a + i * width;
            double upper = a + (i + 1) * width;
            int segments = (int)((upper - lower) / step);
            threads[i] = new Thread(() => {
                results[index] = SolveIntegralTrapezoid(func, lower, upper, segments);
            });
            threads[i].Start();
        }
        for (int i = 0; i < threadsnumber; i++)
        {
            threads[i].Join();
            result += results[i];
        }
        return result;
    }

    public static double SolveIntegralTrapezoid(Func<double, double> function, double lowerBound, double upperBound, int numSegments)
    {
        double interval = (upperBound - lowerBound) / numSegments;
        double result = 0.5 * (function(lowerBound) + function(upperBound));

        for (int i = 1; i < numSegments; i++)
        {
            double x = lowerBound + i * interval;
            result += function(x);
        }

        return result * interval;
    }
}
