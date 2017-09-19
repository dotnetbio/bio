using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bio.Util
{
    /// <summary>
    ///     ParallelOptionsScope
    /// </summary>
    public sealed class ParallelOptionsScope : IDisposable
    {
        /// <summary>
        /// Async local storage for the current scope.
        /// </summary>
        private static readonly AsyncLocal<StackNode> Scope = new AsyncLocal<StackNode>();

        /// <summary>
        ///     A ParallelOptions instance in which MaxDegreeOfParallelism is 1.
        /// </summary>
        public static readonly ParallelOptions SingleThreadedOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };

        /// <summary>
        ///     A ParallelOptions instance in which MaxDegreeOfParallelism is equal to the number of cores in the current
        ///     environment.
        /// </summary>
        public static readonly ParallelOptions FullyParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        /// <summary>
        /// Flag indicating we've been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">New set of options</param>
        private ParallelOptionsScope(ParallelOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var stack = Scope.Value;
            stack = new StackNode { Options = options, Next = stack };
            Scope.Value = stack;
        }

        /// <summary>
        ///     Exists
        /// </summary>
        public static bool Exists
        {
            get
            {
                var stack = Scope.Value;
                return (stack != null);
            }
        }

        /// <summary>
        ///     Current
        /// </summary>
        public static ParallelOptions Current
        {
            get
            {
                var stack = Scope.Value;
                if (stack == null)
                {
                    throw new InvalidOperationException(
                        "This method must be called inside a using(ParallelOptionsScope) statement.");
                }
                return stack.Options;
            }
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposed = true;
                var stack = Scope.Value;
                if (stack != null)
                {
                    Scope.Value = stack.Next;
                }
            }
        }

        /// <summary>
        ///     Creates a new ParallelOptionsScope using the specified thread count and defaults for all other options. Must be
        ///     used in a using statement.
        /// </summary>
        public static ParallelOptionsScope Create(int threadCount)
        {
            return Create(new ParallelOptions { MaxDegreeOfParallelism = threadCount });
        }

        /// <summary>
        ///     Creates a new ParallelOptionsScope using the specified options. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope Create(ParallelOptions options)
        {
            return new ParallelOptionsScope(options);
        }

        /// <summary>
        ///     Creates a new ParallelOptionsScope that specified that a single thread should be used. Must be used in a using
        ///     statement.
        /// </summary>
        public static ParallelOptionsScope CreateSingleThreaded()
        {
            return new ParallelOptionsScope(SingleThreadedOptions);
        }

        /// <summary>
        ///     Creates a new ParallelOptionsScope that sets the number of threads to be equal to the number of cores in the
        ///     current environment. Must be used in a using statement.
        /// </summary>
        public static ParallelOptionsScope CreateFullyParallel()
        {
            return new ParallelOptionsScope(FullyParallelOptions);
        }

        /// <summary>
        ///     Suspend
        /// </summary>
        /// <returns></returns>
        public static IDisposable Suspend()
        {
            return new SuspendScope();
        }

        private class StackNode
        {
            public StackNode Next;

            public ParallelOptions Options;
        }

        private class SuspendScope : IDisposable
        {
            private readonly StackNode _suspendedStackNode;

            private bool _disposed;

            public SuspendScope()
            {
                this._suspendedStackNode = SuspendParallelOptionsScope();
            }

            public void Dispose()
            {
                if (!this._disposed)
                {
                    this._disposed = true;
                    RestoreParallelOptionsScope(this._suspendedStackNode);
                }
            }

            private static StackNode SuspendParallelOptionsScope()
            {
                var stack = Scope.Value;
                if (stack != null)
                {
                    Scope.Value = null;
                }
                return stack;
            }

            private static void RestoreParallelOptionsScope(StackNode stack)
            {
                if (Scope.Value != null)
                {
                    throw new InvalidOperationException(
                        "Cannot replace an existing ParallelOptionsScope stack. Remove the existing stack first with SuspendParallelOptionsScope()");
                }
                Scope.Value = stack;
            }
        }
    }

    /// <summary>
    ///     ParallelQueryExtensions
    /// </summary>
    public static class ParallelQueryExtensions
    {
        /// <summary>
        ///     ParallelQuery
        /// </summary>
        /// <typeparam name="TSource">TSource</typeparam>
        /// <param name="source">source</param>
        /// <returns>ParallelQuery</returns>
        public static ParallelQuery<TSource> WithParallelOptionsScope<TSource>(this ParallelQuery<TSource> source)
        {
            return source.WithDegreeOfParallelism(ParallelOptionsScope.Current.MaxDegreeOfParallelism);
        }
    }
}