// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


partial class Build
{
    Target CI
        => _ => _.DependsOn(Clean)
            .DependsOn(Restore)
            .DependsOn(Compile)
            .Executes();

    Target Clean
        => _ =>
        {
            return _
                .Before(Restore)
                .Executes(() =>
                {
                    Logger.Info("Removing Source Binaries");

                    var sources = WebApiDirectory.GlobDirectories("**/bin", "**/obj")
                        .Concat(DaemonDirectory.GlobDirectories("**/bin", "**/obj"))
                        .Concat(InfrastructureDirectory.GlobDirectories("**/bin", "**/obj"))
                        .ToArray();

                    DeleteDirectories(sources);

                    Logger.Info("Removing Test Binaries");

                    var tests = TestsDirectory.GlobDirectories("**/bin", "**/obj");

                    DeleteDirectories(tests);

                    Logger.Info("Cleaning Artifacts");
                    EnsureCleanDirectory(ArtifactsDirectory);
                    EnsureCleanDirectory(ReportsDirectory);
                });
        };

    Target Restore
        => _ => _
            .Before(Compile)
            .Executes(() =>
            {
                DotNetRestore(s => s
                    .SetProjectFile(Solution));
            });

    Target Compile
        => _ => _
            .Executes(() =>
            {
                DotNetBuild(s => s
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)

                    // .SetAssemblyVersion(GitVersion.AssemblySemVer)
                    // .SetFileVersion(GitVersion.AssemblySemFileVer)
                    // .SetInformationalVersion(GitVersion.InformationalVersion)
                    .EnableNoRestore());
            });

    Target Test
        => _ => _.DependsOn(Compile)
            .Executes(() =>
            {
                DotNetTest(s => s
                    .SetProjectFile(Solution)
                    .SetConfiguration(Configuration)
                    .EnableNoBuild()
                    .EnableNoRestore()
                    .SetResultsDirectory(ReportsDirectory)
                    .SetLogger($"trx;LogFileName=DataCheckerTests.xml")
                );
            });
}
