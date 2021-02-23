-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

SET IDENTITY_INSERT core.ContainerTypes ON;
GO

INSERT INTO core.ContainerTypes
    (id, "name")
SELECT 1, 'Collection'
WHERE NOT EXISTS (SELECT 1
FROM core.ContainerTypes
WHERE id = 1 and "name" = 'Collection');

INSERT INTO core.ContainerTypes
    (id, "name")
SELECT 2, 'Folder'
WHERE NOT EXISTS (SELECT 1
FROM core.ContainerTypes
WHERE id = 2 and "name" = 'Folder');
GO

SET IDENTITY_INSERT core.ContainerTypes OFF;
GO
