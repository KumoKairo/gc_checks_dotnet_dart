import 'package:gc_checks/benchmarking_stuff.dart';

void main(List<String> arguments) {
  const int iterations = 10;
  for (int i = 0; i < iterations; i++) {
    print(RefsBenchmark.main());
  }
}
