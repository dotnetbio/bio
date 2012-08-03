using System;
using System.IO;

namespace Bio.Web.BioHPC
{
    /// <summary>
    /// A class of wrapper functions utilizing the "atomic" methods
    /// exported by the BioHPC web service. Currently contains the file staging methods.
    /// </summary>
    public class BioHPCClient : BioHPCWebService
    {
        /// <summary>
        /// Initializes a new instance of the BioHPCClient class
        /// </summary>
        public BioHPCClient()
        {
        }

        /// <summary>
        /// Download a job's output file
        /// </summary>
        /// <param name="jobid">BioHPC ID of the job</param>
        /// <param name="cntrl">control number</param>
        /// <param name="fname">Name of the file to download (as returned by RequestOutFileName, for example)</param>
        /// <param name="outdir">Output directory - the user has to have write permissions there</param>
        /// <returns>Report from the download operation.</returns>
        public string DownloadFile(string jobid, string cntrl, string fname, string outdir)
        {
            string result = String.Empty;
            long numBytesMax = 3500000;

            // First, find the size of the remote file in bytes
            long fsize = this.QueryFileLength(jobid, cntrl, fname);
            if (fsize == 0)
            {
                result += "File " + fname + " absent or empty";
                return result;
            }

            result += "File size detected: " + fsize.ToString(System.Globalization.CultureInfo.CurrentCulture) + "\n";

            System.IO.FileStream fs1 = null;

            try
            {
                fs1 = new FileStream(outdir + "\\" + fname, FileMode.Create);
            }
            catch
            {
                result += "ERROR: File could not be open.\n";
                return result;
            }

            long current_position = 0;
            long nbytes_totran = 0;
            int counter = 0;
            while (current_position < fsize - 1)
            {
                counter++;
                nbytes_totran = Math.Min(numBytesMax, fsize - current_position);
                result += "Downloading chunk " + counter.ToString(System.Globalization.CultureInfo.CurrentCulture) + " of size " + nbytes_totran.ToString(System.Globalization.CultureInfo.CurrentCulture) + "\n";

                // Call the procedure to start from current_position and return nbytes_totran
                byte[] b1 = null;
                b1 = this.DownloadFileChunk(jobid, cntrl, fname, current_position, nbytes_totran);
                fs1.Write(b1, 0, b1.Length);
                current_position += nbytes_totran;
            }

            fs1.Close();
            result += "File downloaded successfully as " + outdir + "\\" + fname + "\n";

            return result;
        }

        /// <summary>
        /// Uploads an input file to the job's directory on the BioHPC server
        /// </summary>
        /// <param name="jobid">job ID (as obtained from CreateNewJob)</param>
        /// <param name="cntrl">conrol number (as obtained from CreateNewJob)</param>
        /// <param name="filename">Path (on local machine) to the file to be uploaded</param>
        /// <param name="strFile">Name the file should have on the server (for example, as given by the InputFileName field of the
        /// input data structure initialized by InitializeInputParameters).</param>
        /// <returns>Report from the upload operation.</returns>
        public string UploadFile(string jobid, string cntrl, string filename, string strFile)
        {
            string result = String.Empty;
            long numBytesMax = 3500000;
            try
            {
                // get the file information form the selected file and get the reader
                FileInfo flei = new FileInfo(filename);
                using (FileStream fstr = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader br = new BinaryReader(fstr))
                    {
                        // get the length of the file
                        long numBytes = flei.Length;
                        result += "File size: " + numBytes.ToString(System.Globalization.CultureInfo.CurrentCulture) + "\n";
                        long numBytes2read = numBytes;
                        long chunk_bytes;
                        string stmp = String.Empty;
                        bool first = true;
                        int counter = 0;
                        while (numBytes2read > 0)
                        {
                            counter++;
                            chunk_bytes = Math.Min(numBytesMax, numBytes2read);
                            result += "Uploading chunk " + counter.ToString(System.Globalization.CultureInfo.CurrentCulture) + " of size " + chunk_bytes.ToString(System.Globalization.CultureInfo.CurrentCulture) + "\n";

                            // Read the file chunk into a byte array.....
                            byte[] data = br.ReadBytes((int)chunk_bytes);

                            // ...and pass to the web service along with the name of the file to append to.
                            // If it is the first chunk, the file will be open on the server side
                            stmp = this.UploadFileChunk(jobid, cntrl, data, strFile, first);
                            result += stmp + "\n";
                            first = false;

                            // Number of bytes left to read
                            numBytes2read = numBytes2read - chunk_bytes;
                        }

                        result += "Whole file uploaded in " + counter.ToString(System.Globalization.CultureInfo.CurrentCulture) + " steps \n";

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                // display an error message to the user
                result += ex.Message.ToString() + "\n\nUpload Error \n";
                return result;
            }
        }
    }
}
