.NET Bio: Readme.txt
Version 1.0, June 2011

.NET Bio is an open source, reusable .NET library and application programming interface (API) for bioinformatics research.

.NET Bio is available at http://bio.codeplex.com.  It is licensed under the OSI approved Apache 2.0 License , found here:  http://bio.codeplex.com/license.


KNOWN ISSUES
============
- Current issues are listed: http://bio.codeplex.com/workitem/list/basic
- We currently have a single sequence limit of 2GB in length, this is not a data size limit for the assembly process. 
	So any single RNA, DNA, or protein sequence in .NET Bio is limited to 2 billion characters. The largest human chromosome 
	is 250MB so the single sequence limit should NOT be a limiting factor in your development. If you experiences issues related 
	to that limit please post on the issue tracker tab on the Codeplex site.