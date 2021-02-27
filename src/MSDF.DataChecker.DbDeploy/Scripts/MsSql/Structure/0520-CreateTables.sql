-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

-- dv_enumeration schema
IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_enumeration'
WHERE o.name='StatusType' AND o.type='u')
BEGIN
    CREATE TABLE dv_enumeration.StatusType
    (
        StatusTypeId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description] NVARCHAR(512),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_StatusType_StatusTypeId PRIMARY KEY (StatusTypeId),
        CONSTRAINT UC_StatusType_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_enumeration'
WHERE o.name='DatabaseEngineType' AND o.type='u')
BEGIN
    CREATE TABLE dv_enumeration.DatabaseEngineType
    (
        DatabaseEngineTypeId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description]  NVARCHAR(512),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_DatabaseEngineType_DatabaseEngineTypeId PRIMARY KEY (DatabaseEngineTypeId),
        CONSTRAINT UC_DatabaseEngineType_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_enumeration'
WHERE o.name='DiagnosticDataDisplayFormatType' AND o.type='u')
BEGIN
    CREATE TABLE dv_enumeration.DiagnosticDataDisplayFormatType
    (
        DiagnosticDataDisplayFormatTypeId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description]  NVARCHAR(512),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_DiagnosticDataDisplayFormatType_Id PRIMARY KEY (DiagnosticDataDisplayFormatTypeId),
        CONSTRAINT UC_DiagnosticDataDisplayFormatType_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_enumeration'
WHERE o.name='ErrorSeverityLevelType' AND o.type='u')
BEGIN
    CREATE TABLE dv_enumeration.ErrorSeverityLevelType
    (
        ErrorSeverityLevelTypeId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description] NVARCHAR(512),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_ErrorSeverityLevelType_ErrorSeverityLevelTypeId PRIMARY KEY (ErrorSeverityLevelTypeId),
        CONSTRAINT UC_ErrorSeverityLevelType_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_enumeration'
WHERE o.name='ContainerType' AND o.type='u')
BEGIN
    CREATE TABLE dv_enumeration.ContainerType
    (
        ContainerTypeId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description]  NVARCHAR(512),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_ContainerType_ContainerTypeId PRIMARY KEY (ContainerTypeId),
        CONSTRAINT UC_ContainerType_Name UNIQUE ([Name])
    );
END
GO

