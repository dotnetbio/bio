Echo "copying DLLs into bin folder."

@copy "Source\Framework\Bio.Core\bin\Release\Bio.Core.dll" ".\bin"
@copy "Source\Framework\Bio.Core\bin\Release\Bio.Core.xml" ".\bin"
@copy "Source\Framework\Bio.Core\bin\Release\Bio.Core.pdb" ".\bin"

@copy "Source\Framework\Bio.Android\bin\Release\Bio.Android.dll" ".\bin"
@copy "Source\Framework\Bio.Android\bin\Release\Bio.Android.xml" ".\bin"
@copy "Source\Framework\Bio.Android\bin\Release\Bio.Android.pdb" ".\bin"

@copy "Source\Framework\Bio.Desktop\bin\Release\Bio.Desktop.dll" ".\bin"
@copy "Source\Framework\Bio.Desktop\bin\Release\Bio.Desktop.xml" ".\bin"
@copy "Source\Framework\Bio.Desktop\bin\Release\Bio.Desktop.pdb" ".\bin"

@copy "Source\Framework\Bio.iOS\bin\iPhone\Release\Bio.iOS.dll" ".\bin"
@copy "Source\Framework\Bio.iOS\bin\iPhone\Release\Bio.iOS.xml" ".\bin"
@copy "Source\Framework\Bio.iOS\bin\iPhone\Release\Bio.iOS.pdb" ".\bin"

@copy "Source\Framework\Bio.iOS.Unified\bin\iPhone\Release\Bio.iOS.Unified.dll" ".\bin"
@copy "Source\Framework\Bio.iOS.Unified\bin\iPhone\Release\Bio.iOS.Unified.xml" ".\bin"
@copy "Source\Framework\Bio.iOS.Unified\bin\iPhone\Release\Bio.iOS.Unified.pdb" ".\bin"

@copy "Source\Framework\Bio.WebServices\bin\Release\Bio.WebServices.dll" ".\bin"
@copy "Source\Framework\Bio.WebServices\bin\Release\Bio.WebServices.xml" ".\bin"
@copy "Source\Framework\Bio.WebServices\bin\Release\Bio.WebServices.pdb" ".\bin"

@copy "Source\Framework\Shims\Bio.Platform.Helpers\bin\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.PCL.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers\bin\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.PCL.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers\bin\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.PCL.pdb"

@copy "Source\Framework\Shims\Bio.Platform.Helpers.Desktop\bin\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.Desktop.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.Desktop\bin\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.Desktop.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.Desktop\bin\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.Desktop.pdb"

@copy "Source\Framework\Shims\Bio.Platform.Helpers.iOS\bin\iPhone\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.iOS.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.iOS\bin\iPhone\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.iOS.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.iOS\bin\iPhone\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.iOS.pdb"

@copy "Source\Framework\Shims\Bio.Platform.Helpers.iOS.Unified\bin\iPhone\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.iOS.Unified.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.iOS.Unified\bin\iPhone\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.iOS.Unified.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.iOS.Unified\bin\iPhone\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.iOS.Unified.pdb"

@copy "Source\Framework\Shims\Bio.Platform.Helpers.Droid\bin\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.Droid.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.Droid\bin\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.iOS.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.Droid\bin\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.iOS.pdb"

@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinPRT\bin\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.WinPRT.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinPRT\bin\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.WinPRT.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinPRT\bin\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.WinPRT.pdb"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinPRT\bin\Release\Bio.Platform.Helpers.pri" ".\bin\Bio.Platform.Helpers.WinPRT.pri"

@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinRT\bin\Release\Bio.Platform.Helpers.dll" ".\bin\Bio.Platform.Helpers.WinRT.dll"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinRT\bin\Release\Bio.Platform.Helpers.xml" ".\bin\Bio.Platform.Helpers.WinRT.xml"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinRT\bin\Release\Bio.Platform.Helpers.pdb" ".\bin\Bio.Platform.Helpers.WinRT.pdb"
@copy "Source\Framework\Shims\Bio.Platform.Helpers.WinRT\bin\Release\Bio.Platform.Helpers.pri" ".\bin\Bio.Platform.Helpers.WinRT.pri"

@copy "Source\Add-in\Bio.Padena\bin\Release\Bio.Padena.dll" ".\bin"
@copy "Source\Add-in\Bio.Padena\bin\Release\Bio.Padena.xml" ".\bin"
@copy "Source\Add-in\Bio.Padena\bin\Release\Bio.Padena.pdb" ".\bin"

@copy "Source\Add-in\Bio.Pamsam\bin\Release\Bio.Pamsam.dll" ".\bin"
@copy "Source\Add-in\Bio.Pamsam\bin\Release\Bio.Pamsam.xml" ".\bin"
@copy "Source\Add-in\Bio.Pamsam\bin\Release\Bio.Pamsam.pdb" ".\bin"

Echo "Creating Nuget package - Bio.Core"
@Nuget pack bioCore.nuspec

Echo "Creating Nuget package - Bio.Web"
@Nuget pack bioWeb.nuspec

Echo "Creating Nuget package - Bio.Algorithms"
@Nuget pack bioAlgorithms.nuspec

