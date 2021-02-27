-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

-- dv_metadata schema
alter table dv_metadata.container drop constraint if exists fk_container_containertype;
alter table dv_metadata.container add constraint fk_container_containertype foreign key (containertypeid)
references dv_enumeration.containertype (containertypeid);

alter table dv_metadata.container drop constraint if exists fk_container_parentcontainer;
alter table dv_metadata.container add constraint fk_container_parentcontainer foreign key (parentcontainerid)
references dv_metadata.container (containerid);

alter table dv_metadata.ruledefinition drop constraint if exists fk_ruledefinition_rule;
alter table dv_metadata.ruledefinition add constraint fk_ruledefinition_rule foreign key (ruleuniqueid)
references dv_metadata.rule (ruleuniqueid);

alter table dv_metadata.ruledefinition drop constraint if exists fk_ruledefinition_errorseverityleveltype;
alter table dv_metadata.ruledefinition add constraint fk_ruledefinition_errorseverityleveltype foreign key (errorseverityleveltypeid)
references dv_enumeration.errorseverityleveltype (errorseverityleveltypeid);

alter table dv_metadata.ruledefinition drop constraint if exists fk_ruledefinition_container;
alter table dv_metadata.ruledefinition add constraint fk_ruledefinition_container foreign key (containerid)
references dv_metadata.container (containerid);

alter table dv_metadata.rulesql drop constraint if exists fk_rulesql_ruledefinition;
alter table dv_metadata.rulesql add constraint fk_rulesql_ruledefinition foreign key (ruledefinitionid)
references dv_metadata.ruledefinition (ruledefinitionid);

alter table dv_metadata.rulesql drop constraint if exists fk_rulesql_databaseenginetype;
alter table dv_metadata.rulesql add constraint fk_rulesql_databaseenginetype foreign key (databaseenginetypeid)
references dv_enumeration.databaseenginetype (databaseenginetypeid);

alter table dv_metadata.rulesql drop constraint if exists fk_rulesql_defaultdiagnosticdatadisplayformattype;
alter table dv_metadata.rulesql add constraint fk_rulesql_defaultdiagnosticdatadisplayformattype foreign key (defaultdiagnosticdatadisplayformattypeid)
references dv_enumeration.diagnosticdatadisplayformattype (diagnosticdatadisplayformattypeid);

alter table dv_metadata.containertagassociation drop constraint if exists fk_containertagassociation_container;
alter table dv_metadata.containertagassociation add constraint fk_containertagassociation_container foreign key (containerid)
references dv_metadata.container (containerid);

alter table dv_metadata.containertagassociation drop constraint if exists fk_containertagassociation_tag;
alter table dv_metadata.containertagassociation add constraint fk_containertagassociation_tag foreign key (tagid)
references dv_metadata.tag (tagid);

alter table dv_metadata.ruletagassociation drop constraint if exists fk_ruletagassociation_tag;
alter table dv_metadata.ruletagassociation add constraint fk_ruletagassociation_tag foreign key (tagid)
references dv_metadata.tag (tagid);

alter table dv_metadata.ruletagassociation drop constraint if exists fk_ruletagassociation_rule;
alter table dv_metadata.ruletagassociation add constraint fk_ruletagassociation_rule foreign key (ruleuniqueid)
references dv_metadata.rule (ruleuniqueid);

alter table dv_metadata.databaseconnection drop constraint if exists fk_databaseconnection_databaseenginetype;
alter table dv_metadata.databaseconnection add constraint fk_databaseconnection_databaseenginetype foreign key (databaseenginetypeid)
references dv_enumeration.databaseenginetype (databaseenginetypeid);

-- dv_snapshot schema
alter table dv_snapshot.container drop constraint if exists fk_container_containertype;
alter table dv_snapshot.container add constraint fk_container_containertype foreign key (containertypeid)
references dv_enumeration.containertype (containertypeid);

alter table dv_snapshot.container drop constraint if exists fk_container_parentcontainer;
alter table dv_snapshot.container add constraint fk_container_parentcontainer foreign key (parentexecutioncontainerid)
references dv_snapshot.container (executioncontainerid);

alter table dv_snapshot.databaseconnection drop constraint if exists fk_databaseconnection_executioncontext;
alter table dv_snapshot.databaseconnection add constraint fk_databaseconnection_executioncontext foreign key (executioncontextid)
references dv_execution.context (executioncontextid);

alter table dv_snapshot.databaseconnection drop constraint if exists fk_databaseconnection_databaseconnection;
alter table dv_snapshot.databaseconnection add constraint fk_databaseconnection_databaseconnection foreign key (databaseconnectionid)
references dv_metadata.databaseconnection (databaseconnectionid);

