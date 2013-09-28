using System;
using System.IO;
using System.Text;
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
    /// Documentation for the latest Newic file format can be found at
    /// http://evolution.genetics.washington.edu/phylip/newicktree.html
    /// http://en.wikipedia.org/wiki/Newick_format
    /// </summary>
    public class NewickParser : IPhylogeneticTreeParser, IDisposable
    {
        #region -- Member Variables --
        private TextReader _textReader;
        private static char[] newickSpecialCharacters = new char[] { ',', '(', ')', ':', ';' };

        //default tree name
        private string _treeName = "PhylogeneticTree";
        #endregion -- Member Variables --

       

        #region -- Constructors --
        /// <summary>
        /// Default constructor.
        /// </summary>
        public NewickParser()
            : base()
        {
        }
        #endregion -- Constructors --

        #region -- Public Methods --
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
        /// NewickParser class acheives.
        /// </summary>
        public string Description
        {
            get
            {
                return Resource.NEWICK_PARSER_DESCRIPTION;
            }
        }

        /// <summary>
        /// Gets a comma seperated values of the possible
        /// file extensions for a Newick file.
        /// </summary>
        public string FileTypes
        {
            get
            {
                return Resource.NEWICK_FILE_EXTENSION;
            }
        }

        /// <summary>
        /// Parses a Phylogenetic tree text from a string into a PhylogeneticTree.
        /// </summary>
        /// <param name="treeBuilder">Phylogenetic tree text.</param>
        /// <returns>A new PhylogeneticTree instance containing parsed data.</returns>
        public Tree Parse(StringBuilder treeBuilder)
        {
            if (treeBuilder == null)
            {
                throw new ArgumentNullException("treeBuilder");
            }

            _textReader = new StringReader(treeBuilder.ToString());
            return Parse();
        }

        /// <summary>
        /// Parses a Phylogenetic tree text from a reader into a PhylogeneticTree.
        /// </summary>
        /// <param name="reader">A reader for a Phylogenetic tree text.</param>
        /// <returns>A new PhylogeneticTree instance containing parsed data.</returns>
        public Tree Parse(TextReader reader)
        {
            _textReader = reader;
            return Parse();
        }
        
        /// <summary>
        /// Parses a Phylogenetic tree text from a file.
        /// </summary>
        /// <param name="fileName">The name of a Phylogenetic tree file.</param>
        /// <returns>PhylogeneticTree object.</returns>
        public Tree Parse(string fileName)
        {
            _textReader = File.OpenText(fileName);
            _treeName = Path.GetFileNameWithoutExtension(fileName);
            return Parse();
        }
        #endregion -- Public Methods --

        #region -- Private Methods --
        /// <summary>
        /// Parses a Phylogenetic tree text from the local text reader
        /// </summary>
        /// <returns>Parsed PhylogeneticTree object.</returns>
        private Tree Parse()
        {
            Tree tree = new Tree();
            tree.Name = _treeName;

            Node rootNode = GetNode(true).Key;

            if (_textReader.Peek() != ';')
            {
                if (_textReader.Peek() == ':')
                {
                    ReadChar();
                }

            }

            // move to next char after ";"
            char semicolon = ReadChar();
            if (semicolon != ';')
            {
                throw new FileFormatException(string.Format
                    (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name, 
                    "Missing [;] at the end of tree text"));
            }

            tree.Root = rootNode;

            _textReader.Close();
            return tree;
        }

        /// <summary>
        /// Get collection of (Branche and Leaf) nodes
        /// </summary>
        /// <param name="isRoot"></param>
        /// <returns>PhylogeneticNode object</returns>
        private KeyValuePair<Node, Edge> GetNode(bool isRoot)
        {
            Node node;
            Edge edge = new Edge();

            char firstPeek = Peek();
            if (firstPeek == '(')
            {
                node = GetBranch(isRoot);
            }
            else
            {
                node = GetLeaf();
            }

            if (isRoot)
            {
                edge.Distance = double.NaN;
                char secondPeek = Peek();
                if (!newickSpecialCharacters.Contains(secondPeek))
                {
                    node.Name = GetName();
                    secondPeek = Peek();
                }
                if (secondPeek == ':')
                {   // move to next char after ":"
                    ReadChar();
                    edge.Distance = ReadLength();
                    secondPeek = Peek();
                }                
                if (secondPeek == '[')
                {
                    node.MetaData = GetExtendedMetaData();
                }
             



            }
            else
            {
                char thirdPeek = Peek();
                if (!newickSpecialCharacters.Contains(thirdPeek))
                {
                    node.Name = GetName();
                    thirdPeek = Peek();
                }

                if (thirdPeek == ':')
                {
                    //move to next char after ":"
                    char colon = ReadChar();
                    if (colon != ':')
                    {
                        throw new FileFormatException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name, 
                            "Missing [:] before length"));
                    }
                }
                edge.Distance = ReadLength();
                //Now to check for new hampshire extended fields
                thirdPeek = Peek();
                if (thirdPeek == '[')
                {
                    node.MetaData = GetExtendedMetaData();
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
        private Dictionary<string, string> GetExtendedMetaData()
        {
            var builder=new StringBuilder();
            while(true)
            {
                char peek=Peek();
                builder.Append(ReadChar());
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
        private string GetName()
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (true)
            {
                char peek = Peek();
                if (newickSpecialCharacters.Contains(peek))
                {
                    break;
                }
                stringBuilder.Append(ReadChar());
            }
            return stringBuilder.ToString();
        }
        /// <summary>
        /// Gets the Branch node
        /// </summary>
        /// <param name="isRoot"></param>
        /// <returns>Branch object</returns>
        private Node GetBranch(bool isRoot)
        {
            Node branch = new Node();
            branch.IsRoot = isRoot;

            bool firstBranch = true;
            while (true)
            {
                char peek = Peek();
                if (!firstBranch && peek == ')')
                {
                    break;
                }

                char c = ReadChar();
                if (firstBranch)
                {
                    if (c != '(')
                    {
                        throw new FileFormatException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                            "Missing [(] as a first branch"));
                    }
                    firstBranch = false;
                }
                else
                {
                    if (c != ',')
                    {
                        throw new FileFormatException(string.Format
                            (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                            "Missing [,] in between branches"));
                    }
                }

                KeyValuePair<Node,Edge> tmp = GetNode(false);
                branch.Children.Add(tmp.Key, tmp.Value);
            }

            //move to next char of ")"
            char nextCloseChar = ReadChar();
            if (nextCloseChar != ')')
            {
                throw new FileFormatException(string.Format
                    (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                    "Missing [)] branch is not closed"));
            }

            return branch;
        }

        /// <summary>
        /// Get the Leaf node
        /// </summary>
        /// <returns>Leaf object</returns>
        private Node GetLeaf()
        {
            StringBuilder stringBuilder = new StringBuilder();

            while (true)
            {
                char peek = Peek();
                if (peek == ':')
                {
                    break;
                }
                if (("()".Contains(peek.ToString())))
                {
                    throw new FileFormatException(string.Format
                        (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                        "Missing [:] as part of leaf"));

                }
                stringBuilder.Append(ReadChar());
            }

            Node leaf = new Node();
            leaf.Name = stringBuilder.ToString();
            return leaf;
        }




        /// <summary>
        /// Peeks the TextReader char by char 
        /// </summary>
        /// <returns>a character</returns>
        private char Peek()
        {
            while (true)
            {
                int peek = _textReader.Peek();
                if (peek == -1)
                {
                    throw new FileFormatException(string.Format
                        (CultureInfo.CurrentCulture, Resource.IOFormatErrorMessage, Name,
                        "Missing format, Unable to read further "));
                }
                char c = (char)peek;
                //Environment.NewLine
                if (c != '\r' && c != '\n')
                {
                    return c;
                }
                _textReader.Read();
            }
        }

        /// <summary>
        /// Reads Length
        /// </summary>
        /// <returns>length</returns>
        private double ReadLength()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char peek = Peek();
                if (!"-0123456789.E ".Contains(peek.ToString()))
                {
                    break;
                }
                sb.Append(ReadChar());
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
        private char ReadChar()
        {
            while (true)
            {
                char c = (char)_textReader.Read();
                if (c != '\r' && c != '\n')
                {
                    return c;
                }
            }
        }
        #endregion -- Private Methods --

        #region IDisposable Members
        /// <summary>
        /// Implements dispose to supress GC finalize
        /// This is done as one of the methods uses ReadWriterLockSlim
        /// which extends IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose field instances
        /// </summary>
        /// <param name="disposeManaged">If disposeManaged equals true, clean all resources</param>
        protected virtual void Dispose(bool disposeManaged)
        {
            if (disposeManaged)
            {
                if (_textReader != null)
                    _textReader.Dispose();
                _textReader = null;
                _treeName = null;
            }
        }
        #endregion
    }
}
