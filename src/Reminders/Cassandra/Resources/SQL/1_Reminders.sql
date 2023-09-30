CREATE TABLE IF NOT EXISTS reminders (
    type blob,
    id blob,
    hash bigint,
    name varchar,
    start_on timestamp,
    period bigint,
    etag varchar,
    primary key (type, id, name)
);
