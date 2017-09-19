using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Bio.Util;
using System.Globalization;

namespace Bio.Matrix
{

    /// <summary>
    /// The type of functions that can create new Matrix objects from a file.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    /// <param name="filename">The name of the file containing the matrix information.</param>
    /// <param name="missingValue">The special value that represents missing in the created matrix.</param>
    /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
    /// <param name="matrix">The matrix created.</param>
    /// <returns>true if the function was able to create a matrix from the information in the file; otherwise, false</returns>
    public delegate bool TryParseMatrixDelegate<TRowKey, TColKey, TValue>(string filename, TValue missingValue, ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> matrix);

    /// <summary>
    /// A class for creating a factory for parsing matrix files.
    /// </summary>
    /// <typeparam name="TRowKey">The type of the row key. Usually "String"</typeparam>
    /// <typeparam name="TColKey">The type of the col key. Usually "String"</typeparam>
    /// <typeparam name="TValue">The type of the value, for example, double, int, char, etc.</typeparam>
    [Serializable]
    public class MatrixFactory<TRowKey, TColKey, TValue>
    {
        // First value is the class name, second is the method name. Method must have the signature 
        // bool MethodName(string filename, TValue missingValue, out Matrix<TRowKey, TColKey, TValue> matrix)
        // The class name can either require all three generic types, or have all 3 hard coded. Currently there is no support for only some being hard coded.
        // Each parser is tried in order, with the first successful one being used, so they should be ordered from most to least specific.
        // Note that user-defined registered parsers are tried first.

        //!!!should raise an error if no such method
        static Tuple<string, string>[] _defaultParserNames = new[]{
            Tuple.Create("Bio.Matrix.DenseAnsi", "TryGetInstance"),
            Tuple.Create("Bio.Matrix.PaddedDouble", "TryGetInstance"),
            Tuple.Create("Bio.Matrix.SparseMatrix", "TryParseSparseFile"),
            Tuple.Create("Bio.Matrix.DenseMatrix", "TryParseRFileWithDefaultMissing"),
            Tuple.Create("Bio.Matrix.DenseMatrix", "TryParseTabbedRFileWithDefaultMissing"),
            Tuple.Create("Bio.Matrix.DenseAnsi", "TryParseDenseAnsiFormatAsDoubleMatrix"),
            Tuple.Create("Bio.Matrix.DenseAnsi", "TryParseDenseAnsiFormatAsGenericMatrix")
        };

        List<MethodInfo> _builtInParsers;
        List<TryParseMatrixDelegate<TRowKey, TColKey, TValue>> _registeredParsers = new List<TryParseMatrixDelegate<TRowKey, TColKey, TValue>>();
#if !SILVERLIGHT

        private IEnumerable<Assembly> _allReferencedAssemblies;
        private IEnumerable<Assembly> AllReferencedAssemblies
        {
            get
            {
                if (_allReferencedAssemblies == null)
                {
                    _allReferencedAssemblies = EnumerateAssemblyAndAllReferencedUserAssemblies(FileUtils.GetEntryOrCallingAssembly(), EnumerateAllUserAssemblyCodeBases().ToHashSet()).ToHashSet();
                }
                return _allReferencedAssemblies;
            }
        }

        private IEnumerable<Type> _allMatrixTypes;
        private IEnumerable<Type> AllMatrixTypes
        {
            get
            {
                if (_allMatrixTypes == null)
                {
                    _allMatrixTypes = EnumerateMatrixTypes();
                }
                return _allMatrixTypes;
            }
        }
#endif


        //!!!not threadsafe, adds unwanted state
        /// <summary>
        /// The last error messges
        /// </summary>
        public string ErrorMessages { get; private set; }


        /// <summary>
        /// A parameterless constructor for the MatrixFactor.
        /// </summary>
        public MatrixFactory()
        {
            _builtInParsers = KnownMatrixParsers();
        }


