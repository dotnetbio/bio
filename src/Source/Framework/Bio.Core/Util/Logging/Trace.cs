using System;
using System.Collections.Generic;

namespace Bio.Util.Logging
{
    /// <summary>
    /// The Trace class implements a mechanism for logging messages, both to a Log object,
    /// and to a simple message queue that can be used for GUI display or other purposes.
    /// </summary>
    public static class Trace
    {
        /// <summary>
        /// Flag to report non-fatal sequence parsing/formatting errors.
        /// </summary>
        public const ulong SeqWarnings = 0x1;

        /// <summary>
        /// Flag to report details of sequence assembly into the log.
        /// </summary>
        public const ulong AssemblyDetails = 0x2;

        /// <summary>
        /// Default maxiumum messages
        /// </summary>
        public const int DefaultMaxMessages = 20;

        /// <summary>
        /// flags variable
        /// </summary>
        private static ulong flags = 0;

        /// <summary>
        /// maximum messages
        /// </summary>
        private static int maxMessages = DefaultMaxMessages;

        /// <summary>
        /// list of trace messages
        /// </summary>
        private static readonly List<TraceMessage> Messages = new List<TraceMessage>();       

        /// <summary>
        /// Test to see if a flag is in the set of flags currently turned on.
        /// </summary>
        /// <param name="traceSettings">a flag, encoded as a single bit in a ulong.</param>
        /// <returns>true if the flag is set.</returns>
        public static bool Want(ulong traceSettings)
        {
            return (traceSettings & flags) != 0;
        }

        /// <summary>
        /// Report a TraceMessage, by adding it to the front of the message
        /// queue, as well as logging it.
        /// </summary>
        /// <param name="m">the Trace message</param>
        public static void Report(TraceMessage m)
        {
            if (m != null)
            {
                Messages.Insert(0, m);
                ApplicationLog.WriteLine(m.Format());
                TrimToSize();
            }
        }

        /// <summary>
        /// Overload that constructs the TraceMessage from its parts.
        /// </summary>
        /// <param name="context">Where the incident occurred.</param>
        /// <param name="message">The details of what happened.</param>
        /// <param name="data">Pertinent data such as argument values.</param>
        public static void Report(string context, string message, string data)
        {
            DateTime when = DateTime.Now;
            TraceMessage m = new TraceMessage(context, message, data, when);
            Report(m);
        }

        /// <summary>
        /// Overload to report from a plain string.
        /// </summary>
        /// <param name="message">the message.</param>
        public static void Report(string message)
        {
            Report(string.Empty, message, string.Empty);
        }

        /// <summary>
        /// Return the newest message in the queue (or null, if none).
        /// </summary>
        /// <returns>a TraceMessage.</returns>
        public static TraceMessage LatestMessage()
        {
            return GetMessage(0);
        }

        /// <summary>
        /// return the ith message in the queue (0 = newest).
        /// </summary>
        /// <param name="i">the index.</param>
        /// <returns>the TraceMessage.</returns>
        public static TraceMessage GetMessage(int i)
        {
            if (i < 0 || i >= Messages.Count)
            {
                return null;
            }

            return Messages[i];
        }

        /// <summary>
        /// Turn on a flag, expressed as a set bit in a ulong.
        /// </summary>
        /// <param name="traceSettings">The bit to set.</param>
        public static void Set(ulong traceSettings)
        {
            flags = flags | traceSettings;
        }

        /// <summary>
        /// Clear a flag, expressed as a set bit in a ulong.
        /// </summary>
        /// <param name="traceSettings">The bit to clear.</param>
        public static void Clear(ulong traceSettings)
        {
            flags = flags & ~traceSettings;
        }

        /// <summary>
        /// TrimToSize method
        /// </summary>
        private static void TrimToSize()
        {
            int ct = Messages.Count;
            if (ct > maxMessages)
            {
                Messages.RemoveRange(maxMessages, ct - maxMessages);
            }
        }
    }
}
