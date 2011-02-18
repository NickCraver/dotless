namespace dotless.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Core;
    using Core.configuration;
    using Core.Parameters;

    public class Program
    {
        public static int Main(string[] args)
        {
            var arguments = new List<string>();

            arguments.AddRange(args);

            var configuration = GetConfigurationFromArguments(arguments);

            if(configuration.Help)
                return 0;

            if (arguments.Count == 0)
            {
                WriteHelp();
                return 0;
            }

            var returnValue = 0;

            var inputDirectoryPath = Path.GetDirectoryName(arguments[0]);
            if(string.IsNullOrEmpty(inputDirectoryPath)) inputDirectoryPath = ".\\";
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

            var filenames = Directory.GetFiles(inputDirectoryPath, inputFilePattern, configuration.Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly); 
            var engine = new EngineFactory(configuration).GetEngine();

            using (var watcher = new Watcher() { Watch = configuration.Watch })
            {
                if (watcher.Watch && HasWildcards(inputFilePattern))
                {
                    CompilationFactoryDelegate factoryDelegate = (input) => CreationImpl(engine, input, Path.GetFullPath(outputDirectoryPath));
                    watcher.SetupDirectoryWatcher(Path.GetFullPath(inputDirectoryPath), inputFilePattern, factoryDelegate);
                }

                foreach (var filename in filenames)
                {
                    var inputFile = new FileInfo(filename);
                    var pathbuilder = configuration.Recurse
                                          ? new System.Text.StringBuilder(Path.GetDirectoryName(filename) + "\\")
                                          : new System.Text.StringBuilder(outputDirectoryPath + "\\");
                    if (string.IsNullOrEmpty(outputFilename)) pathbuilder.Append(Path.ChangeExtension(inputFile.Name, "css"));
                    else pathbuilder.Append(outputFilename);
                    var outputFilePath = Path.GetFullPath(pathbuilder.ToString());

                    CompilationDelegate compilationDelegate = () => CompileImpl(engine, inputFile.FullName, outputFilePath);
                    Console.WriteLine("[Compile]");
                    var files = compilationDelegate();
                    if (files == null)
                        returnValue = 1;
                    if (watcher.Watch) watcher.SetupWatchers(files, compilationDelegate);
                }
                if (configuration.Watch) WriteAbortInstructions();
                while (watcher.Watch && Console.ReadKey(true).Key != ConsoleKey.Enter) 
                {
                    System.Threading.Thread.Sleep(200);
                }
            }
            return returnValue;
        }
        private static CompilationDelegate CreationImpl(ILessEngine engine, string inputFilePath, string outputDirectoryPath)
        {
            var pathbuilder = new System.Text.StringBuilder(outputDirectoryPath + Path.DirectorySeparatorChar);
            pathbuilder.Append(Path.ChangeExtension(Path.GetFileName(inputFilePath), "css"));
            var outputFilePath = Path.GetFullPath(pathbuilder.ToString());
            return () => CompileImpl(engine, inputFilePath, outputFilePath);
        }

        private static IEnumerable<string> CompileImpl(ILessEngine engine, string inputFilePath, string outputFilePath)
        {
            engine = new FixImportPathDecorator(engine);
            var currentDir = Directory.GetCurrentDirectory();
            try
            {
                Console.WriteLine("{0} -> {1}", inputFilePath, outputFilePath);
                var directoryPath = Path.GetDirectoryName(inputFilePath);
                var source = new dotless.Core.Input.FileReader().GetFileContents(inputFilePath);
                Directory.SetCurrentDirectory(directoryPath);
                var css = engine.TransformToCss(source, inputFilePath);
                File.WriteAllText(outputFilePath, css);
                Console.WriteLine("[Done]");

                var files = new List<string>();
                files.Add(inputFilePath);
                foreach (var file in engine.GetImports())
                    files.Add(Path.Combine(directoryPath, Path.ChangeExtension(file, "less")));
                engine.ResetImports();
                return files;
            }
            catch (IOException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FAILED]");
                Console.WriteLine("Compilation failed: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
                return null;
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDir);
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
            Console.WriteLine("\t\t-w --watch - Watches .less file for changes");
            Console.WriteLine("\t\t-h --help - Displays this dialog");
            Console.WriteLine("\t\t-r --recurse - Apply input file pattern to sub directories");
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
                    else if (arg == "-w" || arg == "--watch")
                    {
                        configuration.Watch = true;
                    }
                    else if (arg == "-r" || arg == "--rescurse")
                    {
                        configuration.Recurse = true;
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