-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

create table if not exists datachecker.databaseenvironments (
    id uuid not null,
    name varchar null,
    version int not null,
    database varchar null,
    "user" varchar null,
    password varchar null,
    datasource varchar null,
    extradata varchar null,
    maptables varchar null,
    createddate timestamp not null,
    securityintegrated bool null,
    maxnumberresults int null,
    timeoutinminutes int null,
    constraint pk_databaseenvironments primary key (id)
);

create table if not exists datachecker.containers (
    id uuid not null,
    name varchar null,
    containertypeid int not null,
    createdbyuserid uuid null,
    parentcontainerid uuid null,
    isdefault bit not null,
    description varchar null,
    environmenttype int null,
    ruledetailsdestinationid int null,
    constraint pk_containers primary key (id)
);

alter table datachecker.containers alter column id set default gen_random_uuid();

create table if not exists datachecker.rules (
    id uuid not null,
    containerid uuid not null,
    name varchar null,
    description varchar null,
    errormessage varchar null,
    errorseveritylevel int not null,
    resolution varchar null,
    diagnosticsql varchar null,
    version varchar null,
    ruleidentification varchar(255) null,
    maxnumberresults int null,
    constraint pk_rules primary key (id),
    constraint uc_rules unique (containerid, ruleidentification)
);

alter table datachecker.containers alter column id set default gen_random_uuid();

create table if not exists destination.ruleexecutionlogs (
    id serial not null,
    ruleid uuid not null,
    databaseenvironmentid uuid not null,
    response varchar null,
    result int not null,
    evaluation bit not null,
    statusid int not null,
    executiondate timestamp not null,
    executiontimems bigint not null,
    executedsql varchar null,
    diagnosticsql varchar null,
    ruledetailsdestinationid int null,
    detailsschema varchar null,
    detailstablename varchar null,
    constraint pk_ruleexecutionlogs primary key (id)
);

create table if not exists core.catalogs (
    id serial not null,
    catalogtype varchar null,
    name varchar null,
    description varchar null,
    updated timestamp not null,
    constraint pk_catalogs primary key (id)
);

create table if not exists core.containertypes (
    id serial not null,
    name varchar null,
    description varchar null,
    constraint pk_containertypes primary key (id)
);

create table if not exists core.logs (
    id serial not null,
    information varchar null,
    source varchar null,
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
    name varchar null,
    description varchar null,
    ispublic bit not null,
    created timestamp not null,
    updated timestamp not null,
    constraint pk_tags primary key (id)
);

create table if not exists datachecker.userparams (
    id uuid not null,
    name varchar null,
    value varchar null,
    databaseenvironmentid uuid not null,
    constraint pk_userparams primary key (id)
);

alter table datachecker.userparams alter column id set default gen_random_uuid();

create table if not exists destination.EdFiRuleExecutionLogDetails(
    id serial not null,
    educationorganizationid int null,
    studentuniqueid varchar null,
    coursecode varchar null,
    discriminator varchar null,
    Programname varchar null,
    staffuniqueid varchar null,
    otherdetails varchar null,
    ruleexecutionlogid int not null,
 constraint pk_edfiruleexecutionlogdetails primary key (id)
);