        /// <summary>
        /// Initializes a new instance of a Matrix parse factory.
        /// </summary>
        /// <returns>A MatrixFactory that can be used to create a Matrix instance from a file.</returns>
        public static MatrixFactory<TRowKey, TColKey, TValue> GetInstance()
        {
            return new MatrixFactory<TRowKey, TColKey, TValue>();
        }

        /// <summary>
        /// Adds the specificed parsing function to the MatrixFactory
        /// </summary>
        /// <param name="tryParseMatrixDelegate">The function to add.</param>
        public void RegisterMatrixParser(TryParseMatrixDelegate<TRowKey, TColKey, TValue> tryParseMatrixDelegate)
        {
            _registeredParsers.Add(tryParseMatrixDelegate);
        }


        /// <summary>
        /// Create a Matrix by parsing the file. The MatrixFactory may try many different parsers to get a result.
        /// </summary>
        /// <param name="filename">The name of the file containing the matrix information.</param>
        /// <param name="missingValue">The special value that represents missing in the created matrix.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <returns>the Matrix created</returns>
        public Matrix<TRowKey, TColKey, TValue> Parse(string filename, TValue missingValue, ParallelOptions parallelOptions)
        {
            Matrix<TRowKey, TColKey, TValue> result;
            if (TryParse(filename, missingValue, parallelOptions, out result))
            {
                return result;
            }
            string firstline = FileUtils.ReadLine(filename);
            throw new ArgumentException(string.Format("Unable to find suitable Matrix Parser for file \"{0}\" with header \"{1}\"\nErrors:\n{2}",
                filename,
                firstline.Length > 25 ? firstline.Substring(0, 25) + "..." : firstline,
                ErrorMessages));
        }

        /// <summary>
        /// Create a Matrix by parsing the file. The MatrixFactory may try many different parsers to get a result.
        /// </summary>
        /// <param name="filename">The name of the file containing the matrix information.</param>
        /// <param name="missingValue">The special value that represents missing in the created matrix.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the multithreaded behavior of this operation.</param>
        /// <param name="result">The matrix created.</param>
        /// <returns>true, if some parse succeeds; otherwise, false</returns>
        public bool TryParse(string filename, TValue missingValue, ParallelOptions parallelOptions, out Matrix<TRowKey, TColKey, TValue> result)
        {
            result = null;
            TextWriter origErrorWriter = Console.Error;
            MFErrorWriter mfErrorWriter = new MFErrorWriter();
            Console.SetError(mfErrorWriter);

            if (_registeredParsers.Count == 0 && _builtInParsers.Count == 0) Console.Error.WriteLine("No matching parsers found");

            foreach (TryParseMatrixDelegate<TRowKey, TColKey, TValue> ParseFunc in _registeredParsers)
            {
                mfErrorWriter.SetLabel(ParseFunc.Method.DeclaringType + "." + ParseFunc.Method.Name);
                if (ParseFunc(filename, missingValue, parallelOptions, out result))
                {
                    ErrorMessages = mfErrorWriter.ToString();
                    Console.SetError(origErrorWriter);
                    return true;
                }
            }
            object[] methodParams4 = new object[] { filename, missingValue, parallelOptions, result };
            object[] methodParams3 = new object[] { filename, missingValue, result };
            foreach (MethodInfo ParseFuncMemberInfo in _builtInParsers)
            {
                mfErrorWriter.SetLabel(ParseFuncMemberInfo.DeclaringType.Name + "." + ParseFuncMemberInfo.Name);
                int argCount = ParseFuncMemberInfo.GetParameters().Length;
                Helper.CheckCondition(argCount == 3 || argCount == 4, () => string.Format(CultureInfo.InvariantCulture, Properties.Resource.ExpectedArgCountOfThreeOrFour));
                object[] methodParams = (3 == argCount) ? methodParams3 : methodParams4;
                bool success = (bool)ParseFuncMemberInfo.Invoke(null, methodParams);
                if (success)
                {
                    result = (Matrix<TRowKey, TColKey, TValue>)methodParams[argCount - 1];//!!!const
                    ErrorMessages = mfErrorWriter.ToString();
                    Console.SetError(origErrorWriter);
                    return true;
                }
            }
            ErrorMessages = mfErrorWriter.ToString();
            Console.SetError(origErrorWriter);
            return false;
        }

