-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

create table if not exists datachecker.databaseenvironments (
    id uuid not null,
    name varchar(16000) null,
    version int not null,
    database varchar(16000) null,
    "user" varchar(16000) null,
    password varchar(16000) null,
    datasource varchar(16000) null,
    extradata varchar(16000) null,
    maptables varchar(16000) null,
    createddate timestamp not null,
    securityintegrated bool null,
    maxnumberresults int null,
    timeoutinminutes int null,
    constraint pk_databaseenvironments primary key (id)
);

create table if not exists datachecker.containers (
    id uuid not null,
    name varchar(16000) null,
    containertypeid int not null,
    createdbyuserid uuid null,
    parentcontainerid uuid null,
    isdefault bit not null,
    description varchar(16000) null,
    environmenttype int null,
    ruledetailsdestinationid int null,
    constraint pk_containers primary key (id)
);

alter table datachecker.containers alter column id set default gen_random_uuid();

create table if not exists datachecker.rules (
    id uuid not null,
    containerid uuid not null,
    name varchar(16000) null,
    description varchar(16000) null,
    errormessage varchar(16000) null,
    errorseveritylevel int not null,
    resolution varchar(16000) null,
    diagnosticsql varchar(16000) null,
    version varchar(16000) null,
    ruleidentification varchar(255) null,
    maxnumberresults int null,
    constraint pk_rules primary key (id)
);

alter table datachecker.containers alter column id set default gen_random_uuid();

create table if not exists destination.ruleexecutionlogs (
    id serial not null,
    ruleid uuid not null,
    databaseenvironmentid uuid not null,
    response varchar(16000) null,
    result int not null,
    evaluation bit not null,
    statusid int not null,
    executiondate timestamp not null,
    executiontimems bigint not null,
    executedsql varchar(16000) null,
    diagnosticsql varchar(16000) null,
    ruledetailsdestinationid int null,
    detailsschema varchar(16000) null,
    detailstablename varchar(16000) null,
    constraint pk_ruleexecutionlogs primary key (id)
);

create table if not exists core.catalogs (
    id serial not null,
    catalogtype varchar(16000) null,
    name varchar(16000) null,
    description varchar(16000) null,
    updated timestamp not null,
    constraint pk_catalogs primary key (id)
);

create table if not exists core.containertypes (
    id serial not null,
    name varchar(16000) null,
    description varchar(16000) null,
    constraint pk_containertypes primary key (id)
);

create table if not exists core.logs (
    id serial not null,
    information varchar(16000) null,
    source varchar(16000) null,
    datecreated timestamp not null,
    constraint pk_logs primary key (id)
);

create table if not exists datachecker.tagentities (
    id serial not null,
    tagid int not null,
    containerid uuid null,
    ruleid uuid null,
    constraint pk_tagentities primary key (id)
);

create table if not exists datachecker.tags (
    id serial not null,
    name varchar(16000) null,
    description varchar(16000) null,
    ispublic bit not null,
    created timestamp not null,
    updated timestamp not null,
    constraint pk_tags primary key (id)
);

create table if not exists datachecker.userparams (
    id uuid not null,
    name varchar(16000) null,
    value varchar(16000) null,
    databaseenvironmentid uuid not null,
    constraint pk_userparams primary key (id)
);

alter table datachecker.userparams alter column id set default gen_random_uuid();

create table if not exists destination.EdFiRuleExecutionLogDetails(
    id serial not null,
    educationorganizationid int null,
    studentuniqueid varchar(16000) null,
    coursecode varchar(16000) null,
    discriminator varchar(16000) null,
    Programname varchar(16000) null,
    staffuniqueid varchar(16000) null,
    otherdetails varchar(16000) null,
    ruleexecutionlogid int not null,
 constraint pk_edfiruleexecutionlogdetails primary key (id)
);
