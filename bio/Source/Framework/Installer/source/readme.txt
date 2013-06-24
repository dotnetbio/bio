.NET Bio 1.1 Release Notes
June 2013

Major Changes included in this release
==================================================
- The Needleman-Wunsch and Smith-Waterman reference algorithms have been changed to provide more consistent output with other variations of these algorithms.
   - the older algorithms are still available under the Bio.Algorithms.Legacy namespace

- Several significant changes to Padena are included as part of this release 
   1) The minimum kmer length is now 13
   2) KMERS are forced to be odd to avoid palindromes

- Odd kmers are only enforced when Padena is auto-calculating it. Otherwise, we'll use whatever is being deliberately set. The lower-bound will now check to ensure we don't have kmers larger than the smallest sequence.

- With this release there is inclusion of EDNAFull similarity matrix that drives a cost function for any of the genomic relatedness comparisons. The integration should allow Needleman*,  Smith*, or any of the dynamic programming algorithms to use another cost function to do their work.  
- Moved Padena, Pamsam, HPC unit tests into separate libraries
- Converted to Visual Studio 2012 unit test formats.
- Created new solutions to build Silverlight, .NET Bio Core and Web projects
- New parsers are available which support .zip based files for FASTA and FASTQ parsing.

Issues and Tasks Addressed as part of this release
==================================================
8598 - Needleman-Wunsch reference algorithm
8599 - Smith-Waterman algorithm
8102 - PADENA performance impacted by new storage class
8663 - Source Code Project restructuring
8621 - Converting SAM files with unmapped segments to BAM throws an error
7889 - NUCmer algorithm does not directly support reverse matches
8196 - Command Prompt shortcut target needs suffix of /F:ON
8663 - Source Code Project Restructuring (see issue for details)
8673 - Newick Parser unable to parse file
8573 - .MSI name should be changed

KNOWN ISSUES
============
- You can see any issues and report ones you may encounter them via the Issue Tracker tab on http://bio.codeplex.com
	To see issues under the Issue Tracker tab choose "advanced view" and then use the component filter to select "BioExcel".
	To report a new issue again under the issue tracker tab select "create new item".