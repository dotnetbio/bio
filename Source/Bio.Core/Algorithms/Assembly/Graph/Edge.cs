using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Edge class.
    /// In case of Directed graph, VertexId1 is the tail and VertexId2 is the head of the edge.
    /// </summary>
    /// <typeparam name="T">Data type.</typeparam>
    [DebuggerDisplay("Id: {Id}, VertexId1: {VertexId1}, VertexId2: {VertexId2}")]
    public class Edge<T>
    {
        /// <summary>
        /// Initializes a new instance of the Edge class.
        /// </summary>
        /// <param name="id"></param>
        public Edge(long id)
        {
            this.Id = id;
            this.VertexId1 = -1;
            this.VertexId2 = -1;
        }

        /// <summary>
        /// Gets Id of the Edge.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// First vertex id.
        /// </summary>
        public long VertexId1 { get; set; }

        /// <summary>
        /// Second vertex id.
        /// </summary>
        public long VertexId2 { get; set; }
    }
}