alter table dv_snapshot.databaseconnection drop constraint if exists fk_databaseconnection_databaseenginetype;
alter table dv_snapshot.databaseconnection add constraint fk_databaseconnection_databaseenginetype foreign key (databaseenginetypeid)
references dv_enumeration.databaseenginetype (databaseenginetypeid);

alter table dv_snapshot.containertagassociation drop constraint if exists fk_containertagassociation_container;
alter table dv_snapshot.containertagassociation add constraint fk_containertagassociation_container foreign key (executioncontainerid)
references dv_snapshot.container (executioncontainerid);

alter table dv_snapshot.containertagassociation drop constraint if exists fk_containertagassociation_tag;
alter table dv_snapshot.containertagassociation add constraint fk_containertagassociation_tag foreign key (executiontagid)
references dv_snapshot.tag (executiontagid);

alter table dv_snapshot.ruledefinitiontagassociation drop constraint if exists fk_ruledefinitiontagassociation_ruledefinition;
alter table dv_snapshot.ruledefinitiontagassociation add constraint fk_ruledefinitiontagassociation_ruledefinition foreign key (ruledefinitionid)
references dv_metadata.ruledefinition (ruledefinitionid);

alter table dv_snapshot.ruledefinitiontagassociation drop constraint if exists fk_ruledefinitiontagassociation_tag;
alter table dv_snapshot.ruledefinitiontagassociation add constraint fk_ruledefinitiontagassociation_tag foreign key (executiontagid)
references dv_snapshot.tag (executiontagid);

alter table dv_snapshot.containerruledefinitionassociation drop constraint if exists fk_containerruledefinitionassociation_container;
alter table dv_snapshot.containerruledefinitionassociation add constraint fk_containerruledefinitionassociation_container foreign key (executioncontainerid)
references dv_snapshot.container (executioncontainerid);

alter table dv_snapshot.containerruledefinitionassociation drop constraint if exists fk_containerruledefinitionassociation_ruledefinition;
alter table dv_snapshot.containerruledefinitionassociation add constraint fk_containerruledefinitionassociation_ruledefinition foreign key (ruledefinitionid)
references dv_metadata.ruledefinition (ruledefinitionid);

-- dv_execution schema

alter table dv_execution.context drop constraint if exists fk_context_databaseconnection;
alter table dv_execution.context add constraint fk_context_databaseconnection foreign key (executiondatabaseconnectionid)
references dv_snapshot.databaseconnection (executiondatabaseconnectionid);

alter table dv_execution.context drop constraint if exists fk_context_container;
alter table dv_execution.context add constraint fk_context_container foreign key (executioncontainerid)
references dv_snapshot.container (executioncontainerid);

alter table dv_execution.contexttagassociation drop constraint if exists fk_contexttagassociation_context;
alter table dv_execution.contexttagassociation add constraint fk_contexttagassociation_context foreign key (executioncontextid)
references dv_execution.context (executioncontextid);

alter table dv_execution.contexttagassociation drop constraint if exists fk_contexttagassociation_tag;
alter table dv_execution.contexttagassociation add constraint fk_contexttagassociation_tag foreign key (executiontagid)
references dv_snapshot.tag (executiontagid);

alter table dv_execution.jobcontextassociation drop constraint if exists fk_jobcontextassociation_context;
alter table dv_execution.jobcontextassociation add constraint fk_jobcontextassociation_context foreign key (executioncontextid)
references dv_execution.context (executioncontextid);

alter table dv_execution.jobcontextassociation drop constraint if exists fk_jobcontextassociation_job;
alter table dv_execution.jobcontextassociation add constraint fk_jobcontextassociation_job foreign key (executionjobid)
references dv_execution.job (executionjobid);

alter table dv_execution.rulelog drop constraint if exists fk_rulelog_context;
alter table dv_execution.rulelog add constraint fk_rulelog_context foreign key (executioncontextid)
references dv_execution.context (executioncontextid);

alter table dv_execution.rulelog drop constraint if exists fk_rulelog_ruledefinition;
alter table dv_execution.rulelog add constraint fk_rulelog_ruledefinition foreign key (ruledefinitionid)
references dv_metadata.ruledefinition (ruledefinitionid);

alter table dv_execution.rulelog drop constraint if exists fk_rulelog_statustype;
alter table dv_execution.rulelog add constraint fk_rulelog_statustype foreign key (statustypeid)
references dv_enumeration.statustype (statustypeid);
