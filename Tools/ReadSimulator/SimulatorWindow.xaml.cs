using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bio.IO;
using Microsoft.Win32;

namespace ReadSimulator
{
    /// <summary>
    /// Interaction logic for SimulatorWindow.xaml
    /// </summary>
    public partial class SimulatorWindow : Window
    {
        private readonly SimulatorController controller = new SimulatorController();

        /// <summary>
        /// Default constructor for the window
        /// </summary>
        public SimulatorWindow()
        {
            DataContext = controller;
            InitializeComponent();
        }

        // Updates UI related to the input sequence. Should be called any time that
        // sequence is changed
        private void UpdateInputSequenceInfo()
        {
            if (controller.SequenceToSplit == null)
            {
                InputSequenceStatus.Text = FindResource("NotLoaded").ToString();
                InputSequenceSize.Text = FindResource("NoBasePairCount").ToString();
            }
            else
            {
                InputSequenceStatus.Text = controller.SequenceToSplit.ID;
                InputSequenceSize.Text = string.Format((string)FindResource("BasePairCount"), controller.SequenceToSplit.Count);
            }

            // Clear any previous output status message
            OutputSequenceStatus1.Text = string.Empty;
            OutputSequenceStatus2.Text = string.Empty;
        }

        // Updates UI related to information that is known just before performing the simulation
        internal void UpdateSimulationStats(long sequenceCount, long fileCount)
        {
            OutputSequenceStatus1.Text = string.Format((string)FindResource("OutputStats"), sequenceCount, fileCount);
        }

        // Updates UI related the the results of simulation
        internal void NotifySimulationComplete(string formatterName)
        {
            if (!string.IsNullOrEmpty(formatterName))
            {
                OutputSequenceStatus2.Text = 
                    string.Format(FindResource("FinishedOutput").ToString(), formatterName);
            }
            else
            {
                string outMessage = FindResource("NoOutputCreated").ToString();
                MessageBox.Show(outMessage, this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                OutputSequenceStatus2.Text = outMessage;
            }
        }

        /// <summary>
        /// Loads the currently opened input file
        /// </summary>
        /// <param name="filename">File to load</param>
        private bool LoadInputFile(string filename)
        {
            try
            {
                controller.ParseSequence(filename);
                UpdateInputSequenceInfo();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error parsing the input file: " + ex.Message, "Parsing Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        /// <summary>
        /// Runs the simulator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimulationButton_Click(object sender, RoutedEventArgs e)
        {
            if (controller.SequenceToSplit == null)
            {
                MessageBox.Show(this, "Please load a sequence before attempting simulation", 
                    "Simulation Not Ready", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                controller.DoSimulation(OutputFileBox.Text, UpdateSimulationStats, NotifySimulationComplete);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the file chooser dialog to select an input sequence file
        /// </summary>
        private void InputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string filterString = controller.QuerySupportedFileType()
                    .Aggregate(string.Empty, (current, filetype) => current + (filetype + "|"));

            filterString += "All Files|*.*";
            openFileDialog.Filter = filterString;
            openFileDialog.CheckFileExists = true;

            if (openFileDialog.ShowDialog(this) == true)
            {
                InputFileBox.Text = openFileDialog.FileName;
                InputFileBox_Changed(null, null);
            }
        }

        // Opens the file chooser dialog to select an output sequence file
        private void OutputBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Use only fasta for save dialog.
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = SequenceParsers.Fasta.Name + "|" + 
                SequenceParsers.Fasta.SupportedFileTypes.Replace(".","*.").Replace(",",";");

            if (saveFileDialog.ShowDialog(this) == true)
            {
                OutputFileBox.Text = saveFileDialog.FileName;
                OutputFileBox_Changed(null, null);
            }
        }

        // Sets the settings data to one of the known default settings
        private void DefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            if (button.Name == "SettingDefaultSanger")
                controller.Settings.SetDefaults(DefaultSettings.SangerDideoxy);
            if (button.Name == "SettingDefaultPyro")
                controller.Settings.SetDefaults(DefaultSettings.PyroSequencing);
            if (button.Name == "SettingDefaultShort")
                controller.Settings.SetDefaults(DefaultSettings.ShortRead);
        }

        /// <summary>
        /// Called when the input file textbox is changed either through the Browse button
        /// or by typing into the box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputFileBox_Changed(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            string inputFileName = InputFileBox.Text;
            if (!string.IsNullOrEmpty(inputFileName) && LoadInputFile(inputFileName))
            {
                SimulationButton.IsEnabled = true;

                if (String.IsNullOrEmpty(OutputFileBox.Text) && File.Exists(inputFileName))
                {
                    OutputFileBox.Text = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        Path.GetFileNameWithoutExtension(inputFileName) + "_out.fa");
                }
            }
            else
            {
                SimulationButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Called when the output file textbox is changed either through the Browse button
        /// or manually by typing into the box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutputFileBox_Changed(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            try
            {
                string filename = OutputFileBox.Text;
                if (Directory.Exists(filename))
                {
                    // Missing filename -- add the input file, or default.
                    string inputFileName;
                    if (!string.IsNullOrEmpty(InputFileBox.Text))
                    {
                        inputFileName = Path.GetFileName(InputFileBox.Text);
                    }
                    else
                    {
                        inputFileName = "readsimulator";
                    }
                    OutputFileBox.Text = Path.Combine(filename,
                                                      Path.GetFileNameWithoutExtension(inputFileName) + "_out.fa");
                    return;
                }

                // Has filename, use existing filename
                string directory = Path.GetDirectoryName(filename);
                if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
                {
                    MessageBox.Show("Selected Output directory and filename is not valid.", this.Title,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

                // Add our extension if one is not supplied.
                filename = Path.GetFileName(filename);
                if (Path.GetExtension(filename) == string.Empty)
                {
                    OutputFileBox.Text += "_out.fa";
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(OutputFileBox.Text) && File.Exists(OutputFileBox.Text))
                {
                    MessageBox.Show(
                        OutputFileBox.Text +
                        " is an existing file. It will be overwritten by the generated output files if you run the simulator using this output. Change the filename or directory if you do not wish to overwrite this file.",
                        this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
