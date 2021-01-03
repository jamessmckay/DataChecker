// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;
using System.IO;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
partial class  Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild
        ? Configuration.Debug
        : Configuration.Release;
    [GitRepository] readonly GitRepository GitRepository;

    [Solution] readonly Solution Solution;

    // [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";

    AbsolutePath TestsDirectory => RootDirectory / "tests";

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    AbsolutePath UiDirectory => SourceDirectory / "MSDF.DataChecker.UI";

    AbsolutePath InfrastructureDirectory => SourceDirectory / "MSDF.DataChecker.Infrastructure";

    AbsolutePath WebApiDirectory => SourceDirectory / "MSDF.DataChecker.WebApi";

    AbsolutePath DaemonDirectory => SourceDirectory / "MSDF.DataChecker.JobExecutorDaemon";

    AbsolutePath ReportsDirectory => RootDirectory / "reports";

    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Test);

    static void DeleteDirectories(IEnumerable<AbsolutePath> absolutePaths)
        => absolutePaths.ForEach(
            x =>
            {
                if (!DirectoryExists(x))
                {
                    return;
                }

                Logger.Info($"Deleting Directory {x}");
                Directory.Delete(x, true);
            });
}
