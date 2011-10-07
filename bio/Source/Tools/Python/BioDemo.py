import clr
import sys
import time
import os
from os import path

# Adding the dll reference will throw an exception if we're debugging in VS from the Python
# development dir, instead of the standard non-dev method of running from the bin\Debug dir or an
# installation dir.
try:
    clr.AddReferenceToFile("Bio.IronPython.dll")
except:
    default_filename = "bin\\Debug\\Small_Size.gbk"
else:
    default_filename = "Small_Size.gbk"

from BioIronPython.Algorithms import *
from BioIronPython.IO import *
from BioIronPython.Util import *
from BioIronPython.Web import *

build_dir = "bin\\Debug"

def deploy_file(filename):
    "Copies a file to the bin\Debug folder, replacing any file of the same name already there."
    new_filename = build_dir + "\\" + filename[filename.rfind("\\") + 1 :]
    try:
        if File.Exists(new_filename):
            File.Delete(new_filename)
    except:
        # don't worry about replacing read-only files that we can't delete
        pass
    else:
        File.Copy(filename, new_filename)

try:
    # make build dir if needed
    if not path.exists(build_dir):
        os.mkdir(build_dir)
    
    # copy test file
    deploy_file("Data\\Small_Size.gbk")
except:
    print "An error occurred: " + `sys.exc_info()` + "\n"
    raw_input("Press enter to exit: ")

again = "y"

print "Welcome to the Bio IronPython Demo!"

while "yY".find(again[0]) != -1:
    try:
        # parse file
        filename = raw_input("\nPlease enter a sequence filename (defaults to " + default_filename + "): ")
        if filename == "":
            filename = default_filename
        seq = open_seq(filename)[0]

        print "\nSuccessfully loaded sequence!"
        print "    ID     = " + seq.ID
        print "    Length = " + `seq.Count` + "\n"

        if seq.Count >= 500:
            # create fragments
            fragments = split_sequence(seq.Range(0, 500), 10, 50)

            print "A subsequence consisting of the first 500 nucleotides or amino acids has been split into",
            print `len(fragments)` + " fragments, each of length 50."
            print "These will now be reassembled!  (This may take a minute.)\n"

            # assemble sequence and sort contigs by descending length
            assembly = assemble_pairwise(fragments)
            contig_list = sorted(assembly.Contigs, lambda c1, c2: c2.Length - c1.Length)

            print "The fragments have been assembled into " + `len(contig_list)` + " contigs, with",
            print `len(assembly.UnmergedSequences)` + " unmerged fragments."
            print "The longest contig has a length of " + `contig_list[0].Length` + "."
            print "Let's do a BLAST search with it.  (This may also take a minute.)\n"

            # run BLAST search
            job_id = submit_blast_search(contig_list[0].Consensus)

            # wait for response
            for i in range(1, 13):
                time.sleep(5)
                result_string = poll_blast_results(job_id)
                if result_string != None:
                    result_list = parse_blast_results(result_string)
                    if result_list != None:
                        print "\nThe following results were returned:\n"
                        print "ID".ljust(40), "Accession".ljust(20), "Length".rjust(10)
                        print "--------------------------------------------------------------------------"
                        for result in result_list:
                            for record in result.Records:
                                for hit in record.Hits:
                                    print hit.Id.ljust(40), hit.Accession.ljust(20), `hit.Length`.rjust(10)
                        print
                        break
                elif i % 2 == 0:
                    print "No response yet after " + `5*i` + " seconds..."
            else:
                print "\nNo results have been returned from the BLAST search."
                print "Giving up on job ID " + `job_id` + "\n"
        else:
            print "Input sequence must have atleast 500 basepairs."

    except:
        print "An error occurred: " + `sys.exc_info()` + "\n"

    # prompt to go again
    again = " "
    while "yYnN".find(again[0]) == -1:
        again = raw_input("Would you like to enter another sequence? (y/n): ")
        if len(again) == 0:
            again = " "