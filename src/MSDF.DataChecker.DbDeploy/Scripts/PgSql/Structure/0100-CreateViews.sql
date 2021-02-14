-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

drop view if exists destination.vw_ruleexecutionlogdetails;

create view destination.vw_ruleexecutionlogdetails
as
    select
        ruleexecutionlogs.id as ruleexecutionlogsid,
        ruleexecutionlogs.ruleid,
        ruleexecutionlogs.databaseenvironmentid,
        ruleexecutionlogs.response,
        ruleexecutionlogs.statusid,
        ruleexecutionlogs.executiondate,
        ruleexecutionlogs.executiontimems,
        ruleexecutionlogs.executedsql,
        ruleexecutionlogs.ruledetailsdestinationid,
        ruleexecutionlogs.detailsschema,
        rules.name as rulename,
        rules.description as ruledescription,
        rules.errormessage,
        rules.errorseveritylevel,
        rules.resolution,
        rules.diagnosticsql,
        rules.version,
        rules.ruleidentification,
        coalesce(rules.maxnumberresults, databaseenvironments.maxnumberresults) as maxnumberresults,
        databaseenvironments.name as databaseenvironmentname,
        destinations.name as destinationtablename,
        containers.name as containername,
        collections.name as collectionname,
        environments.name as environmentsname,
        rank() over (partition by rules.id order by ruleexecutionlogs.id desc) as rule_order
    from destination.ruleexecutionlogs
    join datachecker.rules on
        rules.id = ruleexecutionlogs.ruleid
    join datachecker.databaseenvironments on
        databaseenvironments.id = ruleexecutionlogs.databaseenvironmentid
    join datachecker.containers on
        rules.containerid = containers.id
    join datachecker.containers as collections on
        containers.parentcontainerid = collections.id
    left join core.catalogs as destinations on
        destinations.id = ruleexecutionlogs.ruledetailsdestinationid
    left join core.catalogs as environments on
        environments.id = collections.environmenttype;
