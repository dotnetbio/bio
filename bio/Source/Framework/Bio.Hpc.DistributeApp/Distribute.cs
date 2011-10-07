using System;
using System.Linq;
using Bio.Util.ArgumentParser;
using Bio.Util;
using Bio.Util.Distribute;

namespace Bio.Hpc.DistributeApp
{
    /// <summary>
    /// class Distribute
    /// </summary>
    public class Distribute : IRunnable
    {
        /// <summary>
        /// The object you wish to distribute.
        /// </summary>
        [Parse(ParseAction.Required)]
        public IDistributable Distributable { get; set;}

        /// <summary>
        /// How to distribute the object.
        /// </summary>
        [Parse(ParseAction.Required)]
        public IDistribute Distributor { get; set;}
        
        /// <summary>
        /// Run method
        /// </summary>
        public void Run()
        {
            Distributor.Distribute(Distributable);
        }

        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args">arguments</param>
        static void Main(string[] args)
        {
            Console.Error.WriteLine(SplashString());
            try
            {
                Console.WriteLine(args.Select(s => "\"" + s + "\"").StringJoin(" "));
                CommandArguments.ConstructAndRun<Distribute>(args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                Environment.ExitCode = -532462766; // general failure.
            }
        }

        private static string SplashString()
        {
            const string SplashString = "\n.NET Bio Hpc Distribute Application v1.0"
                                      + "\n  Copyright (c) 2011, The Outercurve Foundation.";
            return (SplashString);
        }
    }
}
