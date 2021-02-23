SET IDENTITY_INSERT core.ContainerTypes ON;
GO

INSERT INTO core.ContainerTypes (id, "name")
SELECT 1, 'Collection'
WHERE NOT EXISTS (SELECT * FROM core.ContainerTypes WHERE id = 1 and "name" = 'Collection');

INSERT INTO core.ContainerTypes (id, "name")
SELECT 2, 'Folder'
WHERE NOT EXISTS (SELECT * FROM core.ContainerTypes WHERE id = 2 and "name" = 'Folder');
GO

SET IDENTITY_INSERT core.ContainerTypes OFF;
GO
