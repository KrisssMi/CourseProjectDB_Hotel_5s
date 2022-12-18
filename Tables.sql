ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

-------------------------ROLE-------------------------

CREATE TABLE role_table (
    role_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    role_name NVARCHAR2(50) NOT NULL,
    CONSTRAINT role_pk PRIMARY KEY (role_id)
);

SELECT * FROM role_table;

INSERT INTO role_table(role_name) VALUES('Administrator');
INSERT INTO role_table(role_name) VALUES('Staff');
INSERT INTO role_table(role_name) VALUES('Guest');
COMMIT;

DROP TABLE role_table;

-------------------------ALBUM-------------------------

CREATE TABLE album_table (
    album_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    album_name NVARCHAR2(50) NOT NULL,
    album_object NUMBER(10) NOT NULL,
    CONSTRAINT album_pk PRIMARY KEY (album_id)
);
--album_object:
--0 - person
--1 - room
--2 - inventory

SELECT * FROM album_table;

DROP TABLE album_table;

-------------------------PHOTO-------------------------

CREATE TABLE photo_table (
    photo_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    photo_album_id NUMBER(10) NOT NULL,
    photo_source BLOB DEFAULT EMPTY_BLOB(),
    CONSTRAINT photo_pk PRIMARY KEY (photo_id),
    CONSTRAINT photo_album_fk FOREIGN KEY (photo_album_id) REFERENCES album_table(album_id)
);

SELECT * FROM photo_table;

DROP TABLE photo_table;

-------------------------PERSON-------------------------

CREATE TABLE person_table (
    person_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    person_role_id NUMBER(10) DEFAULT 3 NOT NULL,
    person_album_id NUMBER(10) NOT NULL,
    person_email NVARCHAR2(50) NOT NULL,
    person_password NVARCHAR2(50) NOT NULL,
    person_first_name NVARCHAR2(50) NOT NULL,
    person_last_name NVARCHAR2(50) NOT NULL,
    person_father_name NVARCHAR2(50) NOT NULL,
    CONSTRAINT person_pk PRIMARY KEY (person_id),
    CONSTRAINT person_role_fk FOREIGN KEY (person_role_id) REFERENCES role_table(role_id),
    CONSTRAINT person_album_fk FOREIGN KEY (person_album_id) REFERENCES album_table(album_id)
);

SELECT * FROM person_table;

UPDATE person_table SET person_role_id = 1 WHERE person_id = 1;
COMMIT;

DROP TABLE person_table;

-------------------------ROOM_TYPE-------------------------

CREATE TABLE room_type_table (
    room_type_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    room_type_name NVARCHAR2(50) NOT NULL,
    room_type_capacity NUMBER(10) NOT NULL,
    room_type_daily_price FLOAT(10) NOT NULL,
    CONSTRAINT room_type_pk PRIMARY KEY (room_type_id)
);

INSERT INTO room_type_table(room_type_name, room_type_capacity, room_type_daily_price) VALUES('One-person', 1, 25.00);
INSERT INTO room_type_table(room_type_name, room_type_capacity, room_type_daily_price) VALUES('Two-person', 2, 40.00);
INSERT INTO room_type_table(room_type_name, room_type_capacity, room_type_daily_price) VALUES('Three-person', 3, 55.00);
COMMIT;

SELECT * FROM room_type_table;

DROP TABLE room_type_table;

-------------------------ROOM-------------------------

CREATE TABLE room_table (
    room_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    room_room_type_id NUMBER(10) NOT NULL,
    room_album_id NUMBER(10) NOT NULL,
    room_number NVARCHAR2(50) NOT NULL,
    room_description NVARCHAR2(200) NOT NULL,
    CONSTRAINT room_pk PRIMARY KEY (room_id),
    CONSTRAINT room_room_type_fk FOREIGN KEY (room_room_type_id) REFERENCES room_type_table(room_type_id),
    CONSTRAINT room_album_fk FOREIGN KEY (room_album_id) REFERENCES album_table(album_id)
);

SELECT * FROM room_table;

INSERT INTO room_table(room_room_type_id, room_album_id, room_number, room_description) VALUES(1, 1, '101', '101 room number, one-person room type');
COMMIT;

DROP TABLE room_table;

-------------------------INVENTORY_TYPE-------------------------

CREATE TABLE inventory_type_table (
    inventory_type_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    inventory_type_name NVARCHAR2(50) NOT NULL,
    CONSTRAINT inventory_type_pk PRIMARY KEY (inventory_type_id)
);

SELECT * FROM inventory_type_table;

INSERT INTO inventory_type_table(inventory_type_name) VALUES('Iron');
INSERT INTO inventory_type_table(inventory_type_name) VALUES('Microwave');
INSERT INTO inventory_type_table(inventory_type_name) VALUES('Fridge');
COMMIT;

DROP TABLE inventory_type_table;

