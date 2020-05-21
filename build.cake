var target = Argument("target","Default");
var debugFolder = Directory("./AccountsManager/bin/Debug");
var testDebugFolder = Directory("./AccountsManagerUnitTests/bin/Debug");
var testProject = Directory("./AccountsManagerUnitTests/bin/Release/netcoreapp2.2/AccountsManagerUnitTests.dll");

Task("Clean")
	.Does(() => {
		CleanDirectory(debugFolder);
		CleanDirectory(testDebugFolder);
		Information("Debug folders have been removed...");
	});

Task("NuGet Restore")
	.IsDependentOn("Clean")
	.Does(() => {
		NuGetRestore("./AccountsManager.sln");
	});

Task("Build")
	.IsDependentOn("NuGet Restore")
	.Does(() => 
	{
		MSBuild("./AccountsManager.sln", new MSBuildSettings{Configuration= "Release", PlatformTarget = PlatformTarget.MSIL});
	});

Task("Compile Installer Script")
	.IsDependentOn("Build")
	.Does(() => {
		InnoSetup("./AccountsManagerSetup.iss");
	});

Task("Default")
	.IsDependentOn("Compile Installer Script");

RunTarget(target);