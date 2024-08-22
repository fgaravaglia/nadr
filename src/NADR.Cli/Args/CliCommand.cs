using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace NADR.Cli.Args
{
    /// <summary>
    /// base class for command managed by Cli
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class CliCommand
    {
        /// <summary>
        /// full path of Code repository, to store the ADR
        /// </summary>
        public string Repository { get; set; }
        /// <summary>
        /// 
        /// </summary>
        protected CliCommand()
        {
            this.Repository = "";
        }
    }
}