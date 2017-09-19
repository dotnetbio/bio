using System.Collections.Generic;
using System.Linq;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Graph data structure.
    /// </summary>
    /// <typeparam name="T">Type of data to store in Vertex.</typeparam>
    /// <typeparam name="U">Type of data to store in edge.</typeparam>
    public class Graph<T, U>
    {
        /// <summary>
        /// Holds Vertices present in the graph.
        /// </summary>
        protected BigList<Vertex<T>> VertexArray { get; set; }

        /// <summary>
        /// Holds edges present in the graph.
        /// </summary>
        protected BigList<Edge<U>> EdgeArray { get; set; }

        /// <summary>
        /// Deleted vertices count.
        /// </summary>
        protected long DeletedVertexCount { get; set; }

        /// <summary>
        /// Deleted edges count.
        /// </summary>
        protected long DeletedEdgeCount { get; set; }

        /// <summary>
        /// # of vertices present in the graph excluding deleted vertices.
        /// </summary>
        public long VertexCount
        {
            get
            {
                return VertexArray.Count - DeletedVertexCount;
            }
        }

        /// <summary>
        /// No of edges present in the graph excluding deleted edges.
        /// </summary>
        public long EdgeCount
        {
            get
            {
                return EdgeArray.Count - DeletedEdgeCount;
            }
        }

        /// <summary>
        /// Total no of vertices in the graph Including deleted vertices.
        /// </summary>
        public long TotalVertexCount
        {
            get
            {
                return VertexArray.Count;
            }
        }

        /// <summary>
        /// Total no of edges in the graph Including deleted edges.
        /// </summary>
        public long TotalEdgeCount
        {
            get
            {
                return EdgeArray.Count;
            }
        }

        /// <summary>
        /// Constructor to create an instance of graph.
        /// </summary>
        public Graph()
        {
            VertexArray = new BigList<Vertex<T>>();
            EdgeArray = new BigList<Edge<U>>();
            DeletedVertexCount = 0;
            DeletedEdgeCount = 0;
        }

        /// <summary>
        /// Gets existing nodes.
        /// </summary>
        public IEnumerable<Vertex<T>> GetVertices()
        {
            return VertexArray.Where(node => node != null);
        }

        /// <summary>
        /// Gets existing nodes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Edge<U>> GetEdges()
        {
            return EdgeArray.Where(edge => edge != null);
        }

        /// <summary>
        /// Deletes the node.
        /// Note this will not delete associated edges.
        /// </summary>
        /// <param name="node">Vertex to be deleted.</param>
        public void DeleteVertex(Vertex<T> node)
        {
            if (node != null)
            {
                node.ClearAllEdges();
                DeletedVertexCount++;
                VertexArray[node.Id] = null;
            }
        }

        /// <summary>
        /// Deletes the node.
        /// Note this will not delete associated edges.
        /// </summary>
        /// <param name="vertexId">Id of the vertex to be deleted.</param>
        public void DeleteVertex(long vertexId)
        {
            this.DeleteVertex(VertexArray[vertexId]);
        }

        /// <summary>
        /// Adds a new vertex to the graph.
        /// </summary>
        /// <param name="vertexData">Data to store in new vertex.</param>
        /// <returns>New vertex.</returns>
        public Vertex<T> AddVertex(T vertexData)
        {
            long id = this.TotalVertexCount;

            Vertex<T> vertex = new Vertex<T>(id, vertexData);
            this.VertexArray.Add(vertex);

            return vertex;
        }

        /// <summary>
        /// Adds new edges to the graph which connects two vertices.
        /// Note: this method will not validate the present of vertex1 and vertex2 in the graph.
        /// </summary>
        /// <param name="edgeData">Data to store in new edge.</param>
        /// <param name="vertexId1">Id of the Vertex1.</param>
        /// <param name="vertexId2">Id of the Vertex2</param>
        /// <returns>New Edge.</returns>
        public Edge<U> AddEdge(U edgeData, long vertexId1, long vertexId2)
        {
            Edge<U> newedge = new Edge<U>(EdgeArray.Count)
            {
                Data = edgeData,
                VertexId1 = vertexId1,
                VertexId2 = vertexId2
            };
            EdgeArray.Add(newedge);

            return newedge;
        }

        /// <summary>
        /// Deletes the edge.
        /// </summary>
        /// <param name="edgeId"></param>
        public void DeleteEdge(long edgeId)
        {
            if (EdgeArray[edgeId] != null)
            {
                EdgeArray[edgeId] = null;
                DeletedEdgeCount++;
            }
        }

        /// <summary>
        /// Gets the vertices adjacent to given vertex.
        /// </summary>
        /// <param name="vertex">Vertex.</param>
        /// <returns>Enumerable of vertices adjacent to given vertex.</returns>
        public IEnumerable<Vertex<T>> GetAdjacentVertices(Vertex<T> vertex)
        {
            for (int i = 0; i < vertex.OutgoingEdgeCount; i++)
            {
                var edge = this.GetEdge(vertex.GetOutgoingEdge(i));
                yield return this.GetVertex(edge.VertexId2);
            }

            for (int i = 0; i < vertex.IncomingEdgeCount; i++)
            {
                var edge = this.GetEdge(vertex.GetIncomingEdge(i));
                yield return this.GetVertex(edge.VertexId1);
            }
        }

        /// <summary>
        /// Verifies if two vertices are adjacent to each other or not.
        /// </summary>
        /// <param name="vertex1">Vertex1</param>
        /// <param name="vertex2">Vertex2</param>
        /// <returns>Returns true if vertices are adjacent to each other else false.</returns>
        public bool IsAdjacent(Vertex<T> vertex1, Vertex<T> vertex2)
        {
            bool result = false;
            if (vertex1 != null && vertex2 != null)
            {
                for (int i = 0; i < vertex1.OutgoingEdgeCount; i++)
                {
                    Edge<U> edge = this.GetEdge(vertex1.GetOutgoingEdge(i));
                    if (edge.VertexId2 == vertex2.Id)
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// Gets the vertex for the specified vertexid.
        /// </summary>
        /// <param name="vertexId">Vertex id.</param>
        public Vertex<T> GetVertex(long vertexId)
        {
            return VertexArray[vertexId];
        }

        /// <summary>
        /// Gets the Edge for the specified edgeid.
        /// </summary>
        /// <param name="edgeId">Edge id.</param>
        public Edge<U> GetEdge(long edgeId)
        {
            return EdgeArray[edgeId];
        }

        /// <summary>
        /// Gets the edge containing vertexid1 and vertexid2.
        /// </summary>
        /// <param name="vertexId1">First vertex id</param>
        /// <param name="vertexId2">Second vertex id.</param>
        public Edge<U> GetEdge(long vertexId1, long vertexId2)
        {
            Vertex<T> vertex1 = VertexArray[vertexId1];
            for (int i = 0; i < vertex1.OutgoingEdgeCount; i++)
            {
                Edge<U> edge = this.GetEdge(vertex1.GetOutgoingEdge(i));
                if (edge.VertexId2 == vertexId2)
                {
                    return edge;
                }
            }
            return null;
        }
    }
}
