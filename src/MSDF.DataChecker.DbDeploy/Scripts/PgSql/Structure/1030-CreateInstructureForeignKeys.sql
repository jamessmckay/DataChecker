alter table instructure.tenantdatabaseconnectionassociation drop constraint if exists fk_tenant;
alter table instructure.tenantdatabaseconnectionassociation add constraint fk_tenant foreign key (tenantid)
references instructure.tenant (tenantid);

alter table instructure.tenantdatabaseconnectionassociation drop constraint if exists fk_databaseconnection;
alter table instructure.tenantdatabaseconnectionassociation add constraint fk_databaseconnection foreign key (databaseconnectionid)
references dv_metadata.databaseconnection (databaseconnectionid);
