# The .NET Bio testing framework.

.NET Bio uses [NUnit 3.0](http://www.nunit.org/), patches should come with tests and all tests should pass before a commit or branch is merged.


### Quick start Windows command line

 - Download the [NUnit binaries](http://github.com/nunit/nunitv2/releases/download/2.6.4/NUnit-2.6.4.zip), unzip and add to your path
 - Build Bio.Tests.Core project (usually in Visual Studio)
 - `nunit-console Source\Tests\Bio.Tests\bin\Release\Bio.Tests.dll`
 
 
### Quick start Mono command line

 - Download the [NUnit binaries](http://github.com/nunit/nunitv2/releases/download/2.6.4/NUnit-2.6.4.zip), unzip and place somewhere accessible
 - Build Bio.Tests.Core project (usually in Xamarin Studio)
 - `export MONO_IOMAP=all`
 - `mono somewhere/nunit-console.exe Source/Tests/Bio.Tests/bin/Release/Bio.Tests.dll`
 
### Running Unit Tests in the IDE

NUnit 3.0 is not incredibly robust in the IDEs at present, so results are mixed but here are some suggestions.  

In **Visual Studio**, NUnit has an integrated Visual Studio add-in which can be downloaded from their website and used to run tests in the Test Explorer pane.

In **Xamarin Studio**, the main problem is that the environmental variable MONO_IOMAP must be set to "all" in order to handle some historical path naming conventions to subdirectories (e.g. TestUtils\Bed\Data.txt) when .NET Bio was not fully cross-platform. In the Xamarin test window, you can right click on any test, then Run Test With -> Edit Custom Modes to create a mode with this variable set.  You can then right click and select the tests to run with this mode, which will allow everything to work.

## Test Data

All of the projects testing data is in a shared project located at  `/bio/Source/Tests/TestData/TestUtils`.  New data files used in testing should be placed here.  On Windows, all of this data will be automatically copied by the build process into the output directory of `Bio.Tests.dll`.  In Mono, these files do not appear to be copied properly so we invoke a "cp" command as part of the build. 