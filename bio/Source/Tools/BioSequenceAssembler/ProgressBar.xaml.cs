namespace SequenceAssembler
{
	#region -- Using directives --

	using System.Windows.Controls;

	#endregion -- Using directives --

	/// <summary>
	/// Interaction logic for ProgressBar.xaml. Progress bar will display
	/// the progress of 
	/// 1. Parsing the sequences stored in GenBank and Fasta files.
	/// 2. Assembly process.
	/// 3. Web service execution process.
	/// </summary>
	public partial class ProgressBar : UserControl
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ProgressBar()
		{
			InitializeComponent();
		}
	}
}
