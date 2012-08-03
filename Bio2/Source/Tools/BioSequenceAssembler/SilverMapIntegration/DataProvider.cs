using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QUT.Bio.SilverMap;
using QUT.Bio.SilverMap.ObjectModel;
using QUT.Bio.Util;
using SequenceAssembler;
using SequenceAssembler.Properties;

namespace SequenceAssembler.SilverMapIntegration
{

	/// <summary>
	/// DataProvider allows a list of SequenceAssembler.BlastResultCollator objects to be 
	/// used as a data source for SilverMap.
	/// </summary>
	
	public class DataProvider : QUT.Bio.SilverMap.DataProvider
	{
		internal readonly FeatureCollection featureCollection = new FeatureCollection();

		// EBI WU-BLAST returns hits with NULL query id. We substitute an automatically created
		// "Unknown query ID NN" id string for each such query.
		private int unknownQuerySerialNumber = 1;
		 
		/// <summary>
		/// Initialise a new DataProvider.
		/// </summary>
		/// <param name="overlay">
		///		A PopupOverlay, legacy of the days when Silverlight did not have any kind of 
		///		popup window function buit in.
		///	</param>
		public DataProvider ( PopupOverlay overlay )
			: base( overlay )
		{
		}

		#region Implement abstract base class.

		/// <summary>
		/// Get a reference to a SilverMap object model.
		/// </summary>
		
		public override IFeatureCollection FeatureCollection
		{
			get
			{
				return featureCollection;
			}
		}

		#endregion

		/// <summary>
		/// Loads the contents of a list of blast result collator objects into the model.
		/// <para>
		///		When finished, fires the LoadComplete event to notify listeners.
		/// </para>
		/// </summary>
		
		public void Load ( IList<BlastResultCollator> blastResults )
		{
			StringWriter errorLog = new StringWriter();

			featureCollection.features.Clear();
			featureCollection.hits.Clear();
			featureCollection.sequences.Clear();
			featureCollection.initialQuerySequence = null;

			foreach ( BlastResultCollator blastResult in blastResults )
			{
				try
				{
					ILinearDomain querySequence = null;
					ILinearDomain hitSequence = null;

					string queryId = blastResult.QueryId == null 
						? String.Format( Resource.SILVERMAP_UNKNOWN_QUERY_ID, unknownQuerySerialNumber ++ )  
						: blastResult.QueryId;

					string subjectId = blastResult.SubjectId == null ? blastResult.Accession : blastResult.SubjectId;

					if ( featureCollection.sequences.ContainsKey( queryId ) )
					{
						querySequence = featureCollection.sequences[queryId];
					}
					else
					{
						querySequence = new SilverMapSequence(
							queryId,
							Resource.SILVERMAP_QUERY_SEQUENCE,
							queryId,
							blastResult.Length
						);
						featureCollection.sequences.Add( queryId, querySequence );
					}

					if ( featureCollection.initialQuerySequence == null )
					{
						featureCollection.initialQuerySequence = querySequence;
					}

					featureCollection.querySequences.Add( querySequence );

					if ( featureCollection.sequences.ContainsKey( subjectId ) )
					{
						hitSequence = featureCollection.sequences[subjectId];
					}
					else
					{
						hitSequence = new SilverMapSequence(
							subjectId,
							blastResult.Description,
							subjectId,
							blastResult.Length
						);
						featureCollection.sequences.Add( subjectId, hitSequence );
					}

					long queryStart = Math.Min( blastResult.QStart, blastResult.QEnd );
					long queryEnd = Math.Max( blastResult.QStart, blastResult.QEnd );

					DefaultFeature queryFeature = new DefaultFeature(
						querySequence,
						queryStart,
						queryEnd,
						blastResult.QueryString
					);

					long subjectStart = Math.Min( blastResult.SStart, blastResult.SEnd );
					long subjectEnd = Math.Max( blastResult.SStart, blastResult.SEnd );

					DefaultFeature hitFeature = new DefaultFeature(
						hitSequence,
						subjectStart,
						subjectEnd,
						blastResult.SubjectString
					);

					featureCollection.features.Add( queryFeature );
					featureCollection.features.Add( hitFeature );

					featureCollection.hits.Add( new Hit( queryFeature, hitFeature )
					{
						EValue = blastResult.EValue,
						Identities = blastResult.Identity,
						Midline = Hit.SEQUENCE_DATA_UNAVAILABLE,
						Positives = blastResult.Positives
					} );
				}
				catch ( Exception e )
				{
					errorLog.WriteLine( String.Format( Resource.SILVERMAP_ERROR_PROCESSING_BLAST_RESULT, e ) );
				}
			}

			if ( errorLog.GetStringBuilder().Length > 0 )
			{
				MessageDialogBox.Show( errorLog.ToString(), Resource.CAPTION, MessageDialogButton.OK );
			}

			SignalLoadComplete();
		}
	}
}
