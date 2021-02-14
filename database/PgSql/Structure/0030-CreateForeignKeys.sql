-- SPDX-License-identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

alter table datachecker.containers  add constraint fk_containerscatalogsruledetailsdestinationid foreign key (ruledetailsdestinationid)
references core.Catalogs (id);

alter table datachecker.containers add constraint fk_containerscontainersparentcontainerid foreign key (parentcontainerid)
references datachecker.containers (id);

alter table datachecker.containers add constraint fk_containerscontainertypescontainertypeid foreign key (containertypeid)
references core.containertypes (id)
on delete cascade;

alter table datachecker.rules add constraint fk_rulescontainerscontainerid foreign key (containerid)
references datachecker.containers (id)
on delete cascade;

alter table datachecker.tagentities add constraint fk_tagentitiescontainerscontainerid foreign key (containerid)
references datachecker.containers (id);

alter table datachecker.tagentities add constraint fk_tagentitiesrulesruleid foreign key (ruleid)
references datachecker.rules (id);

alter table datachecker.tagentities add constraint fk_tagentitiestagstagid foreign key (tagid)
references datachecker.tags (id)
on delete cascade;

alter table datachecker.userparams add constraint fk_userparamsdatabaseenvironmentsdatabaseenvironmentid foreign key (databaseenvironmentid)
references datachecker.databaseenvironments (id)
on delete cascade;

alter table destination.edfiruleexecutionlogdetails add constraint fk_edfiruleexecutionlogdetailsruleexecutionlogid foreign key (ruleexecutionlogid)
references destination.ruleexecutionlogs (id)
on delete cascade;

alter table destination.ruleexecutionlogs add constraint fk_ruleexecutionlogsRulesruleid foreign key (ruleid)
references datachecker.rules (id)
on delete cascade;
