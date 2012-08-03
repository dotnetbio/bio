namespace SequenceAssembler
{
	#region -- Using Directives --

	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Navigation;
	using QUT.Bio.SilverMap;
	using QUT.Bio.Util;
	using SequenceAssembler.SilverMapIntegration;
	using DataProvider = SequenceAssembler.SilverMapIntegration.DataProvider;

	#endregion

	/// <summary>
	/// Hosts a SilverMap MapControl, connecting to incoming BLAST results. 
	/// </summary>
	public partial class SilverMapController : UserControl
	{
		#region -- Private Members --

		/// <summary>
		/// Describes the oldResult loaded on the Map
		/// </summary>
		private IList<BlastResultCollator> oldResult;

		/// <summary>
		/// The radius at which BLAST hits with eValue = 0 will be rendered.
		/// This artificial displacement prevents multiple hits from covering 
		/// the query sequence at the centre of the display.
		/// </summary>
		private const double InnerDisplayRadius = 0.1;

		/// <summary>
		/// The maximum zoom factor for the map.
		/// </summary>
		private const double MaximumScale = 20;

		#endregion

		#region -- Constructor --

		/// <summary>
		/// Initializes a new instance of the SilverMapController class.
		/// </summary>
		public SilverMapController ()
		{
			InitializeComponent();

			map.Initialize( overlay, null, null, new DefaultSilverMapAdapter(
				map, overlay, InnerDisplayRadius, new string[] { "Name", "DistanceFromReferenceSequence" }
			) );

			map.VerticalAlignment = VerticalAlignment.Stretch;
			map.View.GraphView.Canvas.MaximumScale = MaximumScale;
			map.Model.DataProducer = new DataProvider( overlay );
		}
		#endregion

		#region -- Public Properties --

		/// <summary>
		/// Gets or sets the BlastSerializer for the SilverMap 
		/// </summary>
		public IBlastSerializer BlastSerializer
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the Blast output from the selected webservice
		/// </summary>
		public IList<BlastResultCollator> BlastResult
		{
			get;
			private set;
		}
		#endregion

		#region -- Public Method --

		/// <summary>
		/// Updates the list of blast results then causes SilverMap to redraw the display.
		/// </summary>
		/// <param name="resultCollator">Blast hits result</param>
		public void InvokeSilverMap ( IList<BlastResultCollator> resultCollator )
		{
			this.BlastResult = resultCollator;

			if ( this.IsNewResult() )
			{
				if ( this.oldResult == null )
				{
					this.oldResult = new List<BlastResultCollator>();
				}
				else
				{
					this.oldResult.Clear();
				}

				foreach ( BlastResultCollator result in this.BlastResult )
				{
					this.oldResult.Add( result );
				}

				Refresh();
			}
		}
		#endregion

		#region -- Private Static methods --

		/// <summary>
		/// Gets the Xml Serialized data from the given stream.
		/// </summary>
		/// <param name="stream">memory stream</param>
		/// <returns>serialized blast string</returns>
		private static string GetSerializedData ( Stream stream )
		{
			string xml = string.Empty;
			MemoryStream memStream = stream as MemoryStream;
			if ( memStream != null )
			{
				xml = Encoding.UTF8.GetString( memStream.GetBuffer() );
				xml = xml.Substring( xml.IndexOf( Convert.ToChar( 60 ) ) );
				xml = xml.Substring( 0, ( xml.LastIndexOf( Convert.ToChar( 62 ) ) + 1 ) );
				memStream.Close();
			}

			return xml;
		}
		#endregion

		#region -- Private Methods --

		#region IsNewResult
		/// <summary>
		/// This method validates whether the given Blast 
		/// output is new result or old result
		/// </summary>
		/// <returns>whether the result is new or not</returns>
		private bool IsNewResult ()
		{
			if ( this.BlastResult == null || this.oldResult == null )
			{
				return true;
			}

			if ( this.BlastResult.Count != this.oldResult.Count )
			{
				return true;
			}

			if ( this.BlastResult.Count > 0 && this.oldResult.Count > 0 && !this.BlastResult[0].Accession.Equals( this.oldResult[0].Accession ) )
			{
				return true;
			}

			return false;
		}
		#endregion

		#region Method: Refresh
		/// <summary>
		/// Updates the displayed blast hits in the map.
		/// <para>Precondition: this.BlastResult is not null.</para>
		/// <para>Postcondition: Unique id has been allocated for each distinct subject and query
		/// and the contents of map.Model has been replaced with new chromosome, gene and blast
		/// collections derived from the current value of BlastResult.
		/// </para>
		/// </summary>

		private void Refresh ()
		{
			( map.Model.DataProducer as DataProvider ).Load( BlastResult );
		}
		#endregion

		#endregion
	}
}
