insert into core.ContainerTypes (id, name)
overriding system value
values(1, 'Collection')
on conflict on constraint pk_containertypes do nothing;

insert into core.ContainerTypes (id, name)
overriding system value
values(2, 'Folder')
on conflict on constraint pk_containertypes do nothing;

select setval('core.containertypes_id_seq', coalesce((select max(id)+1 FROM core.ContainerTypes), 1), false);
