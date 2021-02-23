-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='datachecker'
WHERE o.name='DatabaseEnvironments' AND o.type='U')
BEGIN
    CREATE TABLE datachecker.DatabaseEnvironments
    (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(max) NULL,
        Version int NOT NULL,
        "Database" nvarchar(max) NULL,
        "User" nvarchar(max) NULL,
        "Password" nvarchar(max) NULL,
        DataSource nvarchar(max) NULL,
        ExtraData nvarchar(max) NULL,
        MapTables nvarchar(max) NULL,
        CreatedDate datetime2(7) NOT NULL,
        SecurityIntegrated bit NULL,
        MaxNumberResults int NULL,
        TimeoutInMinutes int NULL,
        CONSTRAINT PK_DatabaseEnvironments PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='datachecker'
WHERE o.name='Containers' AND o.type='U')
BEGIN
    CREATE TABLE datachecker.Containers
    (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(max) NULL,
        ContainerTypeId int NOT NULL,
        CreatedByUserId uniqueidentifier NULL,
        ParentContainerId uniqueidentifier NULL,
        IsDefault bit NOT NULL,
        Description nvarchar(max) NULL,
        EnvironmentType int NULL,
        RuleDetailsDestinationId int NULL,
        CONSTRAINT PK_Containers PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='datachecker'
WHERE o.name='Rules' AND o.type='U')
BEGIN
    CREATE TABLE datachecker.Rules
    (
        Id uniqueidentifier NOT NULL,
        ContainerId uniqueidentifier NOT NULL,
        Name nvarchar(max) NULL,
        Description nvarchar(max) NULL,
        ErrorMessage nvarchar(max) NULL,
        ErrorSeverityLevel int NOT NULL,
        Resolution nvarchar(max) NULL,
        DiagnosticSql nvarchar(max) NULL,
        Version nvarchar(max) NULL,
        RuleIdentification nvarchar(255) NULL,
        MaxNumberResults int NULL,
        CONSTRAINT PK_Rules PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UC_Rules UNIQUE(ContainerId, RuleIdentification)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='destination'
WHERE o.name='RuleExecutionLogs' AND o.type='U')
BEGIN
    CREATE TABLE destination.RuleExecutionLogs
    (
        Id int IDENTITY(1,1) NOT NULL,
        RuleId uniqueidentifier NOT NULL,
        DatabaseEnvironmentId uniqueidentifier NOT NULL,
        Response nvarchar(max) NULL,
        Result int NOT NULL,
        Evaluation bit NOT NULL,
        StatusId int NOT NULL,
        ExecutionDate datetime2(7) NOT NULL,
        ExecutionTimeMs bigint NOT NULL,
        ExecutedSql nvarchar(max) NULL,
        DiagnosticSql nvarchar(max) NULL,
        RuleDetailsDestinationId int NULL,
        DetailsSchema nvarchar(max) NULL,
        DetailsTableName nvarchar(max) NULL,
        CONSTRAINT PK_RuleExecutionLogs PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='core'
WHERE o.name='Catalogs' AND o.type='U')
BEGIN
    CREATE TABLE core.Catalogs
    (
        Id int IDENTITY(1,1) NOT NULL,
        CatalogType nvarchar(max) NULL,
        Name nvarchar(max) NULL,
        Description nvarchar(max) NULL,
        Updated datetime2(7) NOT NULL,
        CONSTRAINT PK_Catalogs PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='core'
WHERE o.name='ContainerTypes' AND o.type='U')
BEGIN
    CREATE TABLE core.ContainerTypes
    (
        Id int IDENTITY(1,1) NOT NULL,
        Name nvarchar(max) NULL,
        Description nvarchar(max) NULL,
        CONSTRAINT PK_ContainerTypes PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='core'
WHERE o.name='Logs' AND o.type='U')
BEGIN
    CREATE TABLE core.Logs
    (
        Id int IDENTITY(1,1) NOT NULL,
        Information nvarchar(max) NULL,
        Source nvarchar(max) NULL,
        DateCreated datetime2(7) NOT NULL,
        CONSTRAINT PK_Logs PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='datachecker'
WHERE o.name='TagEntities' AND o.type='U')
BEGIN
    CREATE TABLE datachecker.TagEntities
    (
        Id int IDENTITY(1,1) NOT NULL,
        TagId int NOT NULL,
        ContainerId uniqueidentifier NULL,
        RuleId uniqueidentifier NULL,
        CONSTRAINT PK_TagEntities PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='datachecker'
WHERE o.name='Tags' AND o.type='U')
BEGIN
    CREATE TABLE datachecker.Tags
    (
        Id int IDENTITY(1,1) NOT NULL,
        Name nvarchar(max) NULL,
        Description nvarchar(max) NULL,
        IsPublic bit NOT NULL,
        Created datetime2(7) NOT NULL,
        Updated datetime2(7) NOT NULL,
        CONSTRAINT PK_Tags PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='datachecker'
WHERE o.name='UserParams' AND o.type='U')
BEGIN
    CREATE TABLE datachecker.UserParams
    (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(max) NULL,
        Value nvarchar(max) NULL,
        DatabaseEnvironmentId uniqueidentifier NOT NULL,
        CONSTRAINT PK_UserParams PRIMARY KEY CLUSTERED (Id)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_id = o.schema_id AND s.name='destination'
WHERE o.name='EdFiRuleExecutionLogDetails' AND o.type='U')
BEGIN
    CREATE TABLE destination.EdFiRuleExecutionLogDetails
    (
        Id int IDENTITY(1,1) NOT NULL,
        EducationOrganizationId int NULL,
        StudentUniqueId nvarchar(max) NULL,
        CourseCode nvarchar(max) NULL,
        Discriminator nvarchar(max) NULL,
        ProgramName nvarchar(max) NULL,
        StaffUniqueId nvarchar(max) NULL,
        OtherDetails nvarchar(max) NULL,
        RuleExecutionLogId int NOT NULL,
        CONSTRAINT PK_EdFiRuleExecutionLogDetails PRIMARY KEY CLUSTERED (Id)
    );
END
GO
