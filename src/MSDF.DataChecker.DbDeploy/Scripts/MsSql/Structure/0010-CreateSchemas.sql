-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

IF NOT EXISTS(SELECT 1
FROM sys.schemas
WHERE name = N'core')
BEGIN
    EXEC('CREATE SCHEMA core');
END
GO

IF NOT EXISTS(SELECT 1
FROM sys.schemas
WHERE name = N'datachecker')
BEGIN
    EXEC('CREATE SCHEMA datachecker');
END
GO

IF NOT EXISTS(SELECT 1
FROM sys.schemas
WHERE name = N'destination')
BEGIN
    EXEC('CREATE SCHEMA destination');
END
GO
