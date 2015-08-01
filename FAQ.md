**What are the prerequisites I need on my machine to build code in this project?**

On Mac or Linux
- Xamarin Studio with  [mono] (http://www.mono-project.com/) installed.

On Windows

- Microsoft Windows® (Windows 7 or better) x86 or x64 (preferred) versions 
- Visual Studio 2015 ([Community edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) is fine)
- .NET Framework 4.5, which is included with Visual Studio 2015

_Optional Components (required to build some of the additional tools)_

- [Microsoft Silverlight SDK](http://www.microsoft.com/download/en/details.aspx?displaylang=en&id=18149)
- [Microsoft HPC Pack 2008 R2 Client Utilities Redistributable Package with Service Pack 1](http://www.microsoft.com/downloads/en/details.aspx?FamilyID=0a7ba619-fe0e-4e71-82c8-ab4f19c149ad)
- [Microsoft HPC Pack 2008 R2 SDK](http://www.microsoft.com/downloads/en/details.aspx?FamilyID=BC671B22-F158-4A5F-828B-7A374B881172)

**What platforms are supported?**

.NET Bio 2.0 is entirely cross platform, and can be run on Linux, Mac, Windows and a variety of smartphones and tablets. In particular, it supports the following platforms:

- .NET 4.5 or better
- Mono 4.5 or better
- Windows 8 or 8.1 WinRT (w/ .NET 4.5)
- Windows Phone 8.1 WinPRT (w / .NET 4.5)
- Xamarin.iOS
- Xamarin.Android

We currently do not support Silverlight, although it is possible it could be added in the future. You can use Visual Studio 2013 or 2015 with .NET Bio, MonoDevelop on Linux or Mac OSX, or Xamarin Studio on Windows or OS X.

**I found a bug - how do I report it?**

- Click the [Issues](https://github.com/dotnetbio/bio/issues) link.
- Click the "New Issue" button.
- Choose a descriptive title.
- List the component you found the issue in.
- List all the steps to reproduce the problem and include files or images if necessary.
- Describe the impact to you - like blocks all my development or minor usability issue.

**How do I get a copy of Microsoft Visual Studio?**

Visual Studio is an Integrated Development Environment (IDE) on Windows, which presents a programmer with all the tools they need to write a program. There is a [free community edition](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) available which can be used to compile the source and tools for .NET Bio.


**I would prefer not to use Microsoft Visual Studio as my IDE. What alternatives are available?**

- You can use [MonoDevelop](http://monodevelop.com/Download) which runs under Linux and Mac OSX. It is compatible with Visual studio solution files, and is built more for C#/F#.
- You may also want to check the Visual Studio site for things like [VsVIM](http://visualstudiogallery.msdn.microsoft.com/59ca71b3-a4a3-46ca-8fe1-0e90e3f79329) - Vim Emulation layer which integrates the familiar key binding experience of Vim directly into Visual Studio's editor. 
- The [Mono Project](http://www.mono-project.com/) provides a plug-in for the Eclipse IDE, enabling .NET development on a number of platforms. Please visit the Mono pages for specifics.
- Other Eclipse plug-ins are available, for instance [Emonic](http://emonic.sourceforge.net/).

**I am using the BioExcel add-in with Microsoft Excel 2010 and want to use the Chart option under the .NET Bio tab to chart my DNA sequence distribution table. What steps do I need to do?**

- Load the gene sequence you want to chart
- Under the Excel File menu choose the Options submenu
- Select Customize ribbon
- From the right side list of displayed choices enable/check the developer tab
- The Developer tab will now display in Excel, choose macro security and follow the directions for enabling macros in the BioExcel user guide including adjusting the security settings
- After the macro security settings are set, choose the Macros option under the Developer tab, give your new macro a name and choose create
- Right click on VBAproject and choose import file
- Import `DisplayDNASequenceDistribution.bas` - this could be located under `c:\program files (x86)\.NET Bio\1.01\Tools\.NET Bio Extension for Excel\`
- Double click Module 11
- Do a Save As, Excel macro enabled workbook - .xlsm
- Close VB for applcations and return to Excel
- The Chart display macro is now enabled and functionable in your revised sheet

**I am a new developer to the project and I want to build the project**

- First download the source code you want to start with (under the Source Code tab)
- Start Visual studio, then select File->Open -> Project/Solution 
- Select `Bio.sln` or `Bio.Mono.sln` (for Linux/Mac OSX) in the open project dialog box.
- Mouse or tab to the solution Explorer Window, right click on the solution and then choose build solution

**What alignment algorithms are part of the .Net Bio library and therefore availble in the Biology Extension for Excel add-in?**

- Smith-Waterman
- Needleman-Wunsch
- Pairwise-Overlap
- MUMmer 
- NUCmer 

**I have a suggestion for a new feature or some new ideas for the project. How do I make a suggestion?**

- Use the [Issues](https://github.com/dotnetbio/bio/issues) link.

**Is .NET Bio compatible with [Mono](http://www.mono-project.com/Main_Page)?**

Yes, .NET Bio builds and runs under Mono for Linux and Mac OS X.

**Why do I get the error message from PadenaUtil when doing an assembly “character not supported”?**

- Gene sequence reads containing any characters other than ACGT are not compatible with the current version of the Padena algorithm. There are two approaches to dealing with this:

	1. Filter the reads using the FilterReadsUtil utility, excluding those with ambiguous characters.

	2. Write a script or program to split reads containing ambiguity characters (like N), so for example the string AAAAAAAAAANGGGGGGGGGG would become two ‘subreads’, AAAAAAAAAA and GGGGGGGGGG. Note that any ‘subread’ generated in this manner will need to be longer than the kmer size used in a subsequent sequence assembly step, or the subread will be ignored. This approach makes more of the data usable in assembly, at the cost of breaking up some of the reads. 
	
	Ambiguous characters are typically found at the beginning and ends of reads, usually associated with poor base qualities. If this is the case in your data (and you are using a data format that records quality information such as FASTQ), you can trim the start and ends of your reads using one of several applications. A .NET Bio application for this purpose will be available shortly.

**What's the recommended [K-mer](http://en.wikipedia.org/wiki/K-mer) length when using Padenutil?**
- Short answer: 19
- K-mers can be any odd value from 3 to 31 in PadenaUtil
- If a short K-mer is used, then any error in the data will be represented in fewer K-mers, so shorter values are better for noisy data
- If a longer K-mer length is used, there is a higher chance of K-mers being unique, so less ambiguity in assemblies
- Since each dataset is different, the best approach is to evaluate the quality of an assembly using different K-mer lengths, to see what works best. If a run is likely to take a long time, do this on a subset of the data.
- Failing that, choose k=32. Assembly will be slower with longer K-mers, but for the reason given above it is likely to produce more reliable contigs

**The import menu doesn't appear when I use the Excel Add-in for .NET Bio**

This is an Excel bug which occasionally happens when you open an existing spreadsheet. Follow these steps once and it should not happen again:

- Open Excel by double-clicking on your spreadsheet
- Click on the .NET Bio tab to display the .NET Bio ribbon
- Select File -> Options -> Customize ribbon
- Find the .NET Bio entry in the list of Main Tabs on the right, and click to expand it
- Select the Sequence Data entry
- Use the arrow buttons to the right of the list to move it down one place
- Close the dialog

The Import options should reappear on the ribbon, but to the right of the Aligners. If you would like to move it back to the left, reopen the dialog and select Sequence data, then move it one place back up the list.

