namespace dotless.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Core;
    using Core.configuration;
    using Core.Parameters;
    using System.Text;
    using System.Diagnostics;

    public class Program
    {
        public static int Main(string[] args)
        {
            var arguments = new List<string>();

            arguments.AddRange(args);

            var configuration = GetConfigurationFromArguments(arguments);

            if (configuration.Help)
                return 0;

            if (arguments.Count == 0)
            {
                WriteHelp();
                return 0;
            }

            Stopwatch timer = null;

            if (configuration.TimeCompilation)
            {
                timer = new Stopwatch();
                timer.Start();
            }

            var returnValue = 0;

            var inputDirectoryPath = Path.GetDirectoryName(arguments[0]);
            if (string.IsNullOrEmpty(inputDirectoryPath)) inputDirectoryPath = ".\\";
            var inputFilePattern = Path.GetFileName(arguments[0]);
            var outputDirectoryPath = string.Empty;
            var outputFilename = string.Empty;

            if (string.IsNullOrEmpty(inputFilePattern)) inputFilePattern = "*.less";
            if (!Path.HasExtension(inputFilePattern)) inputFilePattern = Path.ChangeExtension(inputFilePattern, "less");

            if (arguments.Count > 1)
            {
                outputDirectoryPath = Path.GetDirectoryName(arguments[1]);
                outputFilename = Path.GetFileName(arguments[1]);
                outputFilename = Path.ChangeExtension(outputFilename, "css");
            }
            else outputDirectoryPath = inputDirectoryPath;
            if (HasWildcards(inputFilePattern)) outputFilename = string.Empty;

            var factory = new EngineFactory(configuration);

            var filenames = Directory.GetFiles(inputDirectoryPath, inputFilePattern, configuration.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var parallelFiles = filenames.AsParallel();
            parallelFiles =
                parallelFiles.WithDegreeOfParallelism(
                    configuration.CompileInParallel ?
                        Environment.ProcessorCount :
                        1
                ).WithExecutionMode(ParallelExecutionMode.ForceParallelism);

            var logLock = new object();
            Action<string> logDel =
                delegate(string log)
                {
                    if (string.IsNullOrEmpty(log)) return;

                    lock (logLock)
                    {
                        Console.Write(log);
                    }
                };

            parallelFiles.ForAll(
                delegate(string filename)
                {
                    var log = new StringBuilder();

                    var inputFile = new FileInfo(filename);

                    var engine = factory.GetEngine(Path.GetDirectoryName(inputFile.FullName));

                    var pathbuilder = configuration.Recurse
                                          ? new System.Text.StringBuilder(Path.GetDirectoryName(filename) + "\\")
                                          : new System.Text.StringBuilder(outputDirectoryPath + "\\");
                    if (string.IsNullOrEmpty(outputFilename)) pathbuilder.Append(Path.ChangeExtension(inputFile.Name, "css"));
                    else pathbuilder.Append(outputFilename);
                    var outputFilePath = Path.GetFullPath(pathbuilder.ToString());

                    CompilationDelegate compilationDelegate = () => CompileImpl(engine, inputFile.FullName, outputFilePath, log, configuration.SilenceLogging);

                    if (!configuration.SilenceLogging)
                    {
                        log.AppendLine("[Compile]");
                    }

                    var files = compilationDelegate();

                    if (files == null)
                    {
                        returnValue = 1;
                    }

                    logDel(log.Length == 0 ? null : log.ToString());
                }
            );

            if (configuration.TimeCompilation)
            {
                timer.Stop();
                Console.WriteLine("Compilation took: {0}ms", timer.ElapsedMilliseconds);
            }

            return returnValue;
        }

        private static IEnumerable<string> CompileImpl(ILessEngine engine, string inputFilePath, string outputFilePath, StringBuilder sb, bool silent)
        {
            try
            {
                if (!silent)
                {
                    sb.AppendFormat("{0} -> {1}\n", inputFilePath, outputFilePath);
                }

                var directoryPath = Path.GetDirectoryName(inputFilePath);
                var source = new dotless.Core.Input.FileReader(directoryPath).GetFileContents(inputFilePath);
                var css = engine.TransformToCss(source, inputFilePath);
                File.WriteAllText(outputFilePath, css);

                if (!silent)
                {
                    sb.AppendLine("[Done]");
                }

                var files = new List<string>();
                files.Add(inputFilePath);
                foreach (var file in engine.GetImports())
                {
                    files.Add(Path.Combine(directoryPath, Path.ChangeExtension(file, "less")));
                }
                engine.ResetImports();
                return files;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (silent)
                {
                    // Need to know the file now
                    sb.AppendFormat("{0} -> {1}\n", inputFilePath, outputFilePath);
                }

                sb.AppendLine("[FAILED]");
                sb.AppendFormat("Compilation failed: {0}\n", ex.Message);
                sb.AppendLine(ex.StackTrace);
                return null;
            }
        }


        private static bool HasWildcards(string inputFilePattern)
        {
            return System.Text.RegularExpressions.Regex.Match(inputFilePattern, @"[\*\?]").Success;
        }

        private static void WriteAbortInstructions()
        {
            Console.WriteLine("Hit Enter to stop watching");
        }

        private static string GetAssemblyVersion()
        {
            Assembly assembly = typeof(EngineFactory).Assembly;
            var attributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true) as
                             AssemblyFileVersionAttribute[];
            if (attributes != null && attributes.Length == 1)
                return attributes[0].Version;
            return "v.Unknown";
        }

        private static void WriteHelp()
        {
            Console.WriteLine("dotless Compiler {0}", GetAssemblyVersion());
            Console.WriteLine("\tCompiles .less files to css files.");
            Console.WriteLine();
            Console.WriteLine("Usage: dotless.Compiler.exe [-switches] <inputfile> [outputfile]");
            Console.WriteLine("\tSwitches:");
            Console.WriteLine("\t\t-m --minify - Output CSS will be compressed");
            Console.WriteLine("\t\t-h --help - Displays this dialog");
            Console.WriteLine("\t\t-r --recurse - Apply input file pattern to sub directories");
            Console.WriteLine("\t\t-p --parallel - Compile using one thread per core");
            Console.WriteLine("\t\t-t --time - Time how long compilation takes");
            Console.WriteLine("\t\t-s --silent - Only log errors");
            Console.WriteLine("\tinputfile: .less file dotless should compile to CSS");
            Console.WriteLine("\toutputfile: (optional) desired filename for .css output");
            Console.WriteLine("\t\t Defaults to inputfile.css");
        }

        private static CompilerConfiguration GetConfigurationFromArguments(List<string> arguments)
        {
            var configuration = new CompilerConfiguration(DotlessConfiguration.Default);

            foreach (var arg in arguments)
            {
                if (arg.StartsWith("-"))
                {
                    if (arg == "-m" || arg == "--minify")
                    {
                        configuration.MinifyOutput = true;
                    }
                    else if (arg == "-h" || arg == "--help")
                    {
                        WriteHelp();
                        configuration.Help = true;
                        return configuration;
                    }
                    else if (arg == "-r" || arg == "--rescurse")
                    {
                        configuration.Recurse = true;
                    }
                    else if (arg == "-p" || arg == "--parallel")
                    {
                        configuration.CompileInParallel = true;
                    }
                    else if (arg == "-t" || arg == "--time")
                    {
                        configuration.TimeCompilation = true;
                    }
                    else if (arg == "-s" || arg == "--silent")
                    {
                        configuration.SilenceLogging = true;
                    }
                    else if (arg.StartsWith("-D") && arg.Contains("="))
                    {
                        var split = arg.Substring(2).Split('=');
                        var key = split[0];
                        var value = split[1];
                        ConsoleArgumentParameterSource.ConsoleArguments.Add(key, value);
                    }
                    else
                    {
                        Console.WriteLine("Unknown command switch {0}.", arg);
                    }
                }
            }
            arguments.RemoveAll(p => p.StartsWith("-"));
            return configuration;
        }
    }
}