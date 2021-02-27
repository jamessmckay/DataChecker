-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

INSERT INTO dv_enumeration.StatusType ([Name], [Description])
SELECT 'Succeeded', 'Succeeded'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.StatusType WHERE [Name] = 'Succeeded');

INSERT INTO dv_enumeration.StatusType ([Name], [Description])
SELECT 'Failed', 'Failed'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.StatusType WHERE [Name] = 'Failed');

INSERT INTO dv_enumeration.StatusType ([Name], [Description])
SELECT 'Error', 'Error'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.StatusType WHERE [Name] = 'Error');

INSERT INTO dv_enumeration.DatabaseEngineType ([Name], [Description])
SELECT 'sqlServer', 'Sql Server'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.DatabaseEngineType WHERE [Name] = 'sqlServer');

INSERT INTO dv_enumeration.DatabaseEngineType ([Name], [Description])
SELECT 'postgreSql', 'PostgreSQL'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.DatabaseEngineType WHERE [Name] = 'postgreSql');

INSERT INTO dv_enumeration.DiagnosticDataDisplayFormatType ([Name], [Description])
SELECT 'raw', 'Raw JSON'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.DiagnosticDataDisplayFormatType WHERE [Name] = 'raw');

INSERT INTO dv_enumeration.ErrorSeverityLevelType ([Name], [Description])
SELECT 'debug', 'Debug'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.ErrorSeverityLevelType WHERE [Name] = 'debug');

INSERT INTO dv_enumeration.ErrorSeverityLevelType ([Name], [Description])
SELECT 'info', 'Information'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.ErrorSeverityLevelType WHERE [Name] = 'info');

INSERT INTO dv_enumeration.ErrorSeverityLevelType ([Name], [Description])
SELECT 'warn', 'Warning'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.ErrorSeverityLevelType WHERE [Name] = 'warn');

INSERT INTO dv_enumeration.ErrorSeverityLevelType ([Name], [Description])
SELECT 'fatal', 'Fatal'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.ErrorSeverityLevelType WHERE [Name] = 'fatal');

INSERT INTO dv_enumeration.ContainerType ([Name], [Description])
SELECT 'Collection', 'Collection'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.ContainerType WHERE [Name] = 'Collection');

INSERT INTO dv_enumeration.ContainerType ([Name], [Description])
SELECT 'Folder', 'Folder'
WHERE NOT EXISTS (SELECT 1 FROM dv_enumeration.ContainerType WHERE [Name] = 'Folder');
