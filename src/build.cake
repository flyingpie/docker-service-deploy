#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=vswhere&version=2.6.7"

var configuration = Argument("configuration", "Release");
var isPreRelease = Argument("isPreRelease", true);
var output = Argument("output", "artifacts");

var sln = GetFiles("*.sln").First().FullPath;

Task("Default")
	.IsDependentOn("Clean")
	.IsDependentOn("Build")
//	.IsDependentOn("Artifact.NuGet")
	.Does(() => {})
;

Task("Clean").Does(() =>
{
	CleanDirectory(output);
});

Task("Build").Does(() =>
{
	DotNetCorePublish("DockerServiceDeploy.CLI/DockerServiceDeploy.CLI.csproj", new DotNetCorePublishSettings
	{
		Configuration = configuration,

//		PublishReadyToRun = true,
		PublishSingleFile = true,
		PublishTrimmed = true,

//		Runtime = "win-x64",
		Runtime = "linux-x64",

		SelfContained = true,

		MSBuildSettings = new DotNetCoreMSBuildSettings()
			.WithProperty("IncludeNativeLibrariesForSelfExtract", "true")
	});
});

RunTarget("Default");
