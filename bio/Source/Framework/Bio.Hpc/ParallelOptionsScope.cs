using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Bio.Hpc.Properties;

namespace Bio.Hpc
{
    /// <summary>
    /// Sealed class holds parallel options scope
    /// </summary>
    public sealed class ParallelOptionsScope : IDisposable
    {
        /// <summary>
        /// A ParallelOptions instance in which MaxDegreeOfParallelism is 1.
        /// </summary>
        public readonly static ParallelOptions SingleThreadedOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };
        /// <summary>
        /// A ParallelOptions instance in which MaxDegreeOfParallelism is equal to the number of cores in the current environment.
        /// </summary>
        public readonly static ParallelOptions FullyParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

#if !SILVERLIGHT
        const string KEY_NAME = "ParallelOptionsScope";
        //private static ParallelOptions _default = new ParallelOptions() { MaxDegreeOfParallelism = -1 };
        private bool _disposed;

        /// <summary>
        /// Creates a new ParallelOptionsScope using the specified thread count and defaults for all other options. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope Create(int threadCount)
        {
            return Create(new ParallelOptions { MaxDegreeOfParallelism = threadCount });
        }

        /// <summary>
        /// Creates a new ParallelOptionsScope using the specified options. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope Create(ParallelOptions options)
        {
            return new ParallelOptionsScope(options);
        }

        /// <summary>
        /// Creates a new ParallelOptionsScope that specified that a single thread should be used. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope CreateSingleThreaded()
        {
            return new ParallelOptionsScope(SingleThreadedOptions);
        }

        /// <summary>
        /// Creates a new ParallelOptionsScope that sets the number of threads to be equal to the number of cores in the current environment. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope CreateFullyParallel()
        {
            return new ParallelOptionsScope(FullyParallelOptions);
        }

        /// <summary>
        /// Suspend task
        /// </summary>
        /// <returns></returns>
        public static IDisposable Suspend()
        {
            return new SuspendScope();
        }

        /// <summary>
        /// Gets either data exists or not
        /// </summary>
        public static bool Exists
        {
            get
            {
                var stack = CallContext.LogicalGetData(KEY_NAME) as StackNode;
                return (stack != null);
            }
        }

        /// <summary>
        /// Gets current parallel options
        /// </summary>
        public static ParallelOptions Current
        {
            get
            {
                var stack = CallContext.LogicalGetData(KEY_NAME) as StackNode;
                if (stack == null)
                {
                    throw new InvalidOperationException(Resource.called_inside);
                }
                return stack.Options;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                var stack = CallContext.LogicalGetData(KEY_NAME) as StackNode;
                if (stack != null) CallContext.LogicalSetData(KEY_NAME, stack.Next);
            }
        }

        private ParallelOptionsScope(ParallelOptions options)
        {
            if (options == null) throw new ArgumentNullException("options");
            var stack = CallContext.LogicalGetData(KEY_NAME) as StackNode;
            stack = new StackNode { Options = options, Next = stack };
            CallContext.LogicalSetData(KEY_NAME, stack);
        }

        /// <summary>
        /// Private class holds stack node
        /// </summary>
        private class StackNode
        {
            /// <summary>
            /// Parallel options
            /// </summary>
            public ParallelOptions Options;
            /// <summary>
            /// Next node
            /// </summary>
            public StackNode Next;
        }

        /// <summary>
        /// Suspend scope
        /// </summary>
        private class SuspendScope : IDisposable
        {
            private bool _disposed = false;
            private StackNode _suspendedStackNode;

            /// <summary>
            /// Constructor
            /// </summary>
            public SuspendScope()
            {
                _suspendedStackNode = SuspendParallelOptionsScope();
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    RestoreParallelOptionsScope(_suspendedStackNode);
                }
            }

            private static StackNode SuspendParallelOptionsScope()
            {
                var stack = CallContext.LogicalGetData(KEY_NAME) as StackNode;
                if (stack != null)
                {
                    CallContext.FreeNamedDataSlot(KEY_NAME);
                    if (CallContext.LogicalGetData(KEY_NAME) != null)
                    {
                        throw new NotImplementedException(Resource.Not_working);
                    }
                }
                return stack;
            }

            private static void RestoreParallelOptionsScope(StackNode stack)
            {
                if (CallContext.LogicalGetData(KEY_NAME) != null)
                {
                    throw new InvalidOperationException(Resource.Cannot_replace);
                }
                CallContext.LogicalSetData(KEY_NAME, stack);
            }
        }
#else
        public static IDisposable Create(ParallelOptions options)
        {
            return new ParallelOptionsScope();
        }

        /// <summary>
        /// Creates a new ParallelOptionsScope that specified that a single thread should be used. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope CreateSingleThreaded()
        {
            return new ParallelOptionsScope();
        }

        /// <summary>
        /// Creates a new ParallelOptionsScope that sets the number of threads to be equal to the number of cores in the current environment. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope CreateFullyParallel()
        {
            return new ParallelOptionsScope();
        }

        public static IDisposable Suspend()
        {
            return new ParallelOptionsScope();
        }
        public void Dispose()
        {
        }

                public static ParallelOptions Current
        {
            get
            {
        return null;
        }
        }


#endif

    }

    /// <summary>
    /// Parallel query extension 
    /// </summary>
    public static class ParallelQueryExtensions
    {
#if !SILVERLIGHT
        /// <summary>
        /// With parallel options scope
        /// </summary>
        /// <typeparam name="TSource">parallel query source</typeparam>
        /// <param name="source">source</param>
        /// <returns></returns>
        public static ParallelQuery<TSource> WithParallelOptionsScope<TSource>(this ParallelQuery<TSource> source)
        {
            return source.WithDegreeOfParallelism(ParallelOptionsScope.Current.MaxDegreeOfParallelism);
        }
#else
        public static IEnumerable<TSource> WithParallelOptionsScope<TSource>(this IEnumerable<TSource> source)
        {
            return source;
        }
#endif
    }

}

