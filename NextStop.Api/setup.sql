CREATE TABLE holidays (
    id SERIAL PRIMARY KEY,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    name TEXT NOT NULL,
    is_school_holiday BOOLEAN NOT NULL
);

CREATE TABLE stops (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    short_name TEXT NOT NULL UNIQUE,
    latitude DOUBLE PRECISION NOT NULL,
    longitude DOUBLE PRECISION NOT NULL
);

CREATE TABLE routes (
    id SERIAL PRIMARY KEY,
    number TEXT NOT NULL,
    valid_from DATE NOT NULL,
    valid_to DATE NOT NULL,
    days_of_operation TEXT NOT NULL
);

CREATE TABLE route_stops (
    route_id INTEGER NOT NULL REFERENCES routes(id),
    stop_id INTEGER NOT NULL REFERENCES stops(id),
    sequence_number INTEGER NOT NULL,
    scheduled_departure_time TIME NOT NULL,
    PRIMARY KEY (route_id, stop_id),
    UNIQUE (route_id, sequence_number)
);

CREATE TABLE checkins (
    id SERIAL PRIMARY KEY,
    route_id INTEGER NOT NULL REFERENCES routes(id),
    stop_id INTEGER NOT NULL REFERENCES stops(id),
    timestamp TIMESTAMP NOT NULL
);

