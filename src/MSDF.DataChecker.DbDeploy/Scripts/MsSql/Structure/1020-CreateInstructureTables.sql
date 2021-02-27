IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='instructure'
WHERE o.name='Tenant' AND o.type='u')
BEGIN
    CREATE TABLE instructure.Tenant (
        TenantId INT IDENTITY NOT NULL,
        TenantUniqueId  NVARCHAR(256) NOT NULL,
        [Name]  NVARCHAR(256) NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT pk_tenant_TenantId PRIMARY KEY (TenantId),
        CONSTRAINT uc_tenant_TenantUniqueId UNIQUE (TenantUniqueId)
    );
END
GO

IF NOT EXISTS (SELECT 1
FROM sys.objects o
    INNER JOIN sys.schemas s ON s.schema_Id = o.schema_Id AND s.name='instructure'
WHERE o.name='TenantDatabaseConnectionAssociation' AND o.type='u')
BEGIN
    CREATE TABLE instructure.TenantDatabaseConnectionAssociation (
        TenantDatabaseConnectionAssociationId INT IDENTITY NOT NULL,
        TenantId INT NOT NULL,
        DatabaseConnectionId INT NOT NULL,
        Created DATETIME2 DEFAULT getdate(),
        Modified DATETIME2 DEFAULT getdate(),
        CONSTRAINT pk_TenantDatabaseConnectionAssociation_TenantDatabaseConnectionAssociationId PRIMARY KEY (TenantDatabaseConnectionAssociationId)
    );
END
GO