        private List<MethodInfo> KnownMatrixParsers()
        {
            List<MethodInfo> readers = new List<MethodInfo>();

            Type[] argTypesPO = new Type[] { typeof(string), typeof(TValue), typeof(ParallelOptions), typeof(Matrix<TRowKey, TColKey, TValue>).MakeByRefType() };
            Type[] argTypesST = new Type[] { typeof(string), typeof(TValue), typeof(Matrix<TRowKey, TColKey, TValue>).MakeByRefType() };
            Type[] genericTypes3 = new Type[] { typeof(TRowKey), typeof(TColKey), typeof(TValue) };
            Type[] genericTypes1 = new Type[] { typeof(TValue) };

            foreach (Tuple<string, string> classAndFunction in _defaultParserNames)
            {
                Type classType;
                if (TryGetType(classAndFunction.Item1, out classType))
                {

                    MethodInfo matrixParserPO = classType.GetMethod(classAndFunction.Item2, argTypesPO);
                    MethodInfo matrixParserST = classType.GetMethod(classAndFunction.Item2, argTypesST);

                    //Give priority to the parallel parser
                    if (matrixParserPO != null)
                    {
                        readers.Add(matrixParserPO);
                    }
                    else if (matrixParserST != null)
                    {
                        readers.Add(matrixParserST);
                    }
                    else
                    {
                        foreach (MethodInfo potentialParser in classType.GetMethods(BindingFlags.Static | BindingFlags.Public))
                        {
                            if (potentialParser.Name.Equals(classAndFunction.Item2, StringComparison.CurrentCultureIgnoreCase) && potentialParser.IsGenericMethodDefinition)
                            {
                                Type[] typeParams = potentialParser.GetGenericArguments();
                                MethodInfo genericMethod = null;
                                if (typeParams.Length == 3 && typeParams[0].GetGenericParameterConstraints().Length == 0)
                                {
                                    genericMethod = potentialParser.MakeGenericMethod(genericTypes3);
                                }
                                else if (typeParams.Length == 1 && typeParams[0].GetGenericParameterConstraints().Length == 0)
                                {
                                    genericMethod = potentialParser.MakeGenericMethod(genericTypes1);
                                }
                                else
                                {
                                    continue;
                                }

                                ParameterInfo[] paramInfo = genericMethod.GetParameters();
                                if (paramInfo.Length == argTypesPO.Length)
                                {
                                    bool parametersMatch = true;
                                    foreach (ParameterInfo pi in paramInfo)
                                    {
                                        parametersMatch &= pi.ParameterType.Equals(argTypesPO[pi.Position]);
                                    }
                                    if (parametersMatch)
                                        readers.Add(genericMethod);
                                }
                                //!!!code is very like that above
                                else if (paramInfo.Length == argTypesST.Length)
                                {
                                    bool parametersMatch = true;
                                    foreach (ParameterInfo pi in paramInfo)
                                    {
                                        parametersMatch &= pi.ParameterType.Equals(argTypesST[pi.Position]);
                                    }
                                    if (parametersMatch)
                                        readers.Add(genericMethod);
                                }

                            }
                        }
                    }
                }
            }
            return readers;
        }

