using System;
using Bio.DistributeApp.Properties;
using Bio.Util.ArgumentParser;
using Bio.Util.Distribute;

namespace Bio.DistributeApp
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
            Console.Error.WriteLine(Resources.DistributeApp_SplashScreen);

            try
            {
                CommandArguments.ConstructAndRun<Distribute>(args, false);
            }
            catch (HelpException)
            {
                Console.Error.WriteLine(Properties.Resources.DistributeApp_Help);
                Environment.ExitCode = 10022; 
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                Environment.ExitCode = -532462766; // general failure.
            }
        }
    }
}
