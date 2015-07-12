!#/bin/sh
# This script will run the tests on mono unix, it is not very robust and is more or
# less specific to one machine, but can be altered to run tests anywhere and is a placeholder for now.

# Build the tests project, this is somewhat brittle right now.  Best to build in VS or xamarin studio if problems are encountered.
xbuild Source/Tests/Bio.Tests/Bio.Tests.Core.csproj

# Turn on IO map to ensure all the paths are converted
# e.g. "TestUtils\data.txt" -> "TestUtils/data.txt"
export MONO_IOMAP=all
# Run the tests
mono ~/bin/NUnit-3.0.0-beta-2/nunit-console.exe Source/Tests/Bio.Tests/bin/Debug/Bio.Tests.dll