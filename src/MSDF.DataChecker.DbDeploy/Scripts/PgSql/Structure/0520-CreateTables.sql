-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

-- dv_enumeration schema
create table if not exists dv_enumeration.statustype
(
    statustypeid serial not null,
    name varchar(256) not null,
    description varchar(512),
    created timestamp,
    modified timestamp,
    constraint pk_statustype_statustypeid primary key (statustypeid),
    constraint uc_statustype_name unique (name)
);

alter table dv_enumeration.statustype alter column created set default current_timestamp;
alter table dv_enumeration.statustype alter column modified set default current_timestamp;

create table if not exists dv_enumeration.databaseenginetype
(
    databaseenginetypeid serial not null,
    name varchar(256) not null,
    description  varchar(512),
    created timestamp,
    modified timestamp,
    constraint pk_databaseenginetype_databaseenginetypeid primary key (databaseenginetypeid),
    constraint uc_databaseenginetype_name unique (name)
);

alter table dv_enumeration.databaseenginetype alter column created set default current_timestamp;
alter table dv_enumeration.databaseenginetype alter column modified set default current_timestamp;

create table if not exists dv_enumeration.diagnosticdatadisplayformattype
(
    diagnosticdatadisplayformattypeid serial not null,
    name varchar(256) not null,
    description  varchar(512),
    created  timestamp,
    modified  timestamp,
    constraint pk_diagnosticdatadisplayformattype_id primary key (diagnosticdatadisplayformattypeid),
    constraint uc_diagnosticdatadisplayformattype_name unique (name)
);

alter table dv_enumeration.diagnosticdatadisplayformattype alter column created set default current_timestamp;
alter table dv_enumeration.diagnosticdatadisplayformattype alter column modified set default current_timestamp;

create table if not exists dv_enumeration.errorseverityleveltype
(
    errorseverityleveltypeid  serial not null,
    name varchar(256) not null,
    description varchar(512),
    created  timestamp,
    modified  timestamp,
    constraint pk_errorseverityleveltype_errorseverityleveltypeid primary key (errorseverityleveltypeid),
    constraint uc_errorseverityleveltype_name unique (name)
);

alter table dv_enumeration.errorseverityleveltype alter column created set default current_timestamp;
alter table dv_enumeration.errorseverityleveltype alter column modified set default current_timestamp;

create table if not exists dv_enumeration.containertype
(
    containertypeid  serial not null,
    name varchar(256) not null,
    description  varchar(512),
    created  timestamp,
    modified  timestamp,
    constraint pk_containertype_containertypeid primary key (containertypeid),
    constraint uc_containertype_name unique (name)
);

alter table dv_enumeration.containertype alter column created set default current_timestamp;
alter table dv_enumeration.containertype alter column modified set default current_timestamp;

-- dv_metadata schema
create table if not exists dv_metadata.container
(
    containerid uuid not null,
    name varchar (256) not null,
    containertypeid int not null,
    parentcontainerid uuid,
    isdefault bit,
    description varchar(512),
    created timestamp,
    modified timestamp,
    constraint pk_container_containerid primary key (containerid),
    constraint uc_container_name unique (name)
);

alter table dv_metadata.container alter column containerid set default gen_random_uuid();
alter table dv_metadata.container alter column created set default current_timestamp;
alter table dv_metadata.container alter column modified set default current_timestamp;

create table if not exists dv_metadata.rule
(
    ruleuniqueid uuid not null,
    name varchar(512) not null,
    description varchar,
    semanticversion varchar(128),
    created timestamp,
    modified timestamp,
    constraint pk_rule_ruleuniqueid primary key (ruleuniqueid),
    constraint uc_rule_name unique (name)
);

alter table dv_metadata.rule alter column ruleuniqueid set default gen_random_uuid();
alter table dv_metadata.rule alter column created set default current_timestamp;
alter table dv_metadata.rule alter column modified set default current_timestamp;

create table if not exists dv_metadata.ruledefinition
(
    ruledefinitionid serial not null,
    ruleuniqueid uuid not null,
    version serial not null,
    errorseverityleveltypeid int not null,
    containerid uuid not null,
    errormessage varchar,
    resolution varchar,
    maxnumberresults int,
    created timestamp,
    modified timestamp,
    constraint pk_ruledefinition_ruledefinitionid primary key (ruledefinitionid),
    constraint uc_ruledefinition_ruleuniqueid unique (ruleuniqueid)
);

alter table dv_metadata.ruledefinition alter column ruleuniqueid set default gen_random_uuid();
alter table dv_metadata.ruledefinition alter column created set default current_timestamp;
alter table dv_metadata.ruledefinition alter column modified set default current_timestamp;

create table if not exists dv_metadata.rulesql
(
    rulesqlid serial not null,
    ruledefinitionid int not null,
    databaseenginetypeid int not null,
    defaultdiagnosticdatadisplayformattypeid int not null,
    rulesql varchar not null,
    diagnosticsql varchar not null,
    created timestamp,
    modified timestamp,
    constraint pk_rulesql_rulesqlid primary key (rulesqlid),
    constraint uc_rulesql_ruledefinitionid_databaseenginetypeid unique (ruledefinitionid, databaseenginetypeid)
);

alter table dv_metadata.rulesql alter column created set default current_timestamp;
alter table dv_metadata.rulesql alter column modified set default current_timestamp;

create table if not exists dv_metadata.tag
(
    tagid serial not null,
    name varchar(256) not null,
    description varchar(512),
    ispublic boolean,
    created timestamp,
    modified timestamp,
    constraint pk_tag_tagid primary key (tagid),
    constraint uc_tag_name unique (name)
);

alter table dv_metadata.tag alter column ispublic set default false;
alter table dv_metadata.tag alter column created set default current_timestamp;
alter table dv_metadata.tag alter column modified set default current_timestamp;

create table if not exists dv_metadata.containertagassociation
(
    containertagassociationid serial not null,
    containerid uuid not null,
    tagid int not null,
    created timestamp,
    modified timestamp,
    constraint pk_containertagassociation_containertagassociationid primary key (containertagassociationid)
);

alter table dv_metadata.containertagassociation alter column created set default current_timestamp;
alter table dv_metadata.containertagassociation alter column modified set default current_timestamp;

create table if not exists dv_metadata.ruletagassociation
(
    ruletagassociationid serial not null,
    ruleuniqueid uuid not null,
    tagid int not null,
    created timestamp,
    modified timestamp,
    constraint pk_ruletagassociation_ruletagassociationid primary key (ruletagassociationid)
);

alter table dv_metadata.ruletagassociation alter column created set default current_timestamp;
alter table dv_metadata.ruletagassociation alter column modified set default current_timestamp;

create table if not exists dv_metadata.databaseconnection
(
    databaseconnectionid serial not null,
    name varchar(256) not null,
    connectionstring varchar not null,
    databaseenginetypeid int not null,
    created timestamp,
    modified timestamp,
    constraint pk_databaseconnection_databaseconnectionid primary key (databaseconnectionid),
    constraint uc_databaseconnection_name unique (name)
);

alter table dv_metadata.databaseconnection alter column created set default current_timestamp;
alter table dv_metadata.databaseconnection alter column modified set default current_timestamp;

-- dv_snapshot schema
create table if not exists dv_snapshot.container
(
    executioncontainerid serial not null,
    containerid uuid not null,
    containertypeid int not null,
    parentexecutioncontainerid int,
    name varchar(256),
    description  varchar(512),
    constraint pk_container_executioncontainerid primary key (executioncontainerid)
);

