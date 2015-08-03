# .NET Bio

.NET Bio is an open source library of common bioinformatics functions, intended to simplify the creation of life science applications.

The core library implements a range of file parsers and formatters for common file types, connectors to commonly-used web services such as NCBI BLAST, and standard algorithms for the comparison and assembly of DNA, RNA and protein sequences. Sample tools and code snippets are also included.


Build Status
------------

|Linux   |Windows |Mac OS X |
|:------:|:------:|:-------:|
|[![Build Status](https://travis-ci.org/dotnetbio/bio.svg?branch=master)](https://travis-ci.org/dotnetbio/bio) | [![Build status](https://ci.appveyor.com/api/projects/status/ihru18bvx5d5yofm/branch/master?svg=true)](https://ci.appveyor.com/project/nigel-delaney/bio/branch/master) | [![Build Status](https://travis-ci.org/dotnetbio/bio.svg?branch=master)](https://travis-ci.org/dotnetbio/bio)|



## Using .NET Bio in your application
.NET Bio binaries are distributed using [Nuget](wwww.nuget.org):

- [.NET Bio Core](https://www.nuget.org/packages/NETBioCore.PCL/)
- [.NET Bio Algorithms](https://www.nuget.org/packages/NetBioAlgorithms.PCL/)
- [.NET Bio Web Services](https://www.nuget.org/packages/NetBioWeb.PCL/)

## Building .NET Bio from source

There are several solution files (.sln) you can use to build .NET Bio on Windows, Mac or Linux. These are in the `src` folder under the main repository.

- `Bio.Mono.sln` builds the .NET/Mono desktop assemblies necessary for Windows, Linux or Mac OSX. This is the easiest version to build and the one we recommend you start with. It can be built with Visual Studio, Xamarin Studio, or MonoDevelop.
- `Bio.sln` builds the full set of binaries and Nuget packages and can only be compiled on Windows with Visual Studio 2013 or later (we recommend Visual Studio 2015).
- `BioTools.sln` builds some optional command line tools which showcase some of the framework classes for .NET Bio.

## Project Goals
.NET Bio has been built with specific goals in mind:

**Extensibility:** .NET Bio is designed to be easy for a programmer to extend with new functions, please refer to the developer documentation available on this site. Developers who extend .NET Bio are encouraged to contribute their code back to the project so that the community as a whole can benefit from their work.

**Flexibility:** Whatever .NET-supported language you choose, the code you write will work with .NET Bio —so the accessibility of Visual Basic®, the power of C#, the speed and conciseness of functional languages such as F# or the ad-hoc scripting capabilities of Python are all available, as are many others. As a library of common code, .NET Bio can be used to build whatever application type meets your needs, whether integrating with applications such as Microsoft Excel, building commandline or GUI applications from scratch, or creating cloud services or workflow components.

**Community:** .NET Bio is a community-owned open source project and welcomes participation and contributions from programmers with an interest in the life sciences. We provide forums for discussions and help, documentation and sample applications, and tools to report bugs and request new features.

## History of the project
The original home for the project was [bio.codeplex.com](http://bio.codeplex.com) - we decided not to carry over the history prior to version 2.0 of the project, but you can still go to the older (deprecated) site and get the original source code if necessary.

## Additional Information

- [FAQ](https://github.com/dotnetbio/bio/blob/master/FAQ.md)
- [Testing](https://github.com/dotnetbio/bio/blob/master/TESTING.md)
- [Class Library Documentation](http://dotnetbio.github.io/Help/index.html)


