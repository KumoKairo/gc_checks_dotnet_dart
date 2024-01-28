using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        System.Environment.SetEnvironmentVariable("COMPlus_JITMinOpts", "1");
        const int iterations = 10;
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            Console.WriteLine(RefsBenchmark.main());
        }

        sw.Stop();
        Console.WriteLine($"Total: {sw.Elapsed.TotalSeconds} GC: {GC.GetTotalPauseDuration().TotalSeconds}");
    }
}
