using System;
using System.IO;
using System.Linq;
using dotless.Core;

namespace dotlessjs.Compiler
{
    using System.Collections.Generic;

    public class Program
    {
        public static void Main(string[] args)
        {
            var files = Directory.GetFiles(".", "*.less");

            var contents = files
                .ToDictionary(f => f, f => File.ReadAllText(f));

            const int rounds = 150;

            Func<string, ILessEngine, int> runTest =
                (file, engine) => Enumerable
                                      .Range(0, rounds)
                                      .Select(i =>
                                                  {
                                                      var starttime = DateTime.Now;
                                                      engine.TransformToCss(contents[file], file);
                                                      var duration = (DateTime.Now - starttime);
                                                      return duration.Milliseconds;
                                                  })
                                      .Sum();

            Console.WriteLine("Press Enter to begin benchmark");
            Console.ReadLine();

            Console.WriteLine("Running each test {0} times\n", rounds);

            var engines = new ILessEngine[]
                              {
                                  new LessEngine("Input")
                              };


            Console.Write("       File        |     Size     |");
            foreach (var engine in engines)
            {
                Console.Write(engine.GetType().Name.PadRight(24).PadLeft(29));
                Console.Write('|');
            }
            Console.WriteLine();
            Console.WriteLine(new string('-', 35 + 30*engines.Length));

            foreach (var file in files)
            {
                var size = rounds*contents[file].Length/1024d;
                Console.Write("{0} | {1,8:#,##0.00} Kb  | ", file.PadRight(18), size);

                var times = new List<double>();
                foreach (var engine in engines)
                {
                    try
                    {
                        var time = runTest(file, engine)/1000d;
                        times.Add(time);
                        Console.Write("{0,8:#.00} s  {1,10:#,##0.00} Kb/s | ", time, size/time);
                    }
                    catch
                    {
                        Console.Write("Failied                     | ");
                    }
                }
                if (times.Count == 2)
                    Console.Write("{0,4:0.#}x", times[0] / times[1]);
                Console.WriteLine();
            }

            //      Console.Read();
        }
    }
}