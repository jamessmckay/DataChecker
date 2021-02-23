-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Containers'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_Containers_Catalogs_RuleDetailsDestinationId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.Containers DROP CONSTRAINT FK_Containers_Catalogs_RuleDetailsDestinationId;
END
GO

ALTER TABLE datachecker.Containers  WITH CHECK ADD  CONSTRAINT FK_Containers_Catalogs_RuleDetailsDestinationId FOREIGN KEY (RuleDetailsDestinationId)
REFERENCES core.Catalogs (Id)
GO

ALTER TABLE datachecker.Containers CHECK CONSTRAINT FK_Containers_Catalogs_RuleDetailsDestinationId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Containers'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_Containers_Containers_ParentContainerId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.Containers DROP CONSTRAINT FK_Containers_Containers_ParentContainerId;
END
GO

ALTER TABLE datachecker.Containers  WITH CHECK ADD  CONSTRAINT FK_Containers_Containers_ParentContainerId FOREIGN KEY (ParentContainerId)
REFERENCES datachecker.Containers (Id)
GO

ALTER TABLE datachecker.Containers CHECK CONSTRAINT FK_Containers_Containers_ParentContainerId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Containers'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_Containers_ContainerTypes_ContainerTypeId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.Containers DROP CONSTRAINT FK_Containers_ContainerTypes_ContainerTypeId;
END
GO

ALTER TABLE datachecker.Containers  WITH CHECK ADD  CONSTRAINT FK_Containers_ContainerTypes_ContainerTypeId FOREIGN KEY (ContainerTypeId)
REFERENCES core.ContainerTypes (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.Containers CHECK CONSTRAINT FK_Containers_ContainerTypes_ContainerTypeId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='Rules'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_Rules_Containers_ContainerId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.Rules DROP CONSTRAINT FK_Rules_Containers_ContainerId;
END
GO

ALTER TABLE datachecker.Rules  WITH CHECK ADD  CONSTRAINT FK_Rules_Containers_ContainerId FOREIGN KEY (ContainerId)
REFERENCES datachecker.Containers (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.Rules CHECK CONSTRAINT FK_Rules_Containers_ContainerId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='TagEntities'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_TagEntities_Containers_ContainerId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.TagEntities DROP CONSTRAINT FK_TagEntities_Containers_ContainerId;
END
GO

ALTER TABLE datachecker.TagEntities  WITH CHECK ADD  CONSTRAINT FK_TagEntities_Containers_ContainerId FOREIGN KEY (ContainerId)
REFERENCES datachecker.Containers (Id)
GO

ALTER TABLE datachecker.TagEntities CHECK CONSTRAINT FK_TagEntities_Containers_ContainerId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='TagEntities'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_TagEntities_Rules_RuleId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.TagEntities DROP CONSTRAINT FK_TagEntities_Rules_RuleId;
END
GO

ALTER TABLE datachecker.TagEntities  WITH CHECK ADD  CONSTRAINT FK_TagEntities_Rules_RuleId FOREIGN KEY (RuleId)
REFERENCES datachecker.Rules (Id)
GO

ALTER TABLE datachecker.TagEntities CHECK CONSTRAINT FK_TagEntities_Rules_RuleId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='TagEntities'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_TagEntities_Tags_TagId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.TagEntities DROP CONSTRAINT FK_TagEntities_Tags_TagId;
END
GO

ALTER TABLE datachecker.TagEntities  WITH CHECK ADD  CONSTRAINT FK_TagEntities_Tags_TagId FOREIGN KEY (TagId)
REFERENCES datachecker.Tags (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.TagEntities CHECK CONSTRAINT FK_TagEntities_Tags_TagId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='UserParams'
    AND CONSTRAINT_SCHEMA='datachecker'
    AND CONSTRAINT_NAME='FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE datachecker.UserParams DROP CONSTRAINT FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId;
END
GO

ALTER TABLE datachecker.UserParams  WITH CHECK ADD  CONSTRAINT FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId FOREIGN KEY (DatabaseEnvironmentId)
REFERENCES datachecker.DatabaseEnvironments (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.UserParams CHECK CONSTRAINT FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='EdFiRuleExecutionLogDetails'
    AND CONSTRAINT_SCHEMA='destination'
    AND CONSTRAINT_NAME='FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE destination.EdFiRuleExecutionLogDetails DROP CONSTRAINT FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId;
END
GO
ALTER TABLE destination.EdFiRuleExecutionLogDetails  WITH CHECK ADD  CONSTRAINT FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId FOREIGN KEY (RuleExecutionLogId)
REFERENCES destination.RuleExecutionLogs (Id)
ON DELETE CASCADE
GO

ALTER TABLE destination.EdFiRuleExecutionLogDetails CHECK CONSTRAINT FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='RuleExecutionLogs'
    AND CONSTRAINT_SCHEMA='destination'
    AND CONSTRAINT_NAME='FK_RuleExecutionLogs_Rules_RuleId'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE destination.RuleExecutionLogs DROP CONSTRAINT "FK_RuleExecutionLogs_Rules_RuleId";
END
GO
ALTER TABLE destination.RuleExecutionLogs  WITH CHECK ADD  CONSTRAINT FK_RuleExecutionLogs_Rules_RuleId FOREIGN KEY (RuleId)
REFERENCES datachecker.Rules (Id)
ON DELETE CASCADE
GO

ALTER TABLE destination.RuleExecutionLogs CHECK CONSTRAINT FK_RuleExecutionLogs_Rules_RuleId
GO
