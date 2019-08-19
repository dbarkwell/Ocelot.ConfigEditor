// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.1.0

// Install addins.
#addin "nuget:https://api.nuget.org/v3/index.json?package=Cake.Gulp&version=0.12.0"

// Install tools.
#tool "nuget:https://api.nuget.org/v3/index.json?package=nuget.commandline&version=4.9.2"

// Load other scripts.
#load "./build/parameters.cake"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionName = "Ocelot.ConfigEditor";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory($"./src/{solutionName}/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    
    var settings = new DotNetCoreCleanSettings
    {
        Configuration = configuration
    };
    
    DotNetCoreClean($"./{solutionName}.sln", settings);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore($"./{solutionName}.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Gulp")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        NoRestore = true
    };
    
    DotNetCoreBuild($"./{solutionName}.sln", settings);
});

Task("Gulp")
    .Does(() =>
{
    Gulp.Local.Execute(settings => 
    {
        settings.WithGulpFile($"./src/{solutionName}/gulpfile.js");
        settings.SetPathToGulpJs($"./src/{solutionName}/node_modules/gulp/bin/gulp.js");
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);