using System;
using System.IO;
using System.Text;

using Bio.Extensions;
using Bio.Phylogenetics;
using Bio.Properties;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace Bio.IO.Newick
{
    /// <summary>
    /// A NewickParser reads from a source of text that is formatted according to 
    /// the Newick notation flat file specification, and converts the data to 
    /// in-memory PhylogeneticTree object.  
    /// Documentation for the latest Newick file format can be found at
    /// http://evolution.genetics.washington.edu/phylip/newicktree.html
    /// http://en.wikipedia.org/wiki/Newick_format
    /// </summary>
    public class NewickParser : IPhylogeneticTreeParser
    {
        private static readonly char[] NewickSpecialCharacters = { ',', '(', ')', ':', ';' };
        const string DefaultTreeName = "PhylogeneticTree";

        /// <summary>
        /// Gets the type of Parser i.e NEWICK.
        /// This is intended to give developers some information 
        /// of the parser class.
        /// </summary>
        public string Name
        {
            get
            {
                return Resource.NEWICK_NAME;
            }
        }

        /// <summary>
        /// Gets the description of Newick parser.
        /// This is intended to give developers some information 
        /// of the parser class. This property returns a simple description of what the
        /// NewickParser class achieves.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.NEWICK_PARSER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma separated values of the possible
        /// file extensions for a Newick file.
        /// </summary>
        public string SupportedFileTypes
        {
            get
            {
                return Resource.NEWICK_FILE_EXTENSION;
            }
        }

        /// <summary>
        /// Name for the tree when parsing. If not set, this will be
        /// set to a default value of "PhylogeneticTree"
        /// </summary>
        public string TreeName
        {
            get;
            set;
        }

        /// <summary>
        /// Parses a Phylogenetic tree text from a StringBuilder into a PhylogeneticTree.
        /// </summary>
        /// <param name="textData">A stream for a Phylogenetic tree text.</param>
        /// <returns>A new PhylogeneticTree instance containing parsed data.</returns>
        public Tree Parse(StringBuilder textData)
        {
            if (textData == null)
            {
                throw new ArgumentNullException("textData");
            }

            return InternalParse(new StringReader(textData.ToString()));

        }

        /// <summary>
        /// Parses a Phylogenetic tree text from a reader into a PhylogeneticTree.
        /// </summary>
        /// <param name="stream">A stream for a Phylogenetic tree text.</param>
        /// <returns>A new PhylogeneticTree instance containing parsed data.</returns>
        public Tree Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var reader = stream.OpenRead())
            {
                return InternalParse(reader);
            }
        }

        /// <summary>
        /// Internal parse method used to parse out the data from a low-level TextReader.
        /// </summary>
        /// <param name="reader">Reader</param>
        /// <returns>Parsed Tree</returns>
        protected Tree InternalParse(TextReader reader)
        {
            string treeName = TreeName;
            if (string.IsNullOrWhiteSpace(treeName))
                treeName = DefaultTreeName;

            Tree tree = new Tree { Name = treeName };
            Node rootNode = GetNode(reader, true).Key;

            if (Peek(reader) != ';')
            {
                if (Peek(reader) == ':')
                {
                    ReadChar(reader);
                }
            }

            // move to next char after ";"
            char semicolon = ReadChar(reader);
            if (semicolon != ';')
            {
                throw new FormatException(string.Format
                    (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                    "Missing [;] at the end of tree text"));
            }

            tree.Root = rootNode;
            return tree;
        }

        /// <summary>
        /// Get collection of (Branch and Leaf) nodes
        /// </summary>
        /// <param name="reader">TextReader</param>
        /// <param name="isRoot"></param>
        /// <returns>PhylogeneticNode object</returns>
        private KeyValuePair<Node, Edge> GetNode(TextReader reader, bool isRoot)
        {
            char firstPeek = Peek(reader);
            Node node = firstPeek == '(' ? this.GetBranch(reader, isRoot) : this.GetLeaf(reader);
            Edge edge = new Edge();

            if (isRoot)
            {
                edge.Distance = double.NaN;
                char secondPeek = Peek(reader);
                if (!NewickSpecialCharacters.Contains(secondPeek))
                {
                    node.Name = GetName(reader);
                    secondPeek = Peek(reader);
                }
                if (secondPeek == ':')
                {   // move to next char after ":"
                    ReadChar(reader);
                    edge.Distance = ReadLength(reader);
                    secondPeek = Peek(reader);
                }                
                if (secondPeek == '[')
                {
                    node.MetaData = GetExtendedMetaData(reader);
                }
            }
            else
            {
                char thirdPeek = Peek(reader);
                if (!NewickSpecialCharacters.Contains(thirdPeek))
                {
                    node.Name = GetName(reader);
                    thirdPeek = Peek(reader);
                }

                if (thirdPeek == ':')
                {
                    //move to next char after ":"
                    char colon = ReadChar(reader);
                    if (colon != ':')
                    {
                        throw new FormatException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name, 
                            "Missing [:] before length"));
                    }
                }
                edge.Distance = ReadLength(reader);
                //Now to check for New Hampshire extended fields
                thirdPeek = Peek(reader);
                if (thirdPeek == '[')
                {
                    node.MetaData = GetExtendedMetaData(reader);
                }
            }
            return new KeyValuePair<Node, Edge>(node, edge);
        }
 
        /// <summary>
        /// Parse Newick extend format files
        /// http://home.cc.umanitoba.ca/~psgendb/doc/atv/NHX.pdf
        /// If the string "[(ampersand)(ampersand)NHX" is not present, throws an error (note ampersand used instead of the xml for some nonsense xml parsing reasons) 
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetExtendedMetaData(TextReader reader)
        {
            var builder=new StringBuilder();
            while(true)
            {
                char peek = Peek(reader);
                builder.Append(ReadChar(reader));
                if(peek==']')
                {                    
                    break;
                }
            }
            var str=builder.ToString();
            //verify extended format and run with it.
            if (!str.StartsWith("[&&NHX:") || !str.EndsWith("]"))
            {
                throw new FormatException("Was expecting a New Hampshire extended field collection, but found parser error.  Bad string is: " + str); 
            }
            int start="[&&NHX:".Length;
            str = str.Substring(start, (str.Length - 1 - start));
            string[] groups = str.Split(':');
            var result = new Dictionary<string, string>();
            foreach (var g in groups)
            {
                var sub=g.Split('=');
                if(sub.Length!=2)
                {
                    throw new FormatException("Could not split NHX metadata key/value pair: "+g);
                }
                result[sub[0]] = sub[1];
            }
            return result;
        }

        /// <summary>
        /// Get the name from the data.
        /// </summary>
        /// <returns></returns>
        private string GetName(TextReader reader)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (true)
            {
                char peek = Peek(reader);
                if (NewickSpecialCharacters.Contains(peek))
                {
                    break;
                }
                stringBuilder.Append(ReadChar(reader));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the Branch node
        /// </summary>
        /// <param name="reader">TextReader we are working with</param>
        /// <param name="isRoot"></param>
        /// <returns>Branch object</returns>
        private Node GetBranch(TextReader reader, bool isRoot)
        {
            Node branch = new Node { IsRoot = isRoot };

            bool firstBranch = true;
            while (true)
            {
                char peek = Peek(reader);
                if (!firstBranch && peek == ')')
                {
                    break;
                }

                char c = ReadChar(reader);
                if (firstBranch)
                {
                    if (c != '(')
                    {
                        throw new FormatException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                            "Missing [(] as a first branch"));
                    }
                    firstBranch = false;
                }
                else
                {
                    if (c != ',')
                    {
                        throw new FormatException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                            "Missing [,] in between branches"));
                    }
                }

                var tmp = GetNode(reader, false);
                branch.Children.Add(tmp.Key, tmp.Value);
            }

            //move to next char of ")"
            char nextCloseChar = ReadChar(reader);
            if (nextCloseChar != ')')
            {
                throw new FormatException(string.Format
                    (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                    "Missing [)] branch is not closed"));
            }

            return branch;
        }

        /// <summary>
        /// Get the Leaf node
        /// </summary>
        /// <returns>Leaf object</returns>
        private Node GetLeaf(TextReader reader)
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (true)
            {
                char peek = Peek(reader);
                if (peek == ':')
                {
                    break;
                }
                if (("()".Contains(peek.ToString())))
                {
                    throw new FormatException(string.Format
                        (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                        "Missing [:] as part of leaf"));

                }
                stringBuilder.Append(ReadChar(reader));
            }

            return new Node { Name = stringBuilder.ToString() };
        }

        /// <summary>
        /// Peeks the TextReader char by char 
        /// </summary>
        /// <returns>a character</returns>
        private char Peek(TextReader reader)
        {
            while (true)
            {
                int peek = reader.Peek();
                if (peek == -1)
                {
                    throw new FormatException(string.Format
                        (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                        "Missing format, Unable to read further "));
                }

                char c = (char)peek;
                if (c != '\r' && c != '\n')
                {
                    return c;
                }
                reader.Read();
            }
        }

        /// <summary>
        /// Reads Length
        /// </summary>
        /// <returns>length</returns>
        private double ReadLength(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char peek = Peek(reader);
                if (!"-0123456789.E ".Contains(peek.ToString()))
                {
                    break;
                }
                sb.Append(ReadChar(reader));
            }
            // consider -ive and +ive length
            if (sb.Length == 0)
            {
                throw new FormatException("Attempted to read length when no length string was given");
            }
            else
            {
               return double.Parse(sb.ToString(), CultureInfo.InvariantCulture); 
            }
            
        }

        /// <summary>
        /// Reads current character
        /// </summary>
        /// <returns>a character</returns>
        private char ReadChar(TextReader reader)
        {
            while (true)
            {
                char c = (char) reader.Read();
                if (c != '\r' && c != '\n')
                {
                    return c;
                }
            }
        }
    }
}
