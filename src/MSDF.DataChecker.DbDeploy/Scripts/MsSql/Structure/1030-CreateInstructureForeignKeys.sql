IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='TenantDatabaseConnectionAssociation'
    AND CONSTRAINT_SCHEMA='instructure'
    AND CONSTRAINT_NAME='FK_TenantDatabaseConnectionAssociation_Tenant'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE instructure.TenantDatabaseConnectionAssociation DROP CONSTRAINT FK_TenantDatabaseConnectionAssociation_Tenant;
END
GO

ALTER TABLE instructure.TenantDatabaseConnectionAssociation ADD CONSTRAINT FK_TenantDatabaseConnectionAssociation_Tenant FOREIGN KEY (TenantId)
REFERENCES instructure.Tenant (TenantId);
GO

IF EXISTS(SELECT 1
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME='TenantDatabaseConnectionAssociation'
    AND CONSTRAINT_SCHEMA='instructure'
    AND CONSTRAINT_NAME='FK_TenantDatabaseConnectionAssociation_DatabaseConnection'
    AND CONSTRAINT_TYPE='FOREIGN KEY')
BEGIN
    ALTER TABLE instructure.TenantDatabaseConnectionAssociation DROP CONSTRAINT FK_TenantDatabaseConnectionAssociation_DatabaseConnection;
END
GO

ALTER TABLE instructure.TenantDatabaseConnectionAssociation ADD CONSTRAINT FK_TenantDatabaseConnectionAssociation_DatabaseConnection FOREIGN KEY (DatabaseConnectionId)
REFERENCES dv_metadata.DatabaseConnection (DatabaseConnectionId);
GO
