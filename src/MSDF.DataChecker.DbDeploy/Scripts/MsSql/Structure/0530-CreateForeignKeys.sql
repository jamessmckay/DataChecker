-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

-- dv_metadata schema
IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Container'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_Container_ContainerType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.Container DROP CONSTRAINT FK_Container_ContainerType;
END
GO

ALTER TABLE dv_metadata.Container ADD CONSTRAINT FK_Container_ContainerType FOREIGN KEY (ContainerTypeId)
REFERENCES dv_enumeration.ContainerType (ContainerTypeId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Container'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_Container_ParentContainer'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.Container DROP CONSTRAINT FK_Container_ParentContainer;
END
GO

ALTER TABLE dv_metadata.Container ADD CONSTRAINT FK_Container_ParentContainer FOREIGN KEY (ParentContainerid)
REFERENCES dv_metadata.Container (Containerid);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleDefinition'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleDefinition_Rule'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleDefinition DROP CONSTRAINT FK_RuleDefinition_Rule;
END
GO

ALTER TABLE dv_metadata.RuleDefinition ADD CONSTRAINT FK_RuleDefinition_Rule FOREIGN KEY (RuleUniqueId)
REFERENCES dv_metadata.[Rule] (RuleUniqueId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleDefinition'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleDefinition_ErrorSeverityLevelType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleDefinition DROP CONSTRAINT FK_RuleDefinition_ErrorSeverityLevelType;
END
GO

ALTER TABLE dv_metadata.RuleDefinition ADD CONSTRAINT FK_RuleDefinition_ErrorSeverityLevelType FOREIGN KEY (ErrorSeverityLevelTypeId)
REFERENCES dv_enumeration.ErrorSeverityLevelType (ErrorSeverityLevelTypeId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleDefinition'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleDefinition_Container'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleDefinition DROP CONSTRAINT FK_RuleDefinition_Container;
END
GO

ALTER TABLE dv_metadata.RuleDefinition ADD CONSTRAINT FK_RuleDefinition_Container FOREIGN KEY (ContainerId)
REFERENCES dv_metadata.Container (ContainerId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleSql'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleSql_RuleDefinition'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleSql DROP CONSTRAINT FK_RuleSql_RuleDefinition;
END
GO

ALTER TABLE dv_metadata.RuleSql ADD CONSTRAINT FK_RuleSql_RuleDefinition FOREIGN KEY (RuleDefinitionId)
REFERENCES dv_metadata.RuleDefinition (RuleDefinitionId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleSql'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleSql_DatabaseEngineType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleSql DROP CONSTRAINT FK_RuleSql_DatabaseEngineType;
END
GO

ALTER TABLE dv_metadata.RuleSql ADD CONSTRAINT FK_RuleSql_DatabaseEngineType FOREIGN KEY (DatabaseEngineTypeId)
REFERENCES dv_enumeration.DatabaseEngineType (DatabaseEngineTypeId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleSql'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleSql_DefaultDiagnosticDataDisplayFormatType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleSql DROP CONSTRAINT FK_RuleSql_DefaultDiagnosticDataDisplayFormatType;
END
GO

ALTER TABLE dv_metadata.RuleSql ADD CONSTRAINT FK_RuleSql_DefaultDiagnosticDataDisplayFormatType FOREIGN KEY (defaultDiagnosticDataDisplayFormatTypeId)
REFERENCES dv_enumeration.DiagnosticDataDisplayFormatType (DiagnosticDataDisplayFormatTypeId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContainerTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_ContainerTagAssociation_Container'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.ContainerTagAssociation DROP CONSTRAINT FK_ContainerTagAssociation_Container;
END
GO

ALTER TABLE dv_metadata.ContainerTagAssociation ADD CONSTRAINT FK_ContainerTagAssociation_Container FOREIGN KEY (ContainerId)
REFERENCES dv_metadata.Container (ContainerId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContainerTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_ContainerTagAssociation_Tag'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.ContainerTagAssociation DROP CONSTRAINT FK_ContainerTagAssociation_Tag;
END
GO

ALTER TABLE dv_metadata.ContainerTagAssociation ADD CONSTRAINT FK_ContainerTagAssociation_Tag FOREIGN KEY (TagId)
REFERENCES dv_metadata.Tag (TagId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleTagAssociation_Tag'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleTagAssociation DROP CONSTRAINT FK_RuleTagAssociation_Tag;
END
GO

ALTER TABLE dv_metadata.RuleTagAssociation ADD CONSTRAINT FK_RuleTagAssociation_Tag FOREIGN KEY (TagId)
REFERENCES dv_metadata.Tag (TagId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_RuleTagAssociation_Rule'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.RuleTagAssociation DROP CONSTRAINT FK_RuleTagAssociation_Rule;
END
GO

ALTER TABLE dv_metadata.RuleTagAssociation ADD CONSTRAINT FK_RuleTagAssociation_Rule FOREIGN KEY (RuleUniqueId)
REFERENCES dv_metadata.[Rule] (RuleUniqueId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='DatabaseConnection'
    AND CONSTRAINT_SCHEMA='dv_metadata'
    AND CONSTRAINT_NAME='FK_DatabaseConnection_DatabaseEngineType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_metadata.DatabaseConnection DROP CONSTRAINT FK_DatabaseConnection_DatabaseEngineType;
END
GO

ALTER TABLE dv_metadata.DatabaseConnection ADD CONSTRAINT FK_DatabaseConnection_DatabaseEngineType FOREIGN KEY (DatabaseEngineTypeId)
REFERENCES dv_enumeration.DatabaseEngineType (DatabaseEngineTypeId);
GO

-- dv_snapshot schema

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Container'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_Container_Containertype'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.Container DROP CONSTRAINT FK_Container_Containertype;
END
GO

ALTER TABLE dv_snapshot.Container ADD CONSTRAINT FK_Container_Containertype FOREIGN KEY (ContainertypeId)
REFERENCES dv_enumeration.Containertype (ContainertypeId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Container'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_Container_ParentContainer'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.Container DROP CONSTRAINT FK_Container_ParentContainer;
END
GO

ALTER TABLE dv_snapshot.Container ADD CONSTRAINT FK_Container_ParentContainer FOREIGN KEY (parentexecutionContainerId)
REFERENCES dv_snapshot.Container (executionContainerId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='DatabaseConnection'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_DatabaseConnenction_ExecutionContext'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.DatabaseConnection DROP CONSTRAINT FK_DatabaseConnenction_ExecutionContext;
END
GO

ALTER TABLE dv_snapshot.DatabaseConnection ADD CONSTRAINT FK_DatabaseConnenction_ExecutionContext FOREIGN KEY (ExecutionContextId)
REFERENCES dv_execution.Context (ExecutionContextId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='DatabaseConnection'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_DatabaseConnection_DatabaseConnection'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.DatabaseConnection DROP CONSTRAINT FK_DatabaseConnection_DatabaseConnection;
END
GO

ALTER TABLE dv_snapshot.DatabaseConnection ADD CONSTRAINT FK_DatabaseConnection_DatabaseConnection FOREIGN KEY (DatabaseConnectionId)
REFERENCES dv_metadata.DatabaseConnection (DatabaseConnectionId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='DatabaseConnection'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_DatabaseConnection_DatabaseEngineType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.DatabaseConnection DROP CONSTRAINT FK_DatabaseConnection_DatabaseEngineType;
END
GO

ALTER TABLE dv_snapshot.DatabaseConnection ADD CONSTRAINT FK_DatabaseConnection_DatabaseEngineType FOREIGN KEY (DatabaseEngineTypeId)
REFERENCES dv_enumeration.DatabaseEngineType (DatabaseEngineTypeId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContainerTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_ContainerTagAssociation_Container'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.ContainerTagAssociation DROP CONSTRAINT FK_ContainerTagAssociation_Container;
END

ALTER TABLE dv_snapshot.ContainerTagAssociation ADD CONSTRAINT FK_ContainerTagAssociation_Container FOREIGN KEY (ExecutionContainerId)
REFERENCES dv_snapshot.Container (ExecutionContainerId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContainerTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_ContainerTagAssociation_Tag'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.ContainerTagAssociation DROP CONSTRAINT FK_ContainerTagAssociation_Tag;
END
GO

ALTER TABLE dv_snapshot.ContainerTagAssociation ADD CONSTRAINT FK_ContainerTagAssociation_Tag FOREIGN KEY (ExecutionTagId)
REFERENCES dv_snapshot.Tag (ExecutionTagId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleDefinitionTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_RuleDefinitionTagAssociation_RuleDefinition'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.RuleDefinitionTagAssociation DROP CONSTRAINT FK_RuleDefinitionTagAssociation_RuleDefinition;
END
GO

ALTER TABLE dv_snapshot.RuleDefinitionTagAssociation ADD CONSTRAINT FK_RuleDefinitionTagAssociation_RuleDefinition FOREIGN KEY (RuleDefinitionId)
REFERENCES dv_metadata.RuleDefinition (RuleDefinitionId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleDefinitionTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_RuleDefinitionTagAssociation_Tag'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
ALTER TABLE dv_snapshot.RuleDefinitionTagAssociation DROP CONSTRAINT FK_RuleDefinitionTagAssociation_Tag;
END
GO

ALTER TABLE dv_snapshot.RuleDefinitionTagAssociation ADD CONSTRAINT FK_RuleDefinitionTagAssociation_Tag FOREIGN KEY (ExecutionTagId)
REFERENCES dv_snapshot.Tag (ExecutionTagId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContainerRuleDefinitionAssociation'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_ContainerRuleDefinitionAssociation_Container'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.ContainerRuleDefinitionAssociation DROP CONSTRAINT FK_ContainerRuleDefinitionAssociation_Container;
END
GO

ALTER TABLE dv_snapshot.ContainerRuleDefinitionAssociation ADD CONSTRAINT FK_ContainerRuleDefinitionAssociation_Container FOREIGN KEY (executionContainerId)
REFERENCES dv_snapshot.Container (executionContainerId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContainerRuleDefinitionAssociation'
    AND CONSTRAINT_SCHEMA='dv_snapshot'
    AND CONSTRAINT_NAME='FK_ContainerRuleDefinitionAssociation_RuleDefinition'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_snapshot.ContainerRuleDefinitionAssociation DROP CONSTRAINT FK_ContainerRuleDefinitionAssociation_RuleDefinition;
END
GO

ALTER TABLE dv_snapshot.ContainerRuleDefinitionAssociation ADD CONSTRAINT FK_ContainerRuleDefinitionAssociation_RuleDefinition FOREIGN KEY (RuleDefinitionId)
REFERENCES dv_metadata.RuleDefinition (RuleDefinitionId);
GO

-- dv_execution schema

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Context'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_Context_DatabaseConnection'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.Context DROP CONSTRAINT FK_Context_DatabaseConnection;
END
GO

ALTER TABLE dv_execution.Context ADD CONSTRAINT FK_Context_DatabaseConnection FOREIGN KEY (ExecutionDatabaseConnectionId)
REFERENCES dv_snapshot.DatabaseConnection (ExecutionDatabaseConnectionId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Context'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_Context_Container'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.Context DROP CONSTRAINT FK_Context_Container;
END
GO

ALTER TABLE dv_execution.Context ADD CONSTRAINT FK_Context_Container FOREIGN KEY (ExecutionContainerId)
REFERENCES dv_snapshot.Container (ExecutionContainerId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContextTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_ContextTagAssociation_Context'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.ContextTagAssociation DROP CONSTRAINT FK_ContextTagAssociation_Context;
END
GO

ALTER TABLE dv_execution.ContextTagAssociation ADD CONSTRAINT FK_ContextTagAssociation_Context FOREIGN KEY (ExecutionContextId)
REFERENCES dv_execution.Context (ExecutionContextId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='ContextTagAssociation'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_ContextTagAssociation_Tag'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.ContextTagAssociation DROP CONSTRAINT FK_ContextTagAssociation_Tag;
END
GO

ALTER TABLE dv_execution.ContextTagAssociation ADD CONSTRAINT FK_ContextTagAssociation_Tag FOREIGN KEY (ExecutionTagId)
REFERENCES dv_snapshot.Tag (ExecutionTagId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='JobContextAssociation'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_JobContextAssociation_Context'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.JobContextAssociation DROP CONSTRAINT FK_JobContextAssociation_Context;
END
GO

ALTER TABLE dv_execution.JobContextAssociation ADD CONSTRAINT FK_JobContextAssociation_Context FOREIGN KEY (ExecutionContextId)
REFERENCES dv_execution.Context (ExecutionContextId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='JobContextAssociation'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_JobContextAssociation_Job'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.JobContextAssociation DROP CONSTRAINT FK_JobContextAssociation_Job;
END
GO

ALTER TABLE dv_execution.JobContextAssociation ADD CONSTRAINT FK_JobContextAssociation_Job FOREIGN KEY (ExecutionJobId)
REFERENCES dv_execution.Job (ExecutionJobId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleLog'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_RuleLog_Context'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.RuleLog DROP CONSTRAINT FK_RuleLog_Context;
END
GO

ALTER TABLE dv_execution.RuleLog ADD CONSTRAINT FK_RuleLog_Context FOREIGN KEY (executionContextId)
REFERENCES dv_execution.Context (executionContextId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleLog'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_RuleLog_RuleDefinition'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.RuleLog DROP CONSTRAINT FK_RuleLog_RuleDefinition;
END
GO

ALTER TABLE dv_execution.RuleLog ADD CONSTRAINT FK_RuleLog_RuleDefinition FOREIGN KEY (RuleDefinitionId)
REFERENCES dv_metadata.RuleDefinition (RuleDefinitionId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleLog'
    AND CONSTRAINT_SCHEMA='dv_execution'
    AND CONSTRAINT_NAME='FK_RuleLog_StatusType'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE dv_execution.RuleLog DROP CONSTRAINT FK_RuleLog_StatusType;
END
GO

ALTER TABLE dv_execution.RuleLog ADD CONSTRAINT FK_RuleLog_StatusType FOREIGN KEY (StatusTypeId)
REFERENCES dv_enumeration.StatusType (StatusTypeId);
GO
