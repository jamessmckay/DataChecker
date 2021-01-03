// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

partial class Build
{
    Target CI_UI
        => _ => _.DependsOn(CleanUi)
            .DependsOn(RestoreNpm)
            .DependsOn(CompileAngular)
            .Executes();

    Target RestoreNpm
        => _ => _
            .Before(CompileAngular)
            .Executes(() =>
            {
                var npmProcess = ProcessTasks.StartProcess("npm", "install", UiDirectory, customLogger: (t, s) => Logger.Info(s));
                npmProcess.AssertWaitForExit();
            });

    Target CleanUi
        => _ => _
            .Before(RestoreNpm)
            .Executes(() =>
            {
                Logger.Info("Removing UI Objects");
                DeleteDirectories(UiDirectory.GlobDirectories("**/node_modules"));
            });

    Target CompileAngular
        => _ => _.Executes(() =>
        {
            var angularProcess =
                ProcessTasks.StartProcess("npm", "run build", UiDirectory, customLogger: (t, s) => Logger.Info(s));

            angularProcess.AssertWaitForExit();
        });
}
