-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

insert into dv_metadata.Tag (name, description, ispublic)
values('Ed-Fi v2.X', 'Ed-Fi Environment v2.X ', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('Ed-Fi v3.X', 'Ed-Fi Environment v3.X ', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('2.5', 'Data Model 2.5', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('2.6', 'Data Model 2.6', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('3.1', 'Data Model 3.1', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('3.2', 'Data Model 3.2', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('3.2-a', 'Data Model 3.2-a', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('3.2-b', 'Data Model 3.2-b', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('3.2-c', 'Data Model 3.2-c', false)
on conflict on constraint uc_tag_name do nothing;

insert into dv_metadata.Tag (name, description, ispublic)
values('3.3-a', 'Data Model 3.3-a', false)
on conflict on constraint uc_tag_name do nothing;
