-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

ALTER TABLE datachecker.Containers  WITH CHECK ADD  CONSTRAINT FK_Containers_Catalogs_RuleDetailsDestinationId FOREIGN KEY (RuleDetailsDestinationId)
REFERENCES core.Catalogs (Id)
GO

ALTER TABLE datachecker.Containers CHECK CONSTRAINT FK_Containers_Catalogs_RuleDetailsDestinationId
GO

ALTER TABLE datachecker.Containers  WITH CHECK ADD  CONSTRAINT FK_Containers_Containers_ParentContainerId FOREIGN KEY (ParentContainerId)
REFERENCES datachecker.Containers (Id)
GO

ALTER TABLE datachecker.Containers CHECK CONSTRAINT FK_Containers_Containers_ParentContainerId
GO

ALTER TABLE datachecker.Containers  WITH CHECK ADD  CONSTRAINT FK_Containers_ContainerTypes_ContainerTypeId FOREIGN KEY (ContainerTypeId)
REFERENCES core.ContainerTypes (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.Containers CHECK CONSTRAINT FK_Containers_ContainerTypes_ContainerTypeId
GO

ALTER TABLE datachecker.Rules  WITH CHECK ADD  CONSTRAINT FK_Rules_Containers_ContainerId FOREIGN KEY (ContainerId)
REFERENCES datachecker.Containers (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.Rules CHECK CONSTRAINT FK_Rules_Containers_ContainerId
GO

ALTER TABLE datachecker.TagEntities  WITH CHECK ADD  CONSTRAINT FK_TagEntities_Containers_ContainerId FOREIGN KEY (ContainerId)
REFERENCES datachecker.Containers (Id)
GO
ALTER TABLE datachecker.TagEntities CHECK CONSTRAINT FK_TagEntities_Containers_ContainerId
GO

ALTER TABLE datachecker.TagEntities  WITH CHECK ADD  CONSTRAINT FK_TagEntities_Rules_RuleId FOREIGN KEY (RuleId)
REFERENCES datachecker.Rules (Id)
GO

ALTER TABLE datachecker.TagEntities CHECK CONSTRAINT FK_TagEntities_Rules_RuleId
GO

ALTER TABLE datachecker.TagEntities  WITH CHECK ADD  CONSTRAINT FK_TagEntities_Tags_TagId FOREIGN KEY (TagId)
REFERENCES datachecker.Tags (Id)
ON DELETE CASCADE
GO

ALTER TABLE datachecker.TagEntities CHECK CONSTRAINT FK_TagEntities_Tags_TagId
GO

ALTER TABLE datachecker.UserParams  WITH CHECK ADD  CONSTRAINT FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId FOREIGN KEY (DatabaseEnvironmentId)
REFERENCES datachecker.DatabaseEnvironments (Id)
ON DELETE CASCADE
GO
ALTER TABLE datachecker.UserParams CHECK CONSTRAINT FK_UserParams_DatabaseEnvironments_DatabaseEnvironmentId
GO

ALTER TABLE destination.EdFiRuleExecutionLogDetails  WITH CHECK ADD  CONSTRAINT FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId FOREIGN KEY (RuleExecutionLogId)
REFERENCES destination.RuleExecutionLogs (Id)
ON DELETE CASCADE
GO

ALTER TABLE destination.EdFiRuleExecutionLogDetails CHECK CONSTRAINT FK_EdFiRuleExecutionLogDetails_RuleExecutionLogs_RuleExecutionLogId
GO

ALTER TABLE destination.RuleExecutionLogs  WITH CHECK ADD  CONSTRAINT FK_RuleExecutionLogs_Rules_RuleId FOREIGN KEY (RuleId)
REFERENCES datachecker.Rules (Id)
ON DELETE CASCADE
GO

ALTER TABLE destination.RuleExecutionLogs CHECK CONSTRAINT FK_RuleExecutionLogs_Rules_RuleId
GO
