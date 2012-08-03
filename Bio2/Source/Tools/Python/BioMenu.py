#////*********************************************************
#// <summary>Adding the dll reference will throw an exception if we're debugging in VS from the Python
#// development dir, instead of the standard non-dev method of running from the bin\Debug dir or an
#// installation dir.</summary>
#////*********************************************************

import clr
import sys
import time

# Adding the dll reference will throw an exception if we're debugging in VS from the Python
# development dir, instead of the standard non-dev method of running from the bin\Debug dir or an
# installation dir.
try:
    clr.AddReferenceToFile("Bio.IronPython.dll")
except:
    pass

from BioIronPython.Algorithms import *
from BioIronPython.IO import *
from BioIronPython.Util import *
from BioIronPython.Web import *
from BioIronPython.SequenceManipulationApplications import *
from BioIronPython.ListOr import *
from BioIronPython.BioDemo import *

again = "y"
while "yY".find(again[0]) != -1:

   option = ""
   
   # Ensuring that the user chooses a number between 1 and 6
   while(option < "1" or option > "6"):
        print "--------------------------------------------------------"
        print "\nPlease choose the application that you want to run:"
        print "\n1-> Bio integrated demo"
        print "\n2-> Concatenate sequences"
        print "\n3-> Strip non-alphabetic characters in a sequence"
        print "\n4-> Remove Poly-A tail from a sequence"
        print "\n5-> Perform logical union of two sequence files"
        print "\n6-> Find differences between two sequences\n"
        print "--------------------------------------------------------"
        
        option = raw_input("\n\nPlease enter the application number (1-6):")
        
   if option == "1":
      BioDemo() 
   elif option == "2":
      ConcatenateSequences() 
   elif option == "3":
      StripNonAlphabets()
   elif option == "4":
      RemovePolyATail()
   elif option == "5":
      ListOr()
   elif option == "6":
      DiffSeq()
      
   again = " "
   option = ""
   while "yYnN".find(again[0]) == -1:
        again = raw_input("Would you like to choose applications again? (y/n): ")
        if len(again) == 0:
            again = " "
            
