-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

insert into core.Catalogs (CatalogType, Name, Description, Updated)
values('EnvironmentType', 'Ed-Fi v2.X', 'Ed-Fi v2.X', now())
on conflict do nothing;

insert into core.Catalogs (CatalogType, Name, Description, Updated)
values('EnvironmentType', 'Ed-Fi v3.X', 'Ed-Fi v3.X', now())
on conflict do nothing;
