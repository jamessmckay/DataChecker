-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT 'Ed-Fi v2.X', 'Ed-Fi Environment v2.X ', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = 'Ed-Fi v2.X');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT 'Ed-Fi v3.X', 'Ed-Fi Environment v3.X ', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = 'Ed-Fi v3.X');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '2.5', 'Data Model 2.5', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '2.5');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '2.6', 'Data Model 2.6', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '2.6');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '3.1', 'Data Model 3.1', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '3.1');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '3.2', 'Data Model 3.2', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '3.2');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '3.2-a', 'Data Model 3.2-a', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '3.2-a');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '3.2-b', 'Data Model 3.2-b', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '3.2-b');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '3.2-c', 'Data Model 3.2-c', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '3.2-c');

INSERT INTO dv_metadata.Tag ([Name], [Description], IsPublic)
SELECT '3.3-a', 'Data Model 3.3-a', 1
WHERE NOT EXISTS (SELECT 1 FROM dv_metadata.Tag WHERE [Name] = '3.3-a');
