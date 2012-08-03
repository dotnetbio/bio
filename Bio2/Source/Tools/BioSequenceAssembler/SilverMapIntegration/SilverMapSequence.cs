#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QUT.Bio.SilverMap.ObjectModel;
using QUT.Bio.Util;

#endregion

namespace SequenceAssembler.SilverMapIntegration
{
	/// <summary>
	/// Application-specific linear domain from which features are obtained.  
	/// </summary>
	internal class SilverMapSequence : ILinearDomain
	{
		#region Private variables

		private string id;
		private string name;
		private readonly List<Xref> xref = new List<Xref>();
		private string label;
		private long length;
		
		#endregion

		#region Constructor

		/// <summary>
		/// Construct a sequence.
		/// </summary>
		/// <param name="id">The unique Id of the sequence.</param>
		/// <param name="name">The name of the sequence.</param>
		/// <param name="label">A shorter "name" suitable for use as an on-screen label.</param>
		/// <param name="length">The length of the sequence.</param>
		
		public SilverMapSequence ( string id, string name, string label, long length )
		{
			this.id = id;
			this.name = name;
			this.label = label;
			this.length = length;
		} 
		
		#endregion

		#region Public properties

		/// <summary>
		/// Gets an enumeration value representing the source of this sequence.
		/// </summary>
		
		public SequenceSource Source
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the Id of this sequence.
		/// </summary>
		
		public string Id
		{
			get
			{
				return id;
			}
		}

		/// <summary>
		/// Gets the name of this sequence.
		/// </summary>
		
		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Gets a shorter "name", suitable for use as an on-screen label. 
		/// </summary>
		
		public string Label
		{
			get
			{
				return label;
			}
		}

		/// <summary>
		/// Gets a list of cross reference ids associated with this sequence.
		/// </summary>
		
		public IEnumerable<QUT.Bio.Util.Xref> Xref
		{
			get
			{
				return xref;
			}
		}

		/// <summary>
		/// Gets the length of this sequence.
		/// </summary>
		
		public long Length
		{
			get
			{
				return length;
			}
		}
		
		#endregion

		#region Public methods

		/// <summary>
		/// Compare this sequence to another, based on Id.
		/// </summary>
		/// <param name="other">
		///		An object that implements ILinearDomain against which this object is to be 
		///		compared.
		///	</param>
		/// <returns>
		///		Returns the result obtained by comparing the Id of this sequence with that of 
		///		the comparison sequence.
		///	</returns>
		
		public int CompareTo ( ILinearDomain other )
		{
			return id.CompareTo(other.Id);
		}

		/// <summary>
		/// Determine equality (by Id) of two ILinearDomain objects.
		/// </summary>
		/// <param name="other">
		///		An object that implements ILinearDomain against which this object is to be 
		///		compared.
		///	</param>
		/// <returns>
		///		Returns true if and only if the respective Id fields of this object and the 
		///		comparison object are equal.
		///	</returns>
		
		public bool Equals ( ILinearDomain other )
		{
			return CompareTo(other) == 0;
		}

		/// <summary>
		///		Determine equality (by Id) of two sequences.
		/// </summary>
		/// <param name="other">
		///		An object
		/// </param>
		/// <returns>
		///		Returns true if and only if the comparison object implements ILinearDomain 
		///		and the respective Id fields of this object and the comparison object are equal.
		///	</returns>
		
		public override bool Equals(object obj)
		{
			return obj is ILinearDomain ? Equals((ILinearDomain)obj) : false;
		}

		/// <summary>
		/// Get a suitable hash code, consistent with Equals.
		/// </summary>
		/// <returns></returns>
		
		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		/// <summary>
		/// Compare this sequence to another
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		
		public int CompareTo(object obj)
		{
			return CompareTo((SilverMapSequence)obj);
		}
		
		#endregion
	}
}
