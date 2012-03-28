.NET Bio Sequence Assembler: Readme.txt
Version 1.01, March 2012

The .NET Bio Sequence Assembler is a proof-of-concept-application that demonstrates how the .NET Bio Framework can be used to create applications for bioinformatics research. It uses rich UI elements to enable the manipulation and visualization of genomic data. 

The .NET Bio Sequence Assembler implements several features of the .NET Bio Framework: a set of parsers for common genome file formats; a set of algorithms for alignment and/or assembly of DNA, RNA or Protein strands; and a set of connectors to several Basic Local Alignment Search Tool (BLAST) Web services for genomic identification. Reports from BLAST services can be viewed as single-line reports, or in the SilverMap visualization component integrated by the Queensland University of Technology.

The .NET Bio Sequence Assembler is available at http://bio.codeplex.com.  It is licensed under the OSI approved Apache 2.0 License, which can be found here:  http://bio.codeplex.com/license.

The SilverMap visualization component can be found at http://qutbio.codeplex.com/.  It is  licensed under the OSI approved MS-PL, which can be found here:  http://qutbio.codeplex.com/license.

KNOWN ISSUES
============
- "Canceling" an algorithm while in progress will give the impression that the processing has stopped, but in fact the algorithm will continue to process and consume CPU/memory resources until completed. The application will act as if it is cancelled by ignoring any response (success or failure), but performance may be impacted until the process is completed.

- We currently have a limit of 1000 sequences to be read in the .NET Bio Sequence Assembler.

- Opening a file with a very large number of sequences, or a file with a very large amount of non-sequence (textual) information, may cause the .NET Bio Sequence Assembler to hang. 

- If attempting to cancel the load of a very large file, the application may appear to hang while it finishes loading the file before it can cancel the load operation.  This is a known threading issue.

- When using NUCMer to perform sequence alignment, un-aligned sequences will not be reported the application's tree view.

- When using the PaDeNa Assembler, un-aligned regions of sequences in contigs will be displayed in lower-case, while the aligned regions will remain in upper case.
