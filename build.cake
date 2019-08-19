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

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var solutionName = "Ocelot.ConfigEditor";
var solutionPath = $"./{solutionName}.sln";
var projectPath = $"./src/{solutionName}/{solutionName}.csproj";
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
        
        DotNetCoreClean(solutionPath, settings);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solutionPath);
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

Task("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Gulp")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            NoRestore = true,
            Configuration = configuration
        };
        
        DotNetCoreBuild(solutionPath, settings);
    });
    
Task("Pack")
    .IsDependentOn("Build")
    .Does(() => 
    {
        var settings = new DotNetCorePackSettings
         {
             Configuration = configuration,
             OutputDirectory = "./artifacts/",
             NoBuild = true,
             NoRestore = true
         };
        
         DotNetCorePack(projectPath, settings);
    });
    
//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);