using Blitz.Reliability.Demo.Libs;
using Blitz.Reliability.Demo.Models;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Blitz.Reliability.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineArgs = args;

            Console.WriteLine($"{Program.ProgramMetadata}");

            Parser.Default.ParseArguments<CommandOptions>(args)
                   .WithParsed<CommandOptions>(o =>
                   {
                       var arguments = CommandLine.Parser.Default.FormatCommandLine<CommandOptions>(o);
                       Console.WriteLine($"{Program.ProgramMetadata.Product} {arguments}");

                       var tester = Program.Services.GetService<Workers.IWorker>();
                       tester.Run(o);
                   })
                   .WithNotParsed(errors =>
                   {
                       foreach (var e in errors)
                       {
                           Console.WriteLine($"{e.Tag}");
                       }
                       Environment.ExitCode = -1;
                   });
        }

        #region "DI"

        private static string[] CommandLineArgs { get; set; }

        private static IServiceProvider _services;

        public static IServiceProvider Services
        {
            get
            {
                if (_services == null)
                {
                    // Create service collection
                    var serviceCollection = new ServiceCollection();

                    // Build DI Stack inc. Logging, Configuration, and Application
                    ConfigureServices(serviceCollection);

                    // Create service provider
                    _services = serviceCollection.BuildServiceProvider();
                }
                return _services;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var configurationBuilder = new ConfigurationBuilder();

            string filename = "rabbitmqconfig.json";
            if (File.Exists(filename)) configurationBuilder.AddJsonFile(filename);

            if ((CommandLineArgs != null) && (CommandLineArgs.Length > 0)) configurationBuilder.AddCommandLine(CommandLineArgs);

            var config = configurationBuilder.Build();
            services.AddSingleton(config);

            // Logging
            services.AddLogging(loggingBuilder => {
                // This line must be 1st
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);

                // Console is generically cloud friendly
                loggingBuilder.AddConsole();
            });

            // App to run
            services.AddTransient<Workers.IWorker, Workers.Worker>();
        }

        #endregion

        #region "Assembly Version"

        private static Models.AssemblyVersionMetadata _assemblyversionmetadata;

        /// <summary>
        /// Semantic Version, etc from Assembly Metadata
        /// </summary>
        public static Models.AssemblyVersionMetadata ProgramMetadata
        {
            get
            {
                if (_assemblyversionmetadata == null)
                {
                    _assemblyversionmetadata = new Models.AssemblyVersionMetadata();
                    var assembly = typeof(Program).Assembly;
                    foreach (var attribute in assembly.GetCustomAttributesData())
                    {
                        if (!attribute.TryParse(out string value))
                        {
                            value = string.Empty;
                        }
                        var name = attribute.AttributeType.Name;
                        System.Diagnostics.Trace.WriteLine($"{name}, {value}");
                        _assemblyversionmetadata.PropertySet(name, value);
                    }
                }
                return _assemblyversionmetadata;
            }
        }

        #endregion


    }
}
