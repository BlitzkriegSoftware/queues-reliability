using CommandLine;

namespace Blitz.Reliability.Demo.Models
{
    /// <summary>
    /// Command Line Switches
    /// </summary>
    public class CommandOptions
    {
        /// <summary>
        /// Verbose Output
        /// </summary>
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        /// <summary>
        /// Default: Transaction Counts
        /// </summary>
        public const int UnitOfWorkDefault = 50;

        /// <summary>
        /// Unit of Work Count
        /// </summary>
        [Option('c', "count", Required = false, HelpText = "How many Units of Work to generate", Default = UnitOfWorkDefault)]
        public int UnitOfWorkCount { get; set; } = UnitOfWorkDefault;

    }
}
