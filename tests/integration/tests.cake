#load "buildData.cake"

var target = Argument<string>("target", "Run-All-Tests");

Setup<BuildData>(ctx =>
{
    var buildDir = "../../BuildArtifacts/temp/_PublishedApplications";
    var grmExecutable = ctx.GetFiles(buildDir + "/**/*.exe").First();

	ctx.Information("Registering Built GRM executable...");
	ctx.Tools.RegisterFile(grmExecutable);

    return new BuildData(ctx);
});

Task("Create-Release")
    .Does<BuildData>((data) =>
{
    GitReleaseManagerCreate(
        data.GitHubUsername,
        data.GitHubPassword,
        data.GitHubOwner,
        data.GitHubRepository,
        new GitReleaseManagerCreateSettings {
            Milestone         = data.GrmMilestone,
            TargetCommitish   = "master"
        });
});

Task("Add-Asset-To-Release")
    .Does(() =>
{

});

Task("Export-Release")
    .Does(() =>
{

});

Task("Discard-Release")
    .Does<BuildData>((data) =>
{
    var settings = new GitReleaseManagerCreateSettings();
    settings.Milestone = data.GrmMilestone;

    // TODO: This can be replaced when a discard alias is added to Cake
    settings.ArgumentCustomization = args => {
        var newArgs = new ProcessArgumentBuilder()
                        .Append("discard");

        Array.ForEach(
            args.Skip(1).ToArray(),
            newArgs.Append
            );

        return newArgs;
    };

    GitReleaseManagerCreate(
        data.GitHubUsername,
        data.GitHubPassword,
        data.GitHubOwner,
        data.GitHubRepository,
        settings);
});

Task("Run-All-Tests")
    .IsDependentOn("Create-Release")
    .IsDependentOn("Add-Asset-To-Release")
    .IsDependentOn("Export-Release")
    .IsDependentOn("Discard-Release");

RunTarget(target);