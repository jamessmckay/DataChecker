IF EXISTS (SELECT name
FROM sys.schemas
WHERE name = N'HangFire')
   BEGIN
    DROP SCHEMA HangFire;
END
GO
CREATE SCHEMA HangFire;
GO
