# The .NET Bio testing framework.

.NET Bio uses [NUnit 3.0](http://www.nunit.org/), patches should come with tests and all tests should pass before a commit or branch is merged. The unit test libraries and runner are all integrated into the Visual Studio solution and can be run directly from the Test Manager window in VS. 

Alternatively, you can use the command line tools to run the unit tests.

### .NET Core
- Open a command line or terminal prompt and execute the following statements in the `Tests/Bio.Tests` folder to run the unit tests:

```
dotnet restore
dotnet build
dotnet test
```

### Testing with nUnit command line

 - Install nUnit 3.0 from [here](https://github.com/nunit/docs/wiki/Installation).
 - Build Bio.Tests project (usually in Visual Studio, or use command line above)
 - `nunit-console Tests\Bio.Tests\bin\Release\Bio.Tests.dll`

## Test Data

All of the projects testing data is in a shared project located at  `Tests/TestData/TestUtils`.  New data files used in testing should be placed here. All of this data will be automatically copied by the build process into the output directory of `Bio.Tests.dll`.  