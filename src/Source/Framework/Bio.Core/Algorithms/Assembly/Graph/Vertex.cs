using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Bio.Algorithms.Assembly.Graph
{
    /// <summary>
    /// Vertex class.
    /// Stores incoming and outgoing edges to improve the performance.
    /// </summary>
    /// <typeparam name="T">Type of data to store in vertex.</typeparam>
    [DebuggerDisplay("Id={Id}, IncomingEdgeCount={IncomingEdgeCount}, OutgoingEdgeCount={OutgoingEdgeCount}")]
    public class Vertex<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes an instance of Vertex class with specified id.
        /// </summary>
        /// <param name="id">Id for new Vertex.</param>
        public Vertex(long id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Initializes an instance of Vertex class with specified id and data.
        /// </summary>
        /// <param name="id">Id for new Vertex.</param>
        /// <param name="data">Data to store in new Vertex.</param>
        public Vertex(long id, T data)
        {
            this.Data = data;
            this.Id = id;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the id of the Vertex.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets or sets the data of the Vertex.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Holds Incoming edges.
        /// </summary>
        private List<long> IncomingEdges { get; set; }

        /// <summary>
        /// Holds outgoing edges.
        /// </summary>
        private List<long> OutgoingEdges { get; set; }

        /// <summary>
        /// Gets incoming edge count.
        /// </summary>
        public int IncomingEdgeCount
        {
            get
            {
                int count = 0;
                if (this.IncomingEdges != null)
                {
                    count = this.IncomingEdges.Count;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets outgoing edge count.
        /// </summary>
        public int OutgoingEdgeCount
        {
            get
            {
                int count = 0;
                if (this.OutgoingEdges != null)
                {
                    count = this.OutgoingEdges.Count;
                }

                return count;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds specified edge id to incoming edges list.
        /// </summary>
        /// <param name="edgeID">Edge id to add.</param>
        public void AddIncomingEdge(long edgeID)
        {
            if (this.IncomingEdges == null)
            {
                this.IncomingEdges = new List<long>();
            }

            this.IncomingEdges.Add(edgeID);
        }

        /// <summary>
        /// Adds specified edge id to outgoing edge list.
        /// </summary>
        /// <param name="edgeId">Edge id to add.</param>
        public void AddOutgoingEdge(long edgeId)
        {
            if (this.OutgoingEdges == null)
            {
                this.OutgoingEdges = new List<long>();
            }

            this.OutgoingEdges.Add(edgeId);
        }

        /// <summary>
        /// Searches for the specified edge id in incoming edge list and if found 
        /// removes it from the incoming edge list.
        /// </summary>
        /// <param name="edgeID">Edge id to remove.</param>
        /// <returns>Returns true if edge id found and successfully removed from the incoming list, else returns false.</returns>
        public bool RemoveFromIncomingEdge(long edgeID)
        {
            bool result = false;
            if (this.IncomingEdges != null)
            {
                result = this.IncomingEdges.Remove(edgeID);
            }

            return result;
        }

        /// <summary>
        /// Searches for the specified edge id in outgoing edge list and if found 
        /// removes it from the outgoing edge list.
        /// </summary>
        /// <param name="edgeId">Edge id to remove.</param>
        /// <returns>Returns true if edge id found and successfully removed from the outgoing list, else returns false.</returns>
        public bool RemoveFromOutgoingEdge(long edgeId)
        {
            bool result = false;
            if (this.OutgoingEdges != null)
            {
                result = this.OutgoingEdges.Remove(edgeId);
            }

            return result;
        }

        /// <summary>
        /// Searches for the oldEdgeId in incoming edge list of the Vertex, if found replaces it with the newEdgeId.
        /// </summary>
        /// <param name="oldEdgeId">Old edge id to search.</param>
        /// <param name="newEdgeId">New edge id to replace with old edge id.</param>
        /// <returns>Returns true if the oldEdgeId found and replaced with newEdgeid, else returns false.</returns>
        public bool ReplaceIncomingEdge(long oldEdgeId, long newEdgeId)
        {
            bool result = false;
            if (this.IncomingEdges != null)
            {
                int index = this.IncomingEdges.IndexOf(oldEdgeId);
                if (index >= 0)
                {
                    this.IncomingEdges[index] = newEdgeId;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches for the oldEdgeId in outgoing edge list of the Vertex, if found replaces it with the newEdgeId.
        /// </summary>
        /// <param name="oldEdgeId">Old edge id to search.</param>
        /// <param name="newEdgeId">New edge id to replace with old edge id.</param>
        /// <returns>Returns true if the oldEdgeId found and replaced with newEdgeid, else returns false.</returns>
        public bool ReplaceOutgoingEdge(long oldEdgeId, long newEdgeId)
        {
            bool result = false;
            if (this.OutgoingEdges != null)
            {
                int index = this.OutgoingEdges.IndexOf(oldEdgeId);
                if (index >= 0)
                {
                    this.OutgoingEdges[index] = newEdgeId;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Replaces the edge id at specified index of incoming edge list of the Vertex with the specified edgeid.
        /// </summary>
        /// <param name="index">Zero baced index of incoming edge list.</param>
        /// <param name="edgeId">Edge id to replace with.</param>
        /// <returns>Returns true if the index is valid and replaced with edgeId, else returns false.</returns>
        public bool ReplaceIncomingEdge(int index, long edgeId)
        {
            if (this.IncomingEdges == null || index >= this.IncomingEdges.Count)
            {
                return false;
            }

            this.IncomingEdges[index] = edgeId;
            return true;
        }

        /// <summary>
        /// Replaces the edge id at specified index of outgoing edge list of the Vertex with the specified edgeid.
        /// </summary>
        /// <param name="index">Zero baced index of outgoing edge list.</param>
        /// <param name="edgeId">Edge id to replace with.</param>
        /// <returns>Returns true if the index is valid and replaced with edgeId, else returns false.</returns>
        public bool ReplaceOutgoingEdge(int index, long edgeId)
        {
            if (this.OutgoingEdges == null || index >= this.OutgoingEdges.Count)
            {
                return false;
            }

            this.OutgoingEdges[index] = edgeId;

            return true;
        }

        /// <summary>
        /// Removes all edges from incoming and outgoing edge list of the Vertex.
        /// </summary>
        public void ClearAllEdges()
        {
            this.ClearIncomingEdges();
            this.ClearOutgoingEdges();
        }

        /// <summary>
        /// Removes all edges from incoming edge list of the Vertex.
        /// </summary>
        public void ClearIncomingEdges()
        {
            if (this.IncomingEdges != null)
            {
                this.IncomingEdges.Clear();
                this.IncomingEdges = null;
            }
        }

        /// <summary>
        /// Removes all edges from outgoing edge list of the Vertex.
        /// </summary>
        public void ClearOutgoingEdges()
        {
            if (this.OutgoingEdges != null)
            {
                this.OutgoingEdges.Clear();
                this.OutgoingEdges = null;
            }
        }
      
        /// <summary>
        /// Gets the incoming edge id present in specified index of incoming edge list in the Vertex.
        /// </summary>
        /// <param name="index">Index of the incoming edge list.</param>
        /// <returns>Returns Edgeid if the index is valid, else returns -1.</returns>
        public long GetIncomingEdge(int index)
        {
            if (this.IncomingEdges == null || index >= this.IncomingEdges.Count)
            {
                return -1;
            }

            return this.IncomingEdges[index];
        }

        /// <summary>
        /// Gets the outgoing edge id present in specified index of outgoing edge list in the Vertex.
        /// </summary>
        /// <param name="index">Index of the outgoing edge list.</param>
        /// <returns>Returns Edgeid if the index is valid, else returns -1.</returns>
        public long GetOutgoingEdge(int index)
        {
            if (this.OutgoingEdges == null || index >= this.OutgoingEdges.Count)
            {
                return -1;
            }

            return this.OutgoingEdges[index];
        }
        #endregion
    }
}
