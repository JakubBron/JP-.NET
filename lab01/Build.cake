var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solutionFile = "./ProgramZTM.sln";
var publishDir = "./publish";


Task("Restore")
    .Does(() =>
{
    Information("Restoring NuGet packages...");
    DotNetRestore(solutionFile);
});


Task("Rebuild")
    .IsDependentOn("Restore")
    .Does(() =>
{
    Information("Building solution...");
    DotNetBuild(solutionFile, new DotNetBuildSettings {
        Configuration = configuration,
        NoRestore = true
    });
});

Task("Test")
    .IsDependentOn("Rebuild")
    .Does(() =>
{
    Information("Running tests...");
    DotNetTest("./ProgZTMTests/ProgZTMTests.csproj", new DotNetTestSettings {
        Configuration = configuration,
        NoBuild = true
    });
});


Task("CleanPublish")
    .IsDependentOn("Test")
    .Does(() =>
{
    Information("Cleaning publish folder...");
    CleanDirectory(publishDir);
});


Task("Publish")
    .IsDependentOn("CleanPublish")
    .Does(() =>
{
    Information("Publishing project...");
    DotNetPublish("./ProgZTM/ProgZTM.csproj", new DotNetPublishSettings {
        Configuration = configuration,
        OutputDirectory = publishDir,
        NoBuild = true
    });
});


Task("Default")
    .IsDependentOn("Publish");


RunTarget(target);
