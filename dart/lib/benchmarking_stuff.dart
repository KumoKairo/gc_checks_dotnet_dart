import 'dart:math';

import 'package:gc_checks/poco.dart';

const int _minimumMeasureDurationMillis = 2000;

class RefsBenchmark extends BenchmarkBase {
  RefsBenchmark() : super('Refs');

  static String main() {
    return RefsBenchmark().report();
  }

  void run() {
    var link = testMethod();
    if (link.next != null) {
      // pass
    }
  }

  RefLink testMethod() {
    var root = RefLink(null);
    var link = root;
    for (var i = 0; i < 100000 - 1; i++) {
      var newLink = RefLink(null);
      link.next = newLink;
      link = newLink;
    }
    return root;
  }
}

class VectorBenchmark extends BenchmarkBase {
  VectorBenchmark() : super('Vectors');

  static String main() {
    return VectorBenchmark().report();
  }

  @override
  @pragma('vm:never-inline')
  @pragma('vm:no-interrupts')
  void run() {
    var vec = testMethod();
  }

  Vector4 testMethod() {
    var vec = Vector4(0.0, 0.0, 0.0, 0.0);
    for (var i = 0; i < 100000 - 1; i++) {
      vec = Vector4(0.0, 0.0, 0.0, 0.0);
    }
    return vec;
  }
}

class BenchmarkBase {
  final String name;
  final PrintEmitter emitter;

  BenchmarkBase(this.name, {this.emitter = const PrintEmitter()});

  /// The benchmark code.
  ///
  /// This function is not used, if both [warmup] and [exercise] are
  /// overwritten.
  void run() {}

  /// Not measured setup code executed prior to the benchmark runs.
  void setup() {}

  /// Not measured teardown code executed after the benchmark runs.
  void teardown() {}

  /// Measures the score for this benchmark by executing it enough times
  /// to reach [minimumMillis].
  _Measurement _measure(int minimumMillis) {
    var minimumMicros = minimumMillis * 1000;
    // If running a long measurement permit some amount of measurement jitter
    // to avoid discarding results that are almost good, but not quite there.
    var allowedJitter =
        (minimumMillis < 1000 ? 0 : (minimumMicros * 0.1)).floor();
    var iter = 2;
    var watch = Stopwatch()..start();
    while (true) {
      watch.reset();

      for (var i = 0; i < iter; i++) {
        run();
      }
      var elapsed = watch.elapsedMicroseconds;
      var measurement = _Measurement(elapsed, iter);
      if (measurement.elapsedMicros >= (minimumMicros - allowedJitter)) {
        return measurement;
      }

      iter = measurement.estimateIterationsNeededToReach(
          minimumMicros: minimumMicros);
    }
  }

  /// Measures the score for the benchmark and returns it.
  double measure() {
    setup();
    // Warmup for at least 100ms. Discard result.
    _measure(100);
    // Run the benchmark for at least 2000ms.
    var result = _measure(_minimumMeasureDurationMillis);
    teardown();
    return result.score;
  }

  String report() {
    return emitter.emit(name, measure());
  }
}

class _Measurement {
  final int elapsedMicros;
  final int iterations;

  _Measurement(this.elapsedMicros, this.iterations);

  double get score => elapsedMicros / iterations;

  int estimateIterationsNeededToReach({required int minimumMicros}) {
    final elapsed = roundDownToMillisecond(elapsedMicros);
    return elapsed == 0
        ? iterations * 1000
        : (iterations * max(minimumMicros / elapsed, 1.5)).ceil();
  }

  static int roundDownToMillisecond(int micros) => (micros ~/ 1000) * 1000;

  @override
  String toString() => '$elapsedMicros in $iterations iterations';
}

class PrintEmitter {
  const PrintEmitter();

  String emit(String testName, double value) {
    return '$testName(RunTime): $value us.';
  }
}
