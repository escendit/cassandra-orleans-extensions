CREATE TYPE IF NOT EXISTS silo (
    address varchar,
    alive_on timestamp,
    role varchar,
    etag varchar,
    fault_zone int,
    host varchar,
    name varchar,
    proxy_port int,
    started_on timestamp,
    status int,
    suspect_times list<frozen<suspect_time>>,
    update_zone int,
    timestamp timestamp
);