        private bool TryGetType(string typeName, out Type returnType)
        {
            string genericsTypeName = typeName + "`3";  // The generic versions will have 3 type parameters, so the type names will all end with `3

#if !SILVERLIGHT
            foreach (Assembly assembly in AllReferencedAssemblies)
            {
                returnType = assembly.GetType(typeName, false, true);
                if (returnType != null)
                    return true;

                returnType = assembly.GetType(genericsTypeName, false, true);
                if (returnType != null)
                {
                    Type[] typeParams = returnType.GetGenericArguments();
                    if (typeParams.Length == 3 && typeParams[0].GetGenericParameterConstraints().Length == 0)
                    {
                        returnType = returnType.MakeGenericType(new Type[] { typeof(TRowKey), typeof(TColKey), typeof(TValue) });
                        return true;
                    }
                }
            }
#endif

            returnType = null;
            return false;
        }

#if !SILVERLIGHT
        private static IEnumerable<string> EnumerateAllUserAssemblyCodeBases()
        {
            Assembly entryAssembly = FileUtils.GetEntryOrCallingAssembly();
            yield return entryAssembly.GetName(false).Name;

            string exePath = Path.GetDirectoryName(entryAssembly.Location);
            Assembly assembly;
            foreach (string dllName in Directory.GetFiles(exePath, "*.dll"))
            {
                assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(dllName);
                }
                catch
                {
                }
                if (assembly != null)
                {
                    yield return assembly.GetName(false).Name;
                }
            }
            foreach (string dllName in Directory.GetFiles(exePath, "*.exe"))
            {
                assembly = null;
                try
                {
                    assembly = Assembly.LoadFile(dllName);
                }
                catch { }
                if (assembly != null)
                {
                    yield return assembly.GetName(false).Name;
                }
            }
        }

        private static IEnumerable<Assembly> EnumerateAssemblyAndAllReferencedUserAssemblies(Assembly assembly, HashSet<string> userAssemblies)
        {
            if (userAssemblies.Contains(assembly.GetName(false).Name))
                yield return assembly;

            foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
            {
                if (userAssemblies.Contains(assemblyName.Name))
                {
                    Assembly referencedAssembly = null;
                    try
                    {
                        referencedAssembly = Assembly.Load(assemblyName);
                    }
                    catch { }
                    if (referencedAssembly != null)
                    {
                        foreach (Assembly refAssem in EnumerateAssemblyAndAllReferencedUserAssemblies(referencedAssembly, userAssemblies))
                            yield return refAssem;
                    }
                    //Assembly referencedAssembly = Assembly.Load(assemblyName);

                    //foreach (Assembly refAssem in EnumerateAssemblyAndAllReferencedUserAssemblies(referencedAssembly, userAssemblies))
                    //    yield return refAssem;
                }
            }

            //List<AssemblyName> assemblyNames = SpecialFunctions.GetEntryOrCallingAssembly().GetReferencedAssemblies().ToList();
            //assemblyNames.Add(SpecialFunctions.GetEntryOrCallingAssembly().GetName());

            //foreach (AssemblyName assemblyName in assemblyNames)
            //{
            //    Assembly assembly = Assembly.Load(assemblyName);
            //    yield return assembly;
            //}
        }


        private static IEnumerable<Type> EnumerateMatrixTypes()
        {
            Type mType = typeof(Matrix<TRowKey, TColKey, TValue>);
            Assembly assembly = Assembly.GetAssembly(mType);
            foreach (Type rawType in assembly.GetTypes())
            {
                Type emittedType = rawType;
                if (rawType.IsGenericTypeDefinition)
                {
                    Type[] typeParams = rawType.GetGenericArguments();
                    if (typeParams.Length == 3 && typeParams[0].GetGenericParameterConstraints().Length == 0)
                    {
                        emittedType = rawType.MakeGenericType(new Type[] { typeof(TRowKey), typeof(TColKey), typeof(TValue) });
                    }
                }
                if (emittedType.IsSubclassOf(mType))
                {
                    yield return emittedType;
                }
            }
        }
#endif

        internal class MFErrorWriter : StringWriter
        {
            private string _label = "";

            public void SetLabel(string label)
            {
                _label = label + ": ";
            }


            public override void Write(char[] buffer, int index, int count)
            {
                base.Write(_label);
                base.Write(buffer, index, count);
            }


        }

    }
}
