.NET Bio Extension for Excel: Readme.txt
Version 1.0 , June 2011

This document describes how to use the .NET Bio Extension for Excel, a Microsoft® Office Excel 2007/2010 add-in that provides a simple and flexible way to work with genomic sequences, metadata, and interval data in an Excel document.

The .NET Bio Extension implements several features of the .NET Bio framework: a set of parsers for common genome file formats; a set of sequencing algorithms for assembly of a consensus DNA strand; a set of connectors to several Basic Local Alignment Search Tool (BLAST) Web services for genome identification; and genomic interval functions that allow the manipulation of BED files inside Excel. 

The .NET Bio Extension can be programmatically extended to use other features in the .NET Bio Framework as well. The .NET Bio Extension is available at http://bio.codeplex.com, and is licensed under the Apache terms which can be found here:  http://bio.codeplex.com/license.

KNOWN ISSUES
============

- Make sure to enable add-ins in Excel to ensure the BioExcel ribbon “.NET Bio” appears. See this link for details on how to enable add-ins in Excel:  http://office.microsoft.com/en-us/excel/HA100341271033.aspx.

- “Canceling” an algorithm while in progress will give the impression that the processing has stopped, but in fact the algorithm will continue to process and consume CPU/memory resources until completed. The application will act as if it is cancelled by ignoring any response (success or failure), but performance may be impacted until the process is completed.