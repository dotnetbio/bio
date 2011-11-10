This is the root directory for .Net Bio.  Everything necessary to edit, build, unit test, and
debug a program that is part of .Net Bio has a specified location within
this directory structure.

The directories and their functions are:

  Build -
      all the build output ends up under the Build directory. The 
      goal is to minimize or eliminate temporary or intermediate 
      files from cluttering up the project directories under source 
      control.

  BuildTools - 
      all the tools and scripts that produce the various .Net Bio
      distribution files are located under the BuildTools directory.
      The goal is to have the tools in one place to minimize search
      time and to simplify the learning of the build process itself.

  Doc - 
      all documentation files for the project are located under the 
      Doc directory

  Source - 
      all source files that contribute to the released 
      binaries and the sample applications are located in their respective   
      directories under the Source directory.

	  Source\Framework
			all program source files that contribute to the released 
			binaries are located in their respective directories  
			under the Framework directory

	  Source\Tools - 
	        all sample code for how to use API provided by parts of the 
			.Net Bio project are located under this directory.

  Tests - 
      Unit Tests that drive the different .Net Bio executables and 
      validate their behavior are located under this directory.

----------
In addition to the code in this tree, there are the following
External Dependencies that must be installed and met:

  Visual Studio 2010 - 
      any version (Express through Ultimate) is required to develop 
      .Net Bio applications.  You can see a comparison of all varities at this site
	  http://www.microsoft.com/visualstudio/en-us/products
	  
	  The VisualStudio 2010 Express edition can 
      be downloaded from http://www.microsoft.com/express/downloads/
                             
  .NET Runtime v4.0 - 
      .NET Bio runs on top of the .NET runtime V4.  The installer can be 
      downloaded from
        http://msdn.microsoft.com/en-us/netframework/aa569263.aspx

----------

KNOWN ISSUES

Large binary alignment files are only supported on 64 bit machines – 
	32 bit machines will throw an out of memory exception.
	