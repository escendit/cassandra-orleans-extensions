CREATE TABLE IF NOT EXISTS membership (
    id varchar,
    silos map<varchar, frozen<silo>>,
    version int,
    etag varchar,
    primary key ( id )
);