-- dv_metadata schema
IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='Container' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.Container
    (
        ContainerId UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
        [Name] NVARCHAR (256) NOT NULL,
        ContainerTypeId INT NOT NULL,
        ParentContainerId UNIQUEIDENTIFIER,
        IsDefault BIT DEFAULT 0,
        [Description] NVARCHAR(512),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_Container_ContainerId PRIMARY KEY (ContainerId),
        CONSTRAINT UC_Container_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='Rule' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.[Rule]
    (
        RuleUniqueId UNIQUEIDENTIFIER NOT NULL DEFAULT newid(),
        [Name] NVARCHAR(512) NOT NULL,
        [Description] NVARCHAR(MAX),
        SemanticVersion NVARCHAR(128),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_Rule_RuleUniqueId PRIMARY KEY (RuleUniqueId),
        CONSTRAINT UC_Rule_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS(SELECT 1
FROM sys.objects WHERE object_id = object_id(N'[dv_metadata].[RuleDefinition_Version]') AND type = 'SO')
BEGIN
    CREATE SEQUENCE dv_metadata.RuleDefinition_Version
    AS INT
    START WITH 1
    INCREMENT BY 1;
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='RuleDefinition' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.RuleDefinition
    (
        RuleDefinitionId INT IDENTITY NOT NULL,
        RuleUniqueId UNIQUEIDENTIFIER NOT NULL,
        [Version] INT NOT NULL DEFAULT (NEXT VALUE FOR dv_metadata.RuleDefinition_Version),
        ErrorSeverityLevelTypeId INT NOT NULL,
        ContainerId UNIQUEIDENTIFIER NOT NULL,
        ErrorMessage NVARCHAR(MAX),
        Resolution NVARCHAR(MAX),
        MaxNumberResults INT,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_RuleDefinition_RuleDefinitionId PRIMARY KEY (RuleDefinitionId),
        CONSTRAINT UC_RuleDefinition_RuleUniqueId UNIQUE (RuleUniqueId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='RuleSql' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.RuleSql
    (
        RuleSqlId INT IDENTITY NOT NULL,
        RuleDefinitionId INT NOT NULL,
        DatabaseEngineTypeId INT NOT NULL,
        DefaultDiagnosticDataDisplayformatTypeId INT NOT NULL,
        RuleSql NVARCHAR(MAX) NOT NULL,
        DiagnosticSql NVARCHAR(MAX) NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_RuleSql_RuleSqlId PRIMARY KEY (RuleSqlId),
        CONSTRAINT UC_RuleSql_RuleDefinitionId_DatabaseEngineTypeId UNIQUE (RuleDefinitionId, DatabaseEngineTypeId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='Tag' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.Tag
    (
        TagId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description] NVARCHAR(512),
        IsPublic BIT DEFAULT 0,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_Tag_TagId PRIMARY KEY (TagId),
        CONSTRAINT UC_Tag_Name UNIQUE ([Name])
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='ContainerTagAssociation' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.ContainerTagAssociation
    (
        ContainerTagAssociationId INT IDENTITY NOT NULL,
        ContainerId UNIQUEIDENTIFIER NOT NULL,
        TagId INT NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_ContainerTagAssociation_ContainerTagAssociationId PRIMARY KEY (ContainerTagAssociationId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='RuleTagAssociation' AND o.type='u')
BEGIN
    CREATE TABLE dv_metadata.RuleTagAssociation
    (
        RuleTagAssociationId INT NOT NULL,
        RuleUniqueId UNIQUEIDENTIFIER NOT NULL,
        TagId INT NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_RuleTagAssociation_RuleTagAssociationId PRIMARY KEY (RuleTagAssociationId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_metadata'
WHERE o.name='DatabaseConnection' AND o.type='u')
BEGIN
CREATE TABLE dv_metadata.DatabaseConnection
    (
        DatabaseConnectionId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        ConnectionString NVARCHAR(MAX) NOT NULL,
        DatabaseEngineTypeId INT NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_DatabaseConnection_DatabaseConnectionId PRIMARY KEY (DatabaseConnectionId),
        CONSTRAINT UC_DatabaseConnection_Name UNIQUE ([Name])
    );
END
GO

-- dv_snapshot schema
IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_snapshot'
WHERE o.name='Container' AND o.type='u')
BEGIN
    CREATE TABLE dv_snapshot.Container
    (
        ExecutionContainerId INT IDENTITY NOT NULL,
        ContainerId UNIQUEIDENTIFIER NOT NULL,
        ContainerTypeId INT NOT NULL,
        ParentExecutionContainerId INT,
        [Name] NVARCHAR(256),
        [Description]  NVARCHAR(512),
        CONSTRAINT PK_Container_ExecutionContainerId PRIMARY KEY (ExecutionContainerId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_snapshot'
WHERE o.name='DatabaseConnection' AND o.type='u')
BEGIN
    CREATE TABLE dv_snapshot.DatabaseConnection
    (
        ExecutionDatabaseConnectionId INT IDENTITY NOT NULL,
        ExecutionContextId INT NOT NULL,
        DatabaseConnectionId INT NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        ConnectionString NVARCHAR(MAX) NOT NULL,
        DatabaseEngineTypeId INT NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_DatabaseConnection_ExecutionDatabaseConnectionId PRIMARY KEY (ExecutionDatabaseConnectionId),
        CONSTRAINT UC_DatabaseConnection_ExecutionContextId UNIQUE (ExecutionContextId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_snapshot'
WHERE o.name='ContainerTagAssociation' AND o.type='u')
BEGIN
    CREATE TABLE dv_snapshot.ContainerTagAssociation
    (
        ExecutionContainerTagAssociationId INT IDENTITY NOT NULL,
        ExecutionContainerId INT NOT NULL,
        ExecutionTagId INT NOT NULL,
        CONSTRAINT PK_ContainerTagAssociation_ExecutionContainerTagAssociationId PRIMARY KEY (ExecutionContainerTagAssociationId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_snapshot'
WHERE o.name='ContainerRuleDefinitionAssociation' AND o.type='u')
BEGIN
    CREATE TABLE dv_snapshot.ContainerRuleDefinitionAssociation
    (
    ExecutionContainerRuleDefinitionAssociationId INT IDENTITY NOT NULL,
    RuleDefinitionId INT NOT NULL,
    ExecutionContainerId INT NOT NULL,
    CONSTRAINT PK_ExecutionContainerRuleDefinitionAssociationId PRIMARY KEY (ExecutionContainerRuleDefinitionAssociationId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_snapshot'
WHERE o.name='RuleDefinitionTagAssociation' AND o.type='u')
BEGIN
    CREATE TABLE dv_snapshot.RuleDefinitionTagAssociation
    (
        ExecutionRuleDefinitionTagAssociationId INT IDENTITY NOT NULL,
        RuleDefinitionId INT NOT NULL,
        ExecutionTagId INT NOT NULL,
        CONSTRAINT PK_RuleDefinitionTagAssociation_ExecutionruleTagAssociationId PRIMARY KEY (ExecutionRuleDefinitionTagAssociationId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_snapshot'
WHERE o.name='Tag' AND o.type='u')
BEGIN
    CREATE TABLE dv_snapshot.Tag
    (
        ExecutionTagId INT IDENTITY NOT NULL,
        TagId INT NOT NULL,
        [Name] NVARCHAR(256),
        [Description] NVARCHAR(512),
        IsPublic BIT DEFAULT 0,
        CONSTRAINT PK_Tag_ExecutionTagId PRIMARY KEY (ExecutionTagId),
        CONSTRAINT UC_Tag_TagId UNIQUE(TagId)
    );
END
GO

-- dv_execution schema
IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_execution'
WHERE o.name='Context' AND o.type='u')
BEGIN
    CREATE TABLE dv_execution.Context
    (
        ExecutionContextId INT IDENTITY NOT NULL,
        ExecutionDatabaseConnectionId INT NOT NULL,
        ExecutionContainerId INT NOT NULL,
        activefrom DATETIME2,
        activeto DATETIME2,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_Context_ExecutionContextId PRIMARY KEY (ExecutionContextId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_execution'
WHERE o.name='ContextTagAssociation' AND o.type='u')
BEGIN
CREATE TABLE dv_execution.ContextTagAssociation
(
    ExecutionContextTagAssociationId INT IDENTITY NOT NULL,
    ExecutionContextId INT NOT NULL,
    ExecutionTagId INT NOT NULL,
    CONSTRAINT PK_ContextTagAssociation_ExecutionContextTagAssociationId PRIMARY KEY (ExecutionContextTagAssociationId)
);
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_execution'
WHERE o.name='Job' AND o.type='u')
BEGIN
    CREATE TABLE dv_execution.Job
    (
        ExecutionJobId INT IDENTITY NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        TenantId INT DEFAULT 0,
        Cron NVARCHAR(100),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_Job_ExecutionJobId PRIMARY KEY (ExecutionJobId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_execution'
WHERE o.name='JobContextAssociation' AND o.type='u')
BEGIN
    CREATE TABLE dv_execution.JobContextAssociation
    (
        ExecutionJobContextAssociationId INT IDENTITY NOT NULL,
        ExecutionContextId INT NOT NULL,
        ExecutionJobId INT NOT NULL,
        CONSTRAINT PK_JobContextAssociation_ExecutionJobContextAssociationId PRIMARY KEY (ExecutionJobContextAssociationId),
        CONSTRAINT UC_JobContextAssociation_ExecutionContextId UNIQUE (ExecutionContextId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='dv_execution'
WHERE o.name='RuleLog' AND o.type='u')
BEGIN
    CREATE TABLE dv_execution.RuleLog
    (
        ExecutionRuleLogId INT IDENTITY NOT NULL,
        ExecutionContextId INT NOT NULL,
        RuleDefinitionId INT NOT NULL,
        StatusTypeId INT NOT NULL,
        Result INT,
        Evaluation BIT,
        Executiondate DATETIME2,
        ExecutionTimeMs BIGINT,
        Response NVARCHAR(MAX),
        DiagnosticData NVARCHAR(MAX),
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT PK_RuleLog_ExecutionRuleLogId PRIMARY KEY (ExecutionRuleLogId)
    );
END
GO
