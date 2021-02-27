IF NOT EXISTS(SELECT 1
FROM sys.schemas
WHERE name = N'instructure')
BEGIN
    EXEC('CREATE SCHEMA instructure');
END
GO
