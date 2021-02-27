-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

insert into dv_enumeration.statustype (name, description)
values('Succeeded', 'Succeeded')
on conflict on constraint uc_statustype_name do nothing;

insert into dv_enumeration.statustype (name, description)
values('Failed', 'Failed')
on conflict  on constraint uc_statustype_name do nothing;

insert into dv_enumeration.statustype (name, description)
values('Error', 'Error')
on conflict on constraint uc_statustype_name do nothing;

insert into dv_enumeration.databaseenginetype (name, description)
values('sqlServer', 'Sql Server')
on conflict on constraint uc_databaseenginetype_name do nothing;

insert into dv_enumeration.databaseenginetype (name, description)
values('postgreSql', 'PostgreSQL')
on conflict on constraint uc_databaseenginetype_name do nothing;

insert into dv_enumeration.diagnosticdatadisplayformattype (name, description)
values ('raw', 'Raw JSON')
on conflict on constraint uc_diagnosticdatadisplayformattype_name do nothing;

insert into dv_enumeration.ErrorSeverityLevelType (name, description)
values ('debug', 'Debug')
on conflict on constraint uc_errorseverityleveltype_name do nothing;

insert into dv_enumeration.ErrorSeverityLevelType (name, description)
values ('info', 'Information')
on conflict on constraint uc_errorseverityleveltype_name do nothing;

insert into dv_enumeration.ErrorSeverityLevelType (name, description)
values ('warn', 'Warning')
on conflict on constraint uc_errorseverityleveltype_name do nothing;

insert into dv_enumeration.ErrorSeverityLevelType (name, description)
values ('fatal', 'Fatal')
on conflict on constraint uc_errorseverityleveltype_name do nothing;

insert into dv_enumeration.ContainerType (name, description)
values ('Collection', 'Collection')
on conflict on constraint uc_containertype_name do nothing;

insert into dv_enumeration.ContainerType (name, description)
values ('Folder', 'Folder')
on conflict on constraint uc_containertype_name do nothing;
