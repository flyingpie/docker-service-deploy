﻿#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
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
	MSBuild(sln, new MSBuildSettings
	{
		Configuration = configuration,
		Restore = true,
		ToolPath = GetFiles(VSWhereLatest() + "/**/MSBuild.exe").FirstOrDefault()
	});
});

RunTarget("Default");