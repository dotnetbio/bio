This directory contains the test data for all BVT, P1 and P2 test cases with Configuration files and Similarity matrices for the same.

The test data include the FastA, GenBank and GFF Files which contains small, medium and large test data with DNA, Protein and RNA sequences. 
These test data were created manually after doing white-board validations on the alignment algorithms with their expected output in the configuration files.

The configuration files details are as below:
* TestsConfig.xml - This configuration file has the config information of all the test cases (FastA, GenBank, Alignment Algorithms, Blast Webservice and object model) except the MUMmer and GFF test cases.
* MUMmerTestsConfig.xml - This configuration file has the config information of all the MUMmer BVT and P1 test cases.
* GFFTestsConfig.xml - This configuration file has the config information of all the GFF BVT and P1 test cases.

This directory also contains the data for the BLOSUM series of similarity matrices.

BLOSUM matrices reference:
Available at http://www.ncbi.nlm.nih.gov/IEB/ToolBox/C_DOC/lxr/source/data/BLOSUM50.
Available at ftp://ftp.ncbi.nih.gov/blast/matrices/IDENTITY.

BLOSUM matrices have been modified from their original content to align with the formatting required to make the Bio code be able to parse them properly. (e.g. remove column nos) 

Blast record files are used for parsing the blast results.