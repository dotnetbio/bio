using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Bio;
using Bio.Algorithms.Translation;
using Bio.Controls;
using Bio.IO;
using Bio.IO.FastA;
using Bio.IO.GenBank;
using Microsoft.Win32;

namespace ReadSimulator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private OpenFileDialog genBankFileDialog = new OpenFileDialog();
        private OpenFileDialog gffFileDialog = new OpenFileDialog();
        private OpenFileDialog fastaFileDialog = new OpenFileDialog();

        public TestWindow()
        {
            InitializeComponent();
            InitializeDataMembers();
        }

        // Reads a file location from a OpenFileDialog and places the resulting path into a specified TextBox
        private void BrowseFile(OpenFileDialog dialog, TextBox resultTextBox)
        {
            if (dialog.ShowDialog(this) == true)
            {
                resultTextBox.Text = dialog.FileName;
            }
        }

        // Parses a sequence and adds it to the displayed list

        private void ParseSequence(ISequenceParser parser, string filename)
        {
            parser.Open(filename);
            IEnumerable<ISequence> parsed = parser.Parse();
            ListBox sequenceList = (ListBox)FindName("SequencesListBox");
            foreach (ISequence seq in parsed)
            {
                sequenceList.Items.Add(seq);
            }
            parser.Close();
        }

        private void InitializeDataMembers()
        {
            /*
            genBankFileDialog.Filter = "*.gbk";
            gffFileDialog.Filter = "*.gff";
            fastaFileDialog.Filter = "*.fasta";
            */
        }

        #region Event Handlers

        private void GenBankBrowse_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)FindName("GenBankFile");
            BrowseFile(genBankFileDialog, tb);
        }

        private void GffBrowse_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)FindName("GffFile");
            BrowseFile(gffFileDialog, tb);
        }

        private void FastaBrowse_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)FindName("FastaFile");
            BrowseFile(fastaFileDialog, tb);
        }

        private void GenBankParse_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)FindName("GenBankFile");
            FileInfo fi = new FileInfo(tb.Text);
            ParseSequence(new GenBankParser(), fi.FullName);
        }

        private void GffParse_Click(object sender, RoutedEventArgs e)
        {
            /*
            TextBox tb = (TextBox)FindName("GffFile");
            FileInfo fi = new FileInfo(tb.Text);
            ParseSequence(new LegacyGffParser(fi.FullName));
             * */
        }

        private void FastaParse_Click(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)FindName("FastaFile");
            FileInfo fi = new FileInfo(tb.Text);
            ParseSequence(new FastAParser(), fi.FullName);
        }
        #endregion

        private void SequencesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox source = (ListBox)sender;
            if (source.SelectedItem != null)
            {
                SequenceViewer viewer = (SequenceViewer)FindName("SequenceViewer");
                viewer.Sequence = (ISequence)source.SelectedItem;
            }
        }

        private void JaredTest_Click(object sender, RoutedEventArgs e)
        {
            Sequence rna = new Sequence(Alphabets.RNA, "AUGGCGCCGAUAAUGACGGUCCUUCCUUGA");
            ISequence protein = ProteinTranslation.Translate(rna);
            string rnaStr = rna.ToString();
            StringBuilder buff = new StringBuilder();
            foreach (byte aa in protein)
            {
                buff.Append((char)aa);
                buff.Append(' ');
            }
            string aaStr = buff.ToString();
        }
        
    }
}
