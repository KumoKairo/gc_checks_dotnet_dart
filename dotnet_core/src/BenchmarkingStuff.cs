using System.Diagnostics;

class RefsBenchmark : BenchmarkBase
{
    public RefsBenchmark() : base("Refs") { }

    public static string main()
    {
        return new RefsBenchmark().report();
    }

    public override void run()
    {
        var link = testMethod();
        if (link.next != null)
        {
            // pass
        }
    }

    private RefLink testMethod()
    {
        var root = new RefLink(null);
        var link = root;
        for (var i = 0; i < 100000 - 1; i++)
        {
            var newLink = new RefLink(null);
            link.next = newLink;
            link = newLink;
        }
        return root;
    }
}

class VectorsBenchmark : BenchmarkBase
{
    public VectorsBenchmark() : base("Vectors")
    {
    }

    public static string main()
    {
        return new VectorsBenchmark().report();
    }

    public override void run()
    {
        GC.Collect();
        var vec = testMethod();
    }

    private Vector4 testMethod()
    {
        var vec = new Vector4(0.0, 0.0, 0.0, 0.0);
        for (var i = 0; i < 100000 - 1; i++)
        {
            vec = new Vector4(0.0, 0.0, 0.0, 0.0);
        }
        return vec;
    }
}

class PrintEmitter
{
    public string emit(string testName, double value)
    {
        return $"{testName}(RunTime): {value} us.";
    }
}

class BenchmarkBase
{
    const int minimumMeasureDurationMillis = 2000;

    string name;
    PrintEmitter emitter;

    public BenchmarkBase(string name)
    {
        this.name = name;
        emitter = new PrintEmitter();
    }

    /// The benchmark code.
    ///
    /// This function is not used, if both [warmup] and [exercise] are
    /// overwritten.
    public virtual void run() { }

    /// Not measured setup code executed prior to the benchmark runs.
    public virtual void setup() { }

    /// Not measured teardown code executed after the benchmark runs.
    public virtual void teardown() { }

    /// Measures the score for this benchmark by executing it enough times
    /// to reach [minimumMillis].
    private Measurement _measure(int minimumMillis)
    {
        var minimumMicros = minimumMillis * 1000;
        // If running a long measurement permit some amount of measurement jitter
        // to avoid discarding results that are almost good, but not quite there.
        var allowedJitter = Math.Floor(
            minimumMillis < 1000 ? 0 : (minimumMicros * 0.1));
        var iter = 5;
        var watch = new Stopwatch();
        while (true)
        {
            watch.Reset();
            watch.Start();

            for (var i = 0; i < iter; i++)
            {
                run();
            }
            watch.Stop();
            var elapsed = watch.Elapsed.TotalMicroseconds;
            var measurement = new Measurement((int)elapsed, iter);
            if (measurement.elapsedMicros >= (minimumMicros - allowedJitter))
            {
                return measurement;
            }

            iter = measurement.estimateIterationsNeededToReach(
                minimumMicros: minimumMicros);
        }
    }

    /// Measures the score for the benchmark and returns it.
    double measure()
    {
        setup();
        // Warmup for at least 100ms. Discard result.
        _measure(100);
        // Run the benchmark for at least 2000ms.
        var result = _measure(minimumMeasureDurationMillis);
        teardown();
        return result.score;
    }

    public string report()
    {
        return emitter.emit(name, measure());
    }
}

class Measurement
{
    public int elapsedMicros;
    public int iterations;

    public Measurement(int elapsedMicros, int iterations)
    {
        this.elapsedMicros = elapsedMicros;
        this.iterations = iterations;
    }

    public double score => elapsedMicros / iterations;

    public int estimateIterationsNeededToReach(int minimumMicros)
    {
        var elapsed = roundDownToMillisecond(elapsedMicros);
        var result = elapsed == 0
            ? iterations * 1000
            : Math.Ceiling(iterations * Math.Max(minimumMicros / elapsed, 1.5));

        return (int)result;
    }

    static int roundDownToMillisecond(int micros) => micros / 1000 * 1000;

    override public string ToString() => $"{elapsedMicros} in {iterations} iterations";
}