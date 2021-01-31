#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=vswhere&version=2.8.4"

var configuration = Argument("configuration", "Release");
var output = Argument("output", "artifacts");

var name = "SampleAPI";
var sln = $"{name}.sln";

Task("Clean").Does(() =>
{
	CleanDirectory(output);
});

Task("Build").Does(() =>
{
	MSBuild(sln, new MSBuildSettings
	{
		Restore = true,
		ToolPath = GetFiles(VSWhereLatest() + "/**/MSBuild.exe").FirstOrDefault(),
		Configuration = configuration,
	}
		.WithTarget("Publish")
	);
});

Task("Test").Does(() =>
{
	VSTest("./**/publish/**/*.UnitTest.dll", new VSTestSettings
	{
		ToolPath = GetFiles(VSWhereLatest() + "/**/vstest.console.exe").FirstOrDefault()
	});
});

Task("Artifact.Docker").Does(() =>
{
	var bin = System.IO.Path.Combine(output, $"{name}.Docker/bin");
	CreateDirectory(bin);
	CopyFiles($"10-Host/{name}/bin/netcoreapp3.1/publish/*.dll", bin);
	CopyFiles($"10-Host/{name}/bin/netcoreapp3.1/publish/*.dll.config", bin);
	CopyFiles($"10-Host/{name}/bin/netcoreapp3.1/publish/*.runtimeconfig.json", bin);
	CopyFiles($"10-Host/{name}/bin/netcoreapp3.1/publish/*.xml", bin);

	CopyDirectory("01-Docker", System.IO.Path.Combine(output, $"{name}.Docker"));
});

Task("Default")
	.IsDependentOn("Clean")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Artifact.Docker")
	.Does(() => {})
;

RunTarget("Default");