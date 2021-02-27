create table if not exists instructure.tenant (
    tenantid serial not null,
    tenantuniqueid  varchar(256) not null,
    name  varchar(256) not null,
    created timestamp,
    modified timestamp,
    constraint pk_tenant_tenantid primary key (tenantid),
    constraint uc_tenant_tenantuniqueid unique (tenantuniqueid)
);

alter table instructure.tenant alter column created set default current_timestamp;
alter table instructure.tenant alter column modified set default current_timestamp;

create table if not exists instructure.tenantdatabaseconnectionassociation (
    tenantdatabaseconnectionassociationid  serial not null,
    tenantid int not null,
    databaseconnectionid int not null,
    created timestamp,
    modified timestamp,
    constraint pk_tenantdatabaseconnectionassociation_tenantdatabaseconnectionassociationid primary key (tenantdatabaseconnectionassociationid)
);

alter table instructure.tenantdatabaseconnectionassociation alter column created set default current_timestamp;
alter table instructure.tenantdatabaseconnectionassociation alter column modified set default current_timestamp;