create table if not exists dv_snapshot.databaseconnection
(
    executiondatabaseconnectionid serial not null,
    executioncontextid int not null,
    databaseconnectionid int not null,
    name varchar(256) not null,
    connectionstring varchar not null,
    databaseenginetypeid int not null,
    created timestamp,
    modified timestamp,
    constraint pk_databaseconnection_executiondatabaseconnectionid primary key (executiondatabaseconnectionid),
    constraint uc_databaseconnection_executioncontextid unique (executioncontextid)
);

alter table dv_snapshot.databaseconnection alter column created set default current_timestamp;
alter table dv_snapshot.databaseconnection alter column modified set default current_timestamp;

create table if not exists dv_snapshot.containertagassociation
(
    executioncontainertagassociationid serial not null,
    executioncontainerid int not null,
    executiontagid int not null,
    constraint pk_containertagassociation_executioncontainertagassociationid primary key (executioncontainertagassociationid)
);

create table if not exists dv_snapshot.containerruledefinitionassociation
(
   executioncontainerruledefinitionassociationid serial not null,
   ruledefinitionid int not null,
   executioncontainerid int not null,
   constraint pk_executioncontainerruledefinitionassociationid primary key (executioncontainerruledefinitionassociationid)
);

create table if not exists dv_snapshot.ruledefinitiontagassociation
(
    executionruletagassociationid serial not null,
    ruledefinitionid int not null,
    executiontagid int not null,
    constraint pk_ruletagassociation_executionruletagassociationid primary key (executionruletagassociationid)
);

create table if not exists dv_snapshot.tag
(
    executiontagid serial not null,
    tagid int not null,
    name varchar(256),
    description varchar(512),
    ispublic boolean,
    constraint pk_tag_executiontagid primary key (executiontagid),
    constraint uc_tag_tagid unique(tagid)
);

alter table dv_snapshot.tag alter column ispublic set default false;

-- dv_execution schema
create table if not exists dv_execution.context
(
    executioncontextid serial not null,
    executiondatabaseconnectionid int not null,
    executioncontainerid int not null,
    activefrom timestamp with time zone,
    activeto timestamp with time zone,
    created timestamp,
    modified timestamp,
    constraint pk_context_executioncontextid primary key (executioncontextid)
);

alter table dv_execution.context alter column created set default current_timestamp;
alter table dv_execution.context alter column modified set default current_timestamp;

create table if not exists dv_execution.contexttagassociation
(
    executioncontexttagassociationid serial not null,
    executioncontextid int not null,
    executiontagid int not null,
    constraint pk_contexttagassociation_executioncontexttagassociationid primary key (executioncontexttagassociationid)
);

create table if not exists dv_execution.job
(
    executionjobid serial not null,
    name varchar(256) not null,
    tenantid int,
    cron varchar(100),
    created timestamp,
    modified timestamp,
    constraint pk_job_executionjobid primary key (executionjobid)
);

alter table dv_execution.job alter column tenantid set default 0;
alter table dv_execution.job alter column created set default current_timestamp;
alter table dv_execution.job alter column modified set default current_timestamp;

create table if not exists dv_execution.jobcontextassociation
(
    executionjobcontextassociationid serial not null,
    executioncontextid int not null,
    executionjobid int not null,
    constraint pk_jobcontextassociation_executionjobcontextassociationid primary key (executionjobcontextassociationid),
    constraint uc_jobcontextassociation_executioncontextid unique (executioncontextid)
);

create table if not exists dv_execution.rulelog
(
    executionrulelogid serial not null,
    executioncontextid int not null,
    ruledefinitionid int not null,
    statustypeid int not null,
    result int,
    evaluation boolean,
    executiondate timestamp with time zone,
    executiontimems bigint,
    response varchar,
    diagnosticdata json,
    created timestamp,
    modified timestamp,
     constraint pk_rulelog_executionrulelogid primary key (executionrulelogid)
);

alter table dv_execution.rulelog alter column created set default current_timestamp;
alter table dv_execution.rulelog alter column modified set default current_timestamp;