-------------------------INVENTORY-------------------------

CREATE TABLE inventory_table (
    inventory_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    inventory_inventory_type_id NUMBER(10) NOT NULL,
    inventory_album_id NUMBER(10) NOT NULL,
    inventory_description NVARCHAR2(200) NOT NULL,
    inventory_daily_price FLOAT(10) NOT NULL,
    CONSTRAINT inventory_pk PRIMARY KEY (inventory_id),
    CONSTRAINT inventory_inventory_type_fk FOREIGN KEY (inventory_inventory_type_id) REFERENCES inventory_type_table(inventory_type_id),
    CONSTRAINT inventory_album_fk FOREIGN KEY (inventory_album_id) REFERENCES album_table(album_id)
);

SELECT * FROM inventory_table;

DROP TABLE inventory_table;

-------------------------SERVICE_TYPE-------------------------

CREATE TABLE service_type_table (
    service_type_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    service_type_name NVARCHAR2(50) NOT NULL,
    service_type_daily_price FLOAT(10) NOT NULL,
    CONSTRAINT service_type_pk PRIMARY KEY (service_type_id)
);

SELECT * FROM service_type_table;

INSERT INTO service_type_table(service_type_name, service_type_daily_price) VALUES('Cleaning', 2.50);
INSERT INTO service_type_table(service_type_name, service_type_daily_price) VALUES('Feeding', 7.50);
COMMIT;

DROP TABLE service_type_table;

-------------------------SERVICE-------------------------

CREATE TABLE service_table (
    service_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    service_service_type_id NUMBER(10) NOT NULL,
    service_person_id NUMBER(10) NOT NULL,
    CONSTRAINT service_pk PRIMARY KEY (service_id),
    CONSTRAINT service_service_type_fk FOREIGN KEY (service_service_type_id) REFERENCES service_type_table(service_type_id),
    CONSTRAINT service_person_fk FOREIGN KEY (service_person_id) REFERENCES person_table(person_id)
);

SELECT * FROM service_table;

DROP TABLE service_table;

-------------------------BOOKING-------------------------

CREATE TABLE booking_table (
    booking_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    booking_room_id NUMBER(10) NOT NULL,
    booking_start_date DATE NOT NULL,
    booking_end_date DATE NOT NULL,
    booking_state NUMBER(1) DEFAULT 0,
    CONSTRAINT booking_pk PRIMARY KEY (booking_id),
    CONSTRAINT booking_room_fk FOREIGN KEY (booking_room_id) REFERENCES room_table(room_id)
);
--booking_state:
--0 - booked by guest
--1 - approved by the administrator

SELECT * FROM booking_table;

DROP TABLE booking_table;

-------------------------RESIDENT-------------------------

CREATE TABLE resident_table (
    resident_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    resident_person_id NUMBER(10) NOT NULL,
    resident_booking_id NUMBER(10) NOT NULL,
    CONSTRAINT resident_pk PRIMARY KEY (resident_id),
    CONSTRAINT resident_person_fk FOREIGN KEY (resident_person_id) REFERENCES person_table(person_id),
    CONSTRAINT resident_booking_fk FOREIGN KEY (resident_booking_id) REFERENCES booking_table(booking_id)
);

SELECT * FROM resident_table;

DROP TABLE resident_table;

-------------------------RENT-------------------------

CREATE TABLE rent_table (
    rent_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    rent_inventory_id NUMBER(10) NOT NULL,
    rent_booking_id NUMBER(10) NOT NULL,
    rent_start_date DATE NOT NULL,
    rent_end_date DATE NOT NULL,
    CONSTRAINT rent_pk PRIMARY KEY (rent_id),
    CONSTRAINT rent_inventory_fk FOREIGN KEY (rent_inventory_id) REFERENCES inventory_table(inventory_id),
    CONSTRAINT rent_booking_fk FOREIGN KEY (rent_booking_id) REFERENCES booking_table(booking_id)
);

SELECT * FROM rent_table;

DROP TABLE rent_table;

-------------------------SUBSCRIPTION-------------------------

CREATE TABLE subscription_table (
    subscription_id NUMBER(10) GENERATED AS IDENTITY(START WITH 1 INCREMENT BY 1),
    subscription_service_id NUMBER(10) NOT NULL,
    subscription_booking_id NUMBER(10) NOT NULL,
    subscription_start_date DATE NOT NULL,
    subscription_end_date DATE NOT NULL,
    CONSTRAINT subscription_pk PRIMARY KEY (subscription_id),
    CONSTRAINT subscription_service_fk FOREIGN KEY (subscription_service_id) REFERENCES service_table(service_id),
    CONSTRAINT subscription_booking_fk FOREIGN KEY (subscription_booking_id) REFERENCES booking_table(booking_id)
);

SELECT * FROM subscription_table;

DROP TABLE subscription_table;