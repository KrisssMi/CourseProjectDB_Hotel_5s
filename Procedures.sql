ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

-------------------------CREATE_ALBUM-------------------------

CREATE OR REPLACE PROCEDURE create_album
    (
        p_album_name IN album_table.album_name%TYPE,
        p_album_object IN album_table.album_object%TYPE,
        o_album_id OUT album_table.album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE UPPER(album_name) = UPPER(p_album_name) AND album_object = p_album_object;
    IF (cnt = 0) THEN
        INSERT INTO album_table(album_name, album_object) VALUES(p_album_name, p_album_object);
        COMMIT;
        SELECT COUNT(*) INTO cnt FROM album_table WHERE UPPER(album_name) = UPPER(p_album_name) AND album_object = p_album_object;
        IF (cnt != 0) THEN
            SELECT album_id INTO o_album_id FROM album_table WHERE UPPER(album_name) = UPPER(p_album_name) AND album_object = p_album_object;
        ELSE
            RAISE_APPLICATION_ERROR(-20063, 'Album commit error');
        END IF;
    END IF;
END create_album;

DECLARE
    album_id NUMBER(10);
BEGIN
    create_album('101', album_id);
    dbms_output.put_line('Album id - ' || album_id);
END;

-------------------------UPDATE_ALBUM-------------------------

CREATE OR REPLACE PROCEDURE update_album
    (
        p_album_id IN album_table.album_id%TYPE,
        p_album_name IN album_table.album_name%TYPE,
        p_album_object IN album_table.album_object%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_album_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM album_table WHERE UPPER(album_name) = UPPER(p_album_name) AND album_object = p_album_object AND album_id != p_album_id;
        IF (cnt = 0) THEN
            UPDATE album_table SET album_name = p_album_name, album_object = p_album_object WHERE album_id = p_album_id;
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20062, 'This album already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
    END IF;
END update_album;

--Successful update
CALL update_album(1, 'kristina.minevich@mail.ru', 0);

--Updating a non-existent album. Error simulation
CALL update_album(404, 'kristina.minevich@mail.ru', 0);

--Updating a non-existent album. Error simulation
CALL update_album(3, 'cleaning1@mail.ru', 1);

DROP PROCEDURE update_album;

-------------------------DELETE_ALBUM-------------------------

CREATE OR REPLACE PROCEDURE delete_album
    (
        p_album_id IN album_table.album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_album_id;
    IF (cnt != 0) THEN
        DELETE FROM album_table WHERE album_id = p_album_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
    END IF;
END delete_album;

--Successful delete
CALL delete_album(25);

--Deleting a non-existent album. Error simulation
CALL delete_album(404);

DROP PROCEDURE delete_album;

-------------------------CREATE_PHOTO-------------------------

CREATE OR REPLACE PROCEDURE create_photo
    ( 
        p_photo_album_id IN photo_table.photo_album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_photo_album_id;
    IF (cnt != 0) THEN
        INSERT INTO photo_table(photo_album_id) VALUES(p_photo_album_id);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
    END IF;
END create_photo;

--Successful create
CALL create_photo(4);

--Deleting a non-existent album. Error simulation
CALL create_photo(404);

DROP PROCEDURE create_photo;

-------------------------UPDATE_PHOTO-------------------------

CREATE OR REPLACE PROCEDURE update_photo
    (
        p_photo_id IN photo_table.photo_id%TYPE,
        p_photo_album_id IN photo_table.photo_album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM photo_table WHERE photo_id = p_photo_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_photo_album_id;
        IF (cnt != 0) THEN
            UPDATE photo_table SET photo_album_id = p_photo_album_id WHERE photo_id = p_photo_id;
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20066, 'Photo is not found');
    END IF;
END update_photo;

--Successful update
CALL update_photo(1, 2);

--Updating a non-existent photo. Error simulation
CALL update_photo(404, 2);

--Updating a non-existent album. Error simulation
CALL update_photo(1, 404);

DROP PROCEDURE update_photo;

-------------------------UPDATE_PHOTO_SOURCE-------------------------

CREATE OR REPLACE PROCEDURE update_photo_source
    (
        p_photo_id IN photo_table.photo_id%TYPE,
        p_source_name IN NVARCHAR2
    )
IS
    src_file BFILE;
    dst_file BLOB;
    lgh_file BINARY_INTEGER;
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM photo_table WHERE photo_id = p_photo_id;
    IF (cnt != 0) THEN
        src_file := bfilename('DATA_PUMP_DIR', p_source_name);
        SELECT photo_source INTO dst_file FROM photo_table WHERE photo_id = p_photo_id FOR UPDATE;
        DBMS_LOB.FILEOPEN(src_file, DBMS_LOB.FILE_READONLY);
        lgh_file := DBMS_LOB.GETLENGTH(src_file);
        DBMS_LOB.LOADFROMFILE(dst_file, src_file, lgh_file);
        UPDATE photo_table SET photo_source = dst_file WHERE photo_id = p_photo_id;
        DBMS_LOB.FILECLOSE(src_file);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20066, 'Photo is not found');
    END IF;
END update_photo_source;

--Successful update
CALL update_photo_source(3, '3.png');

--Updating a non-existent photo. Error simulation
CALL update_photo_source(404, '1.png');

DROP PROCEDURE update_photo_source;

SELECT * FROM photo_table;

-------------------------DELETE_PHOTO-------------------------

CREATE OR REPLACE PROCEDURE delete_photo
    (
        p_photo_id IN photo_table.photo_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM photo_table WHERE photo_id = p_photo_id;
    IF (cnt != 0) THEN
        DELETE FROM photo_table WHERE photo_id = p_photo_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20066, 'Photo is not found');
    END IF;
END delete_photo;

--Successful delete
CALL delete_photo(1);

--Deleting a non-existent photo. Error simulation
CALL delete_photo(404);

DROP PROCEDURE delete_photo;

-------------------------REGISTER_PERSON-------------------------

CREATE OR REPLACE PROCEDURE register_person
    ( 
        p_person_album_id IN person_table.person_album_id%TYPE,
        p_person_email IN person_table.person_email%TYPE,
        p_person_password IN person_table.person_password%TYPE,
        p_person_first_name IN person_table.person_first_name%TYPE,
        p_person_last_name IN person_table.person_last_name%TYPE,
        p_person_father_name IN person_table.person_father_name%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM person_table WHERE UPPER(person_email) = UPPER(p_person_email);
    IF (cnt = 0) THEN
        INSERT INTO person_table(person_album_id, person_email, person_password, person_first_name, person_last_name, person_father_name) 
            VALUES(p_person_album_id, p_person_email, encryption_password(p_person_password), p_person_first_name, p_person_last_name, p_person_father_name);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20007, 'This person already exists');
    END IF;
END register_person;

--First launch - successful registration. Second - error simulation
CALL register_person('eugenia.blinova@mail.ru', 'Pa$$w0rd', 'Eugenia', 'Blinova', 'Alexandrovna');

DROP PROCEDURE register_person;

-------------------------LOG_IN_PERSON-------------------------

CREATE OR REPLACE PROCEDURE log_in_person
    (
        p_person_email IN person_table.person_email%TYPE,
        p_person_password IN person_view.person_password%TYPE,
        o_person_id OUT person_table.person_id%TYPE,
        o_person_album_id OUT person_table.person_album_id%TYPE,
        o_person_email OUT person_table.person_email%TYPE,
        o_person_password OUT person_view.person_password%TYPE,
        o_person_first_name OUT person_view.person_first_name%TYPE,
        o_person_last_name OUT person_view.person_last_name%TYPE,
        o_person_father_name OUT person_view.person_father_name%TYPE,
        o_role_name OUT person_view.role_name%TYPE
    )
IS
    CURSOR person_cursor IS SELECT person_id, album_id, person_email, p_person_password, person_first_name, person_last_name, person_father_name, role_name FROM person_view WHERE UPPER(person_email) = UPPER(p_person_email) AND person_password = encryption_password(p_person_password);
BEGIN
    OPEN person_cursor;
    FETCH person_cursor INTO o_person_id, o_person_album_id, o_person_email, o_person_password, o_person_first_name, o_person_last_name, o_person_father_name, o_role_name;
    IF person_cursor%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20008, 'Login/Password error');
    END IF;
    CLOSE person_cursor;
END log_in_person;

--Authorization with valid data
DECLARE
    person_email NVARCHAR2(50);
    person_password NVARCHAR2(50);
    person_first_name NVARCHAR2(50);
    person_last_name NVARCHAR2(50);
    person_father_name NVARCHAR2(50);
    role_name NVARCHAR2(50);
BEGIN
    log_in_person('yury.karlenok@mail.ru', 'Pa$$w0rd', person_email, person_password, person_first_name, person_last_name, person_father_name, role_name);
    dbms_output.put_line('Person email - ' || person_email);
    dbms_output.put_line('Person password - ' || person_password);
    dbms_output.put_line('Person first name - ' || person_first_name);
    dbms_output.put_line('Person last name - ' || person_last_name);
    dbms_output.put_line('Person father name - ' || person_father_name);
    dbms_output.put_line('Role name - ' || role_name);
END;

--Authorization with invalid data. Error simulation
DECLARE
    person_email NVARCHAR2(50);
    person_password NVARCHAR2(50);
    person_first_name NVARCHAR2(50);
    person_last_name NVARCHAR2(50);
    person_father_name NVARCHAR2(50);
    role_name NVARCHAR2(50);
BEGIN
    log_in_person('eugenia.blinova@mail.ru', '1234', person_email, person_password, person_first_name, person_last_name, person_father_name, role_name);
    dbms_output.put_line('Person email - ' || person_email);
    dbms_output.put_line('Person password - ' || person_password);
    dbms_output.put_line('Person first name - ' || person_first_name);
    dbms_output.put_line('Person last name - ' || person_last_name);
    dbms_output.put_line('Person father name - ' || person_father_name);
    dbms_output.put_line('Role name - ' || role_name);
END;

DROP PROCEDURE log_in_person;

-------------------------SEARCH_PERSON-------------------------

CREATE OR REPLACE PROCEDURE search_person
    (
        p_person_email IN person_table.person_email%TYPE,
        o_person_id OUT person_table.person_id%TYPE,
        o_person_album_id OUT person_table.person_album_id%TYPE,
        o_person_email OUT person_table.person_email%TYPE,
        o_person_first_name OUT person_table.person_first_name%TYPE,
        o_person_last_name OUT person_table.person_last_name%TYPE,
        o_person_father_name OUT person_table.person_father_name%TYPE
    )
IS
    CURSOR person_cursor IS SELECT person_id, person_album_id, person_email, person_first_name, person_last_name, person_father_name FROM person_table WHERE UPPER(person_email) = UPPER(p_person_email);
BEGIN
    OPEN person_cursor;
    FETCH person_cursor INTO o_person_id, o_person_album_id, o_person_email, o_person_first_name, o_person_last_name, o_person_father_name;
    IF person_cursor%NOTFOUND THEN
    RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
    END IF;
    CLOSE person_cursor;
END search_person;

--Search for an existing person
DECLARE
    person_id NUMBER(10);
    person_album_id NUMBER(10);
    person_email NVARCHAR2(50);
    person_first_name NVARCHAR2(50);
    person_last_name NVARCHAR2(50);
    person_father_name NVARCHAR2(50);
BEGIN
    search_person('yury.karlenok@mail.ru', person_id, person_album_id, person_email, person_first_name, person_last_name, person_father_name);
    dbms_output.put_line('Person id - ' || person_id);
    dbms_output.put_line('Person album id - ' || person_album_id);
    dbms_output.put_line('Person email - ' || person_email);
    dbms_output.put_line('Person first name - ' || person_first_name);
    dbms_output.put_line('Person last name - ' || person_last_name);
    dbms_output.put_line('Person father name - ' || person_father_name);
END;

--Search for a non-existent person. Error simulation
DECLARE
    person_id NUMBER(10);
    person_album_id NUMBER(10);
    person_email NVARCHAR2(50);
    person_first_name NVARCHAR2(50);
    person_last_name NVARCHAR2(50);
    person_father_name NVARCHAR2(50);
BEGIN
    search_person('eugeniaa.blinova@mail.ru', person_id, person_album_id, person_email, person_first_name, person_last_name, person_father_name);
    dbms_output.put_line('Person id - ' || person_id);
    dbms_output.put_line('Person album id - ' || person_album_id);
    dbms_output.put_line('Person email - ' || person_email);
    dbms_output.put_line('Person first name - ' || person_first_name);
    dbms_output.put_line('Person last name - ' || person_last_name);
    dbms_output.put_line('Person father name - ' || person_father_name);
END;

DROP PROCEDURE search_person;

-------------------------UPDATE_PERSON-------------------------

CREATE OR REPLACE PROCEDURE update_person
    (
        p_person_id IN person_table.person_id%TYPE,
        p_person_album_id IN person_table.person_album_id%TYPE,
        p_person_email IN person_table.person_email%TYPE,
        p_person_password IN person_table.person_password%TYPE,
        p_person_first_name IN person_table.person_first_name%TYPE,
        p_person_last_name IN person_table.person_last_name%TYPE,
        p_person_father_name IN person_table.person_father_name%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_person_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM person_table WHERE UPPER(person_email) = UPPER(p_person_email) AND person_id != p_person_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_person_album_id;
            IF (cnt != 0) THEN
                UPDATE person_table SET person_album_id = p_person_album_id, person_email = p_person_email, person_password = encryption_password(p_person_password), 
                person_first_name = p_person_first_name, person_last_name = p_person_last_name, person_father_name = p_person_father_name WHERE person_id = p_person_id;
                COMMIT;
            ELSE
                RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20007, 'This person already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
    END IF;
END update_person;

--Successful update
CALL update_person(21, 1, 'eugenia.blinova@mail.ru', 'Pa$$w0rd', 'Eugenia', 'Blinova', 'Alexandrovna');

--Update to an existing email. Error simulation
CALL update_person(21, 1, 'yury.karlenok@mail.ru', 'Pa$$w0rd', 'Eugenia', 'Blinova', 'Alexandrovna');

--Updating a non-existent person. Error simulation
CALL update_person(404, 1, 'yury.karlenok@mail.ru', 'Pa$$w0rd', 'Eugenia', 'Blinova', 'Alexandrovna');

--Update to an existing album. Error simulation
CALL update_person(21, 404, 'eugenia.blinova@mail.ru', 'Pa$$w0rd', 'Eugenia', 'Blinova', 'Alexandrovna');

DROP PROCEDURE update_person;

-------------------------DELETE_PERSON-------------------------

CREATE OR REPLACE PROCEDURE delete_person
    (
        p_person_id IN person_table.person_id%TYPE,
        o_person_album_id OUT person_table.person_album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_person_id;
    IF (cnt != 0) THEN
        SELECT person_album_id INTO o_person_album_id FROM person_table WHERE person_id = p_person_id;
        DELETE FROM person_table WHERE person_id = p_person_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
    END IF;
END delete_person;

--Successful delete
CALL delete_person(21);

--Deleting a non-existent person. Error simulation
CALL delete_person(404);

DROP PROCEDURE delete_person;

-------------------------UPDATE_ROLE-------------------------

CREATE OR REPLACE PROCEDURE update_role
    (
        p_person_id IN person_table.person_id%TYPE,
        p_role_id IN role_table.role_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_person_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM role_table WHERE role_id = p_role_id;
        IF (cnt != 0) THEN
            UPDATE person_table SET person_role_id = p_role_id WHERE person_id = p_person_id;
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20001, 'Role is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
    END IF;
END update_role;

--Successful update
CALL update_role(22, 1);
CALL update_role(22, 3);

--Updating a non-existent person. Error simulation
CALL update_role(404, 1);

--Update to a non-existent role. Error simulation
CALL update_role(22, 4);

DROP PROCEDURE update_role;

-------------------------CREATE_ROOM_TYPE-------------------------

CREATE OR REPLACE PROCEDURE create_room_type
    ( 
        p_room_type_name IN room_type_table.room_type_name%TYPE,
        p_room_type_capacity IN room_type_table.room_type_capacity%TYPE,
        p_room_type_daily_price IN room_type_table.room_type_daily_price%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM room_type_table WHERE UPPER(room_type_name) = UPPER(p_room_type_name);
    IF (cnt = 0) THEN
        IF (p_room_type_capacity > 0) THEN
            IF (p_room_type_daily_price > 0) THEN
                INSERT INTO room_type_table(room_type_name, room_type_capacity, room_type_daily_price) VALUES(p_room_type_name, p_room_type_capacity, p_room_type_daily_price);
                COMMIT;
            ELSE
                RAISE_APPLICATION_ERROR(-20014, 'The room type daily price should be positive number');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20013, 'The room type capacity should be positive number');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20012, 'This room type already exists');
    END IF;
END create_room_type;

--Successful create
CALL create_room_type('Four-person', 4, 70.00);

--Creating an existing room type. Error simulation
CALL create_room_type('One-person', 1, 25.00);

--Creation with a non-positive capacity. Error simulation
CALL create_room_type('Five-person', -5, 70.00);

--Creation with a non-positive price. Error simulation
CALL create_room_type('Five-person', 5, -70.00);

DROP PROCEDURE create_room_type;

-------------------------UPDATE_ROOM_TYPE-------------------------

CREATE OR REPLACE PROCEDURE update_room_type
    (
        p_room_type_id IN room_type_table.room_type_id%TYPE,
        p_room_type_name IN room_type_table.room_type_name%TYPE,
        p_room_type_capacity IN room_type_table.room_type_capacity%TYPE,
        p_room_type_daily_price IN room_type_table.room_type_daily_price%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM room_type_table WHERE room_type_id = p_room_type_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM room_type_table WHERE UPPER(room_type_name) = UPPER(p_room_type_name) AND room_type_id != p_room_type_id;
        IF (cnt = 0) THEN
            IF (p_room_type_capacity > 0) THEN
                IF (p_room_type_daily_price > 0) THEN
                    UPDATE room_type_table SET room_type_name = p_room_type_name, room_type_capacity = p_room_type_capacity, room_type_daily_price = p_room_type_daily_price WHERE room_type_id = p_room_type_id;
                    COMMIT;
                ELSE
                    RAISE_APPLICATION_ERROR(-20014, 'The room type daily price should be positive number');
                END IF; 
            ELSE
                RAISE_APPLICATION_ERROR(-20013, 'The room type capacity should be positive number');
            END IF; 
        ELSE
            RAISE_APPLICATION_ERROR(-20012, 'This room type already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20011, 'Room type is not found');
    END IF;
END update_room_type;

--Successful update
CALL update_room_type(4, 'Four-person', 4, 65.00);

--Update to an existing room type. Error simulation
CALL update_room_type(4, 'One-person', 4, 70.00);

--Updating a non-existent room type. Error simulation
CALL update_room_type(404, 'Five-person', 4, 70.00);

--Updating with a non-positive capacity. Error simulation
CALL update_room_type(4, 'Four-person', -4, 65.00);

--Updating with a non-positive price. Error simulation
CALL update_room_type(4, 'Four-person', 4, -65.00);

DROP PROCEDURE update_room_type;

-------------------------DELETE_ROOM_TYPE-------------------------

CREATE OR REPLACE PROCEDURE delete_room_type
    (
        p_room_type_id IN room_type_table.room_type_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM room_type_table WHERE room_type_id = p_room_type_id;
    IF (cnt != 0) THEN
        DELETE FROM room_type_table WHERE room_type_id = p_room_type_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20011, 'Room type is not found');
    END IF;
END delete_room_type;

--Successful delete
CALL delete_room_type(21);

--Deleting a non-existent room type. Error simulation
CALL delete_room_type(404);

DROP PROCEDURE delete_room_type;

-------------------------CREATE_ROOM-------------------------

CREATE OR REPLACE PROCEDURE create_room
    ( 
        p_room_room_type_id IN room_table.room_room_type_id%TYPE,
        p_room_album_id IN room_table.room_album_id%TYPE,
        p_room_number IN room_table.room_number%TYPE,
        p_room_description IN room_table.room_description%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM room_table WHERE UPPER(room_number) = UPPER(p_room_number);
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM room_table WHERE UPPER(room_description) = UPPER(p_room_description);
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM room_type_table WHERE room_type_id = p_room_room_type_id;
            IF (cnt != 0) THEN
                SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_room_album_id;
                IF (cnt != 0) THEN
                    INSERT INTO room_table(room_room_type_id, room_album_id, room_number, room_description) VALUES(p_room_room_type_id, p_room_album_id, p_room_number, p_room_description);
                    COMMIT;
                ELSE
                    RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20011, 'Room type is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20018, 'This room description already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20017, 'This room number already exists');
    END IF;
END create_room;

--Successful create
CALL create_room(1, NULL, '102');

--Creating an existing room number. Error simulation
CALL create_room(1, NULL, '101');

--Creating a non-existent room type. Error simulation
CALL create_room(10, NULL, '103');

--Creating a non-existent album. Error simulation
CALL create_room(1, 2, '104');

DROP PROCEDURE create_room;

-------------------------UPDATE_ROOM-------------------------

CREATE OR REPLACE PROCEDURE update_room
    (
        p_room_id IN room_table.room_id%TYPE,
        p_room_room_type_id IN room_table.room_room_type_id%TYPE,
        p_room_album_id IN room_table.room_album_id%TYPE,
        p_room_number IN room_table.room_number%TYPE,
        p_room_description IN room_table.room_description%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM room_table WHERE room_id = p_room_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM room_table WHERE UPPER(room_number) = UPPER(p_room_number) AND room_id != p_room_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM room_table WHERE UPPER(room_description) = UPPER(p_room_description) AND room_id != p_room_id;
            IF (cnt = 0) THEN
                SELECT COUNT(*) INTO cnt FROM room_type_table WHERE room_type_id = p_room_room_type_id;
                IF (cnt != 0) THEN
                    SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_room_album_id;
                    IF (cnt != 0) THEN
                        UPDATE room_table SET room_room_type_id = p_room_room_type_id, room_album_id = p_room_album_id, room_number = p_room_number, room_description = p_room_description WHERE room_id = p_room_id;
                        COMMIT;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20011, 'Room type is not found');
                END IF; 
            ELSE
                RAISE_APPLICATION_ERROR(-20018, 'This room description already exists');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20017, 'This room number already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20016, 'Room is not found');
    END IF;
END update_room;

--Successful update
CALL update_room(21, 1, NULL, '103');

--Updating a non-existent room. Error simulation
CALL update_room(404, 1, NULL, '101');

--Updating an existing room number. Error simulation
CALL update_room(21, 1, NULL, '101');

--Updating a non-existent room type. Error simulation
CALL update_room(21, 10, NULL, '103');

--Updating a non-existent album. Error simulation
CALL update_room(21, 1, 2, '103');

DROP PROCEDURE update_room;

-------------------------DELETE_ROOM-------------------------

CREATE OR REPLACE PROCEDURE delete_room
    (
        p_room_id IN room_table.room_id%TYPE,
        o_room_album_id OUT room_table.room_album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM room_table WHERE room_id = p_room_id;
    IF (cnt != 0) THEN
        SELECT room_album_id INTO o_room_album_id FROM room_table WHERE room_id = p_room_id;
        DELETE FROM room_table WHERE room_id = p_room_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20016, 'Room is not found');
    END IF;
END delete_room;

--Successful delete
CALL delete_room(21);

--Deleting a non-existent room number. Error simulation
CALL delete_room(404);

DROP PROCEDURE delete_room;

-------------------------CREATE_INVENTORY_TYPE-------------------------

CREATE OR REPLACE PROCEDURE create_inventory_type
    ( 
        p_inventory_type_name IN inventory_type_table.inventory_type_name%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM inventory_type_table WHERE UPPER(inventory_type_name) = UPPER(p_inventory_type_name);
    IF (cnt = 0) THEN
        INSERT INTO inventory_type_table(inventory_type_name) VALUES(p_inventory_type_name);
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20022, 'This inventory type already exists');
    END IF;
END create_inventory_type;

--First launch - successful create. Second - error simulation
CALL create_inventory_type('Hairdryer');

DROP PROCEDURE create_inventory_type;

-------------------------UPDATE_INVENTORY_TYPE-------------------------

CREATE OR REPLACE PROCEDURE update_inventory_type
    (
        p_inventory_type_id IN inventory_type_table.inventory_type_id%TYPE,
        p_inventory_type_name IN inventory_type_table.inventory_type_name%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM inventory_type_table WHERE inventory_type_id = p_inventory_type_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM inventory_type_table WHERE UPPER(inventory_type_name) = UPPER(p_inventory_type_name) AND inventory_type_id != p_inventory_type_id;
        IF (cnt = 0) THEN
            UPDATE inventory_type_table SET inventory_type_name = p_inventory_type_name WHERE inventory_type_id = p_inventory_type_id;
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20022, 'This inventory type already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20021, 'Inventory type is not found');
    END IF;
END update_inventory_type;

--Successful update
CALL update_inventory_type(22, 'Hairdryer');

--Update to an existing inventory type. Error simulation
CALL update_inventory_type(22, 'Iron');

--Updating a non-existent inventory type. Error simulation
CALL update_inventory_type(404, 'Hairdryer');

DROP PROCEDURE update_inventory_type;

-------------------------DELETE_INVENTORY_TYPE-------------------------

CREATE OR REPLACE PROCEDURE delete_inventory_type
    (
        p_inventory_type_id IN inventory_type_table.inventory_type_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM inventory_type_table WHERE inventory_type_id = p_inventory_type_id;
    IF (cnt != 0) THEN
        DELETE FROM inventory_type_table WHERE inventory_type_id = p_inventory_type_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20021, 'Inventory type is not found');
    END IF;
END delete_inventory_type;

--Successful delete
CALL delete_inventory_type(22);

--Deleting a non-existent inventory type. Error simulation
CALL delete_inventory_type(404);

DROP PROCEDURE delete_inventory_type;

-------------------------CREATE_INVENTORY-------------------------

CREATE OR REPLACE PROCEDURE create_inventory
    ( 
        p_inventory_inventory_type_id IN inventory_table.inventory_inventory_type_id%TYPE,
        p_inventory_album_id IN inventory_table.inventory_album_id%TYPE,
        p_inventory_description IN inventory_table.inventory_description%TYPE,
        p_inventory_daily_price IN inventory_table.inventory_daily_price%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM inventory_table WHERE UPPER(inventory_description) = UPPER(p_inventory_description);
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM inventory_type_table WHERE inventory_type_id = p_inventory_inventory_type_id;
        IF (cnt != 0) THEN
            SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_inventory_album_id;
            IF (cnt != 0) THEN
                IF (p_inventory_daily_price > 0) THEN
                    INSERT INTO inventory_table(inventory_inventory_type_id, inventory_album_id, inventory_description, inventory_daily_price) 
                    VALUES(p_inventory_inventory_type_id, p_inventory_album_id, p_inventory_description, p_inventory_daily_price);
                    COMMIT;
                ELSE
                    RAISE_APPLICATION_ERROR(-20028, 'The inventory daily price should be positive number');
                END IF; 
            ELSE
                RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20021, 'Inventory type is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20027, 'This inventory description already exists');
    END IF;
END create_inventory;

--Successful create
CALL create_inventory(1, NULL, 'HOT-1998-003', 3.50);

--Creating an existing inventory. Error simulation
CALL create_inventory(1, NULL, 'HOT-1998-001', 3.50);

--Creating a non-existent inventory type. Error simulation
CALL create_inventory(10, NULL, 'HOT-1998-004', 3.50);

--Creating a non-existent album. Error simulation
CALL create_inventory(1, 2, 'HOT-1998-004', 3.50);

--Creation with a non-positive price. Error simulation
CALL create_inventory(1, NULL, 'HOT-1998-004', -3.50);

DROP PROCEDURE create_inventory;

-------------------------UPDATE_INVENTORY-------------------------

CREATE OR REPLACE PROCEDURE update_inventory
    (
        p_inventory_id IN inventory_table.inventory_id%TYPE,
        p_inventory_inventory_type_id IN inventory_table.inventory_inventory_type_id%TYPE,
        p_inventory_album_id IN inventory_table.inventory_album_id%TYPE,
        p_inventory_description IN inventory_table.inventory_description%TYPE,
        p_inventory_daily_price IN inventory_table.inventory_daily_price%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM inventory_table WHERE inventory_id = p_inventory_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM inventory_table WHERE UPPER(inventory_description) = UPPER(p_inventory_description) AND inventory_id != p_inventory_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM inventory_type_table WHERE inventory_type_id = p_inventory_inventory_type_id;
            IF (cnt != 0) THEN
                SELECT COUNT(*) INTO cnt FROM album_table WHERE album_id = p_inventory_album_id;
                IF (cnt != 0) THEN
                    IF (p_inventory_daily_price > 0) THEN
                        UPDATE inventory_table SET inventory_inventory_type_id = p_inventory_inventory_type_id, inventory_album_id = p_inventory_album_id, 
                        inventory_description = p_inventory_description, inventory_daily_price = p_inventory_daily_price
                        WHERE inventory_id = p_inventory_id;
                        COMMIT;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20028, 'The inventory daily price should be positive number');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20061, 'Album is not found');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20021, 'Inventory type is not found');
            END IF; 
        ELSE
            RAISE_APPLICATION_ERROR(-20027, 'This inventory description already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20026, 'Inventory is not found');
    END IF;
END update_inventory;

--Successful update
CALL update_inventory(21, 1, NULL, 'HOT-1998-003', 7.50);

--Updating a non-existent inventory. Error simulation
CALL update_inventory(404, 1, NULL, 'HOT-1998-003', 7.50);

--Updating an existing inventory. Error simulation
CALL update_inventory(21, 1, NULL, 'HOT-1998-001', 7.50);

--Updating a non-existent inventory type. Error simulation
CALL update_inventory(21, 10, NULL, 'HOT-1998-003', 7.50);

--Updating a non-existent album. Error simulation
CALL update_inventory(21, 1, 2, 'HOT-1998-003', 7.50);

--Updating with a non-positive price. Error simulation
CALL update_inventory(21, 1, NULL, 'HOT-1998-003', -7.50);

DROP PROCEDURE update_inventory;

-------------------------DELETE_INVENTORY-------------------------

CREATE OR REPLACE PROCEDURE delete_inventory
    (
        p_inventory_id IN inventory_table.inventory_id%TYPE,
        o_inventory_album_id OUT inventory_table.inventory_album_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM inventory_table WHERE inventory_id = p_inventory_id;
    IF (cnt != 0) THEN
        SELECT inventory_album_id INTO o_inventory_album_id FROM inventory_table WHERE inventory_id = p_inventory_id;
        DELETE FROM inventory_table WHERE inventory_id = p_inventory_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20026, 'Inventory is not found');
    END IF;
END delete_inventory;

--Successful delete
CALL delete_inventory(21);

--Deleting a non-existent inventory. Error simulation
CALL delete_inventory(404);

DROP PROCEDURE delete_inventory;

-------------------------CREATE_SERVICE_TYPE-------------------------

CREATE OR REPLACE PROCEDURE create_service_type
    (
        p_service_type_name IN service_type_table.service_type_name%TYPE,
        p_service_type_daily_price IN service_type_table.service_type_daily_price%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM service_type_table WHERE UPPER(service_type_name) = UPPER(p_service_type_name);
    IF (cnt = 0) THEN
        IF (p_service_type_daily_price > 0) THEN
            INSERT INTO service_type_table(service_type_name, service_type_daily_price)
            VALUES(p_service_type_name, p_service_type_daily_price);
            COMMIT;
        ELSE
            RAISE_APPLICATION_ERROR(-20033, 'The service type daily price should be positive number');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20032, 'This service type already exists');
    END IF;
END create_service_type;

--Successful create
CALL create_service_type('Test', 25.00);

--Creating an existing service type. Error simulation
CALL create_service_type('Cleaning', 15.00);

--Creation with a non-positive price. Error simulation
CALL create_service_type('Test', -5.00);

DROP PROCEDURE create_service_type;

-------------------------UPDATE_SERVICE_TYPE-------------------------

CREATE OR REPLACE PROCEDURE update_service_type
    (
        p_service_type_id IN service_type_table.service_type_id%TYPE,
        p_service_type_name IN service_type_table.service_type_name%TYPE,
        p_service_type_daily_price IN service_type_table.service_type_daily_price%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM service_type_table WHERE service_type_id = p_service_type_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM service_type_table WHERE UPPER(service_type_name) = UPPER(p_service_type_name) AND service_type_id != p_service_type_id;
        IF (cnt = 0) THEN
            IF (p_service_type_daily_price > 0) THEN
                UPDATE service_type_table SET service_type_name = p_service_type_name, service_type_daily_price = p_service_type_daily_price WHERE service_type_id = p_service_type_id;
                COMMIT;
            ELSE
                RAISE_APPLICATION_ERROR(-20033, 'The service type daily price should be positive number');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20032, 'This service type already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20031, 'Service type is not found');
    END IF;
END update_service_type;

--Successful update
CALL update_service_type(21, 'Test', 25.00);

--Updating a non-existent service type. Error simulation
CALL update_service_type(22, 'Cleaning', 25.00);

--Updating an existing service type. Error simulation
CALL update_service_type(21, 'Feeding', 25.00);

--Updating with a non-positive price. Error simulation
CALL update_service_type(22, 'Cleaning', -25.00);

DROP PROCEDURE update_service_type;

-------------------------DELETE_SERVICE_TYPE-------------------------

CREATE OR REPLACE PROCEDURE delete_service_type
    (
        p_service_type_id IN service_type_table.service_type_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM service_type_table WHERE service_type_id = p_service_type_id;
    IF (cnt != 0) THEN
        DELETE FROM service_type_table WHERE service_type_id = p_service_type_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20031, 'Service type is not found');
    END IF;
END delete_service_type;

--Successful delete
CALL delete_service_type(21);

--Deleting a non-existent service type. Error simulation
CALL delete_service_type(404);

DROP PROCEDURE delete_service_type;

-------------------------CREATE_SERVICE-------------------------

CREATE OR REPLACE PROCEDURE create_service
    (
        p_service_service_type_id IN service_table.service_service_type_id%TYPE,
        p_service_person_id IN service_table.service_person_id%TYPE
    )
IS
    cnt NUMBER;
    role_id NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM service_table WHERE service_service_type_id = p_service_service_type_id AND service_person_id = p_service_person_id;
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM service_type_table WHERE service_type_id = p_service_service_type_id;
        IF (cnt != 0) THEN
            SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_service_person_id;
            IF (cnt != 0) THEN
                SELECT person_role_id INTO role_id FROM person_table WHERE person_id = p_service_person_id;
                IF (role_id = 2) THEN
                    INSERT INTO service_table(service_service_type_id, service_person_id)
                    VALUES(p_service_service_type_id, p_service_person_id);
                    COMMIT;
                ELSE
                    RAISE_APPLICATION_ERROR(-20038, 'This person is not a staff');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20031, 'Service type is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20037, 'This service already exists');
    END IF;
END create_service;

--Successful create
CALL create_service(3, 41);

--Creating an existing service. Error simulation
CALL create_service(1, 42);

--Creating with a person who is not staff. Error simulation
CALL create_service(1, 1);

DROP PROCEDURE create_service;

-------------------------UPDATE_SERVICE-------------------------

CREATE OR REPLACE PROCEDURE update_service
    (
        p_service_id IN service_table.service_id%TYPE,
        p_service_service_type_id IN service_table.service_service_type_id%TYPE,
        p_service_person_id IN service_table.service_person_id%TYPE
    )
IS
    cnt NUMBER;
    role_id NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM service_table WHERE service_id = p_service_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM service_table WHERE service_service_type_id = p_service_service_type_id AND service_person_id = p_service_person_id AND service_id != p_service_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM service_type_table WHERE service_type_id = p_service_service_type_id;
            IF (cnt != 0) THEN
                SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_service_person_id;
                IF (cnt != 0) THEN
                    SELECT person_role_id INTO role_id FROM person_table WHERE person_id = p_service_person_id;
                    IF (role_id = 2) THEN
                        UPDATE service_table SET service_service_type_id = p_service_service_type_id, service_person_id = p_service_person_id WHERE service_id = p_service_id;
                        COMMIT;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20038, 'This person is not a staff');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20031, 'Service type is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20037, 'This service already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20036, 'Service is not found');
    END IF;
END update_service;

--Successful update
CALL update_service(2, 1, 41);

--Updating a non-existent service. Error simulation
CALL update_service(404, 1, 4);

--Updating an existing service. Error simulation
CALL update_service(2, 1, 42);

--Updating with a person who is not staff. Error simulation
CALL update_service(2, 1, 1);

DROP PROCEDURE update_service;

-------------------------DELETE_SERVICE-------------------------

CREATE OR REPLACE PROCEDURE delete_service
    (
        p_service_id IN service_table.service_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM service_table WHERE service_id = p_service_id;
    IF (cnt != 0) THEN
        DELETE FROM service_table WHERE service_id = p_service_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20036, 'Service is not found');
    END IF;
END delete_service;

--Successful delete
CALL delete_service(6);

--Deleting a non-existent service. Error simulation
CALL delete_service(404);

DROP PROCEDURE delete_service;

-------------------------CREATE_BOOKING-------------------------

CREATE OR REPLACE PROCEDURE create_booking
    (
        p_booking_room_id IN booking_table.booking_room_id%TYPE,
        p_booking_start_date IN booking_table.booking_start_date%TYPE,
        p_booking_end_date IN booking_table.booking_end_date%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_room_id = p_booking_room_id AND booking_start_date = p_booking_start_date AND booking_end_date = p_booking_end_date;
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM room_table WHERE room_id = p_booking_room_id;
        IF (cnt != 0) THEN
            IF (p_booking_start_date <= p_booking_end_date) THEN
                INSERT INTO booking_table(booking_room_id, booking_start_date, booking_end_date)
                VALUES(p_booking_room_id, p_booking_start_date, p_booking_end_date);
                COMMIT;
            ELSE
                RAISE_APPLICATION_ERROR(-20043, 'Booking end date less than booking start date');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20016, 'Room is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20042, 'This booking already exists');
    END IF;
END create_booking;

--First launch - successful create. Second - error simulation
CALL create_booking(1, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-02', 'YYYY-MM-DD'));

--Creating with a non-existent room type. Error simulation
CALL create_booking(404, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-02', 'YYYY-MM-DD'));

--Creating with end date less than start date. Error simulation
CALL create_booking(1, TO_DATE('2021-01-02', 'YYYY-MM-DD'), TO_DATE('2021-01-01', 'YYYY-MM-DD'));

DROP PROCEDURE create_booking;


-------------------------GUEST_CREATE_BOOKING-------------------------

CREATE OR REPLACE PROCEDURE guest_create_booking
    (
        p_booking_room_id IN booking_table.booking_room_id%TYPE,
        p_booking_start_date IN booking_table.booking_start_date%TYPE,
        p_booking_end_date IN booking_table.booking_end_date%TYPE,
        o_booking_id OUT booking_table.booking_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_room_id = p_booking_room_id AND booking_start_date = p_booking_start_date AND booking_end_date = p_booking_end_date;
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM room_table WHERE room_id = p_booking_room_id;
        IF (cnt != 0) THEN
            IF (p_booking_start_date <= p_booking_end_date) THEN
                INSERT INTO booking_table(booking_room_id, booking_start_date, booking_end_date)
                VALUES(p_booking_room_id, p_booking_start_date, p_booking_end_date);
                COMMIT;
                SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_room_id = p_booking_room_id AND booking_start_date = p_booking_start_date AND booking_end_date = p_booking_end_date;
                IF (cnt != 0) THEN
                    SELECT booking_id INTO o_booking_id FROM booking_table WHERE booking_room_id = p_booking_room_id AND booking_start_date = p_booking_start_date AND booking_end_date = p_booking_end_date;
                ELSE
                    RAISE_APPLICATION_ERROR(-20044, 'Booking commit error');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20043, 'Booking end date less than booking start date');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20016, 'Room is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20042, 'This booking already exists');
    END IF;
END guest_create_booking;

--First launch - successful create. Second - error simulation
DECLARE
    booking_id NUMBER(10);
BEGIN
    guest_create_booking(1, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-04', 'YYYY-MM-DD'), booking_id);
    dbms_output.put_line('Booking id - ' || booking_id);
END;

--Creating with a non-existent room type. Error simulation
DECLARE
    booking_id NUMBER(10);
BEGIN
    guest_create_booking(404, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-02', 'YYYY-MM-DD'), booking_id);
    dbms_output.put_line('Booking id - ' || booking_id);
END;

--Creating with end date less than start date. Error simulation
DECLARE
    booking_id NUMBER(10);
BEGIN
    guest_create_booking(2, TO_DATE('2021-01-02', 'YYYY-MM-DD'), TO_DATE('2021-01-01', 'YYYY-MM-DD'), booking_id);
    dbms_output.put_line('Booking id - ' || booking_id);
END;

DROP PROCEDURE guest_create_booking;

-------------------------UPDATE_BOOKING-------------------------

CREATE OR REPLACE PROCEDURE update_booking
    (
        p_booking_id IN booking_table.booking_id%TYPE,
        p_booking_room_id IN booking_table.booking_room_id%TYPE,
        p_booking_start_date IN booking_table.booking_start_date%TYPE,
        p_booking_end_date IN booking_table.booking_end_date%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_booking_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_room_id = p_booking_room_id AND booking_start_date = p_booking_start_date AND booking_end_date = p_booking_end_date AND booking_id != p_booking_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM room_table WHERE room_id = p_booking_room_id;
            IF (cnt != 0) THEN
                IF (p_booking_start_date <= p_booking_end_date) THEN
                    UPDATE booking_table SET booking_room_id = p_booking_room_id, booking_start_date = p_booking_start_date, booking_end_date = p_booking_end_date WHERE booking_id = p_booking_id;
                    COMMIT;
                ELSE
                    RAISE_APPLICATION_ERROR(-20043, 'Booking end date less than booking start date');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20016, 'Room is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20042, 'This booking already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
    END IF;
END update_booking;

--Successful update
CALL update_booking(2, 1, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-04', 'YYYY-MM-DD'));

--Updating a non-existent booking. Error simulation
CALL update_booking(404, 1, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-02', 'YYYY-MM-DD'));

--Updating an existing booking. Error simulation
CALL update_booking(2, 1, TO_DATE('2022-11-26', 'YYYY-MM-DD'), TO_DATE('2022-11-30', 'YYYY-MM-DD'));

--Updating with a non-existent room type. Error simulation
CALL update_booking(2, 404, TO_DATE('2021-01-01', 'YYYY-MM-DD'), TO_DATE('2021-01-04', 'YYYY-MM-DD'));

--Creating with end date less than start date. Error simulation
CALL update_booking(2, 1, TO_DATE('2021-01-10', 'YYYY-MM-DD'), TO_DATE('2021-01-04', 'YYYY-MM-DD'));

DROP PROCEDURE update_booking;


-------------------------APPROVE_BOOKING-------------------------

CREATE OR REPLACE PROCEDURE approve_booking
    (
        p_booking_id IN booking_table.booking_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_booking_id;
    IF (cnt != 0) THEN
        UPDATE booking_table SET booking_state = 1 WHERE booking_id = p_booking_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
    END IF;
END approve_booking;

DROP PROCEDURE approve_booking;

-------------------------DELETE_BOOKING-------------------------

CREATE OR REPLACE PROCEDURE delete_booking
    (
        p_booking_id IN booking_table.booking_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_booking_id;
    IF (cnt != 0) THEN
        DELETE FROM booking_table WHERE booking_id = p_booking_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
    END IF;
END delete_booking;

--Successful delete
CALL delete_booking(2);

--Deleting a non-existent booking. Error simulation
CALL delete_booking(404);

DROP PROCEDURE delete_booking;


-------------------------GUEST_DELETE_BOOKING-------------------------

CREATE OR REPLACE PROCEDURE guest_delete_booking
    (
        p_booking_id IN booking_table.booking_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_booking_id;
    IF (cnt != 0) THEN
        DELETE FROM resident_table WHERE resident_booking_id = p_booking_id;
        DELETE FROM rent_table WHERE rent_booking_id = p_booking_id;
        DELETE FROM subscription_table WHERE subscription_booking_id = p_booking_id;
        DELETE FROM booking_table WHERE booking_id = p_booking_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
    END IF;
END guest_delete_booking;

--Successful delete
CALL guest_delete_booking(11);

--Deleting a non-existent booking. Error simulation
CALL guest_delete_booking(404);

DROP PROCEDURE guest_delete_booking;

-------------------------CREATE_RESIDENT-------------------------

CREATE OR REPLACE PROCEDURE create_resident
    (
        p_resident_person_id IN resident_table.resident_person_id%TYPE,
        p_resident_booking_id IN resident_table.resident_booking_id%TYPE
    )
IS
    cnt NUMBER;
    room_type_capacity NUMBER;
    resident_count NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM resident_table WHERE resident_person_id = p_resident_person_id AND resident_booking_id = p_resident_booking_id;
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_resident_person_id;
        IF (cnt != 0) THEN
            SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_resident_booking_id;
            IF (cnt != 0) THEN
                SELECT room_type_capacity INTO room_type_capacity FROM booking_view WHERE booking_id = p_resident_booking_id AND ROWNUM = 1;
                SELECT COUNT(*) INTO resident_count FROM resident_table WHERE resident_booking_id = p_resident_booking_id;
                IF (resident_count < room_type_capacity) THEN
                    INSERT INTO resident_table(resident_person_id, resident_booking_id)
                    VALUES(p_resident_person_id, p_resident_booking_id);
                    COMMIT;
                ELSE 
                    RAISE_APPLICATION_ERROR(-20048, 'Number of residents cannot exceed the room capacity');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20047, 'This resident already exists');
    END IF;
END create_resident;

--First launch - successful create. Second - error simulation
CALL create_resident(1, 5);

--Creating with a non-existent person. Error simulation
CALL create_resident(404, 5);

--Creating with a non-existent booking. Error simulation
CALL create_resident(1, 404);

--Creating with the number of residents greater than or equal to room capacity. Error simulation
CALL create_resident(21, 5);

CALL create_resident(1, 6);
CALL create_resident(21, 6);
CALL create_resident(41, 6);
CALL create_resident(42, 6);

CALL create_resident(1, 5);

DROP PROCEDURE create_resident;

-------------------------UPDATE_RESIDENT-------------------------

CREATE OR REPLACE PROCEDURE update_resident
    (
        p_resident_id IN resident_table.resident_id%TYPE,
        p_resident_person_id IN resident_table.resident_person_id%TYPE,
        p_resident_booking_id IN resident_table.resident_booking_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM resident_table WHERE resident_id = p_resident_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM resident_table WHERE resident_person_id = p_resident_person_id AND resident_booking_id = p_resident_booking_id AND resident_id != p_resident_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM person_table WHERE person_id = p_resident_person_id;
            IF (cnt != 0) THEN
                SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_resident_booking_id;
                IF (cnt != 0) THEN
                    UPDATE resident_table SET resident_person_id = p_resident_person_id, resident_booking_id = p_resident_booking_id WHERE resident_id = p_resident_id;
                    COMMIT;
                ELSE
                    RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20006, 'Person is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20047, 'This resident already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20046, 'Resident is not found');
    END IF;
END update_resident;

-- Successful update
CALL update_resident(2, 1, 6);

-- Updating an existing resident. Error simulation
CALL update_resident(2, 21, 6);

-- Updating a non-existent resident. Error simulation
CALL update_resident(404, 1, 6);

-- Updating a non-existent person. Error simulation
CALL update_resident(2, 404, 6);

-- Updating a non-existent booking. Error simulation
CALL update_resident(2, 1, 404);

DROP PROCEDURE update_resident;

-------------------------DELETE_RESIDENT-------------------------

CREATE OR REPLACE PROCEDURE delete_resident
    (
        p_resident_id IN resident_table.resident_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM resident_table WHERE resident_id = p_resident_id;
    IF (cnt != 0) THEN
        DELETE FROM resident_table WHERE resident_id = p_resident_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20046, 'Resident is not found');
    END IF;
END delete_resident;

--Successful delete
CALL delete_resident(1);

--Deleting a non-existent resident. Error simulation
CALL delete_resident(404);

DROP PROCEDURE delete_resident;

-------------------------CREATE_RENT-------------------------

CREATE OR REPLACE PROCEDURE create_rent
    (
        p_rent_inventory_id IN rent_table.rent_inventory_id%TYPE,
        p_rent_booking_id IN rent_table.rent_booking_id%TYPE,
        p_rent_start_date IN rent_table.rent_start_date%TYPE,
        p_rent_end_date IN rent_table.rent_end_date%TYPE
    )
IS
    cnt NUMBER;
    booking_start_date DATE;
    booking_end_date DATE;
BEGIN
    SELECT COUNT(*) INTO cnt FROM rent_table WHERE rent_inventory_id = p_rent_inventory_id AND rent_booking_id = p_rent_booking_id AND rent_start_date = p_rent_start_date AND rent_end_date = p_rent_end_date;
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM inventory_table WHERE inventory_id = p_rent_inventory_id;
        IF (cnt != 0) THEN
            SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_rent_booking_id;
            IF (cnt != 0) THEN
                SELECT booking_start_date, booking_end_date INTO booking_start_date, booking_end_date FROM booking_table WHERE booking_id = p_rent_booking_id AND ROWNUM = 1;
                IF (p_rent_start_date >= booking_start_date AND p_rent_end_date <= booking_end_date) THEN
                    IF (p_rent_start_date <= p_rent_end_date) THEN
                        INSERT INTO rent_table(rent_inventory_id, rent_booking_id, rent_start_date, rent_end_date)
                        VALUES(p_rent_inventory_id, p_rent_booking_id, p_rent_start_date, p_rent_end_date);
                        COMMIT;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20053, 'Rent end date less than rent start date');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20054, 'Rent period is outside booking period');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20026, 'Inventory is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20052, 'This rent already exists');
    END IF;
END create_rent;

--First launch - successful create. Second - error simulation
CALL create_rent(1, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Creating with a non-existent inventory. Error simulation
CALL create_rent(404, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Creating with a non-existent booking. Error simulation
CALL create_rent(1, 404, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Creating with rent period which outside booking period. Error simulation
CALL create_rent(2, 9, TO_DATE('2022-12-01', 'YYYY-MM-DD'), TO_DATE('2022-12-04', 'YYYY-MM-DD'));

--Creating with end date less than start date. Error simulation
CALL create_rent(2, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-02', 'YYYY-MM-DD'));

DROP PROCEDURE create_rent;

-------------------------UPDATE_RENT-------------------------

CREATE OR REPLACE PROCEDURE update_rent
    (
        p_rent_id IN rent_table.rent_id%TYPE,
        p_rent_inventory_id IN rent_table.rent_inventory_id%TYPE,
        p_rent_booking_id IN rent_table.rent_booking_id%TYPE,
        p_rent_start_date IN rent_table.rent_start_date%TYPE,
        p_rent_end_date IN rent_table.rent_end_date%TYPE
    )
IS
    cnt NUMBER;
    booking_start_date DATE;
    booking_end_date DATE;
BEGIN
    SELECT COUNT(*) INTO cnt FROM rent_table WHERE rent_id = p_rent_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM rent_table WHERE rent_inventory_id = p_rent_inventory_id AND rent_booking_id = p_rent_booking_id AND rent_start_date = p_rent_start_date AND rent_end_date = p_rent_end_date AND rent_id != p_rent_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM inventory_table WHERE inventory_id = p_rent_inventory_id;
            IF (cnt != 0) THEN
                SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_rent_booking_id;
                IF (cnt != 0) THEN
                    SELECT booking_start_date, booking_end_date INTO booking_start_date, booking_end_date FROM booking_table WHERE booking_id = p_rent_booking_id AND ROWNUM = 1;
                    IF (p_rent_start_date >= booking_start_date AND p_rent_end_date <= booking_end_date) THEN
                        IF (p_rent_start_date <= p_rent_end_date) THEN
                            UPDATE rent_table SET rent_inventory_id = p_rent_inventory_id, rent_booking_id = p_rent_booking_id, rent_start_date = p_rent_start_date, rent_end_date = p_rent_end_date WHERE rent_id = p_rent_id;
                            COMMIT;
                        ELSE
                            RAISE_APPLICATION_ERROR(-20053, 'Rent end date less than rent start date');
                        END IF;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20054, 'Rent period is outside booking period');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20026, 'Inventory is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20052, 'This rent already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20051, 'Rent is not found');
    END IF;
END update_rent;

--Successful update
CALL update_rent(26, 1, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating a non-existent rent. Error simulation
CALL update_rent(404, 1, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating an existing rent. Error simulation
CALL update_rent(26, 2, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating a non-existent inventory. Error simulation
CALL update_rent(26, 404, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating a non-existent booking. Error simulation
CALL update_rent(26, 2, 404, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating with rent period which outside booking period. Error simulation
CALL update_rent(26, 2, 9, TO_DATE('2022-12-01', 'YYYY-MM-DD'), TO_DATE('2022-12-04', 'YYYY-MM-DD'));

--Updating with end date less than start date. Error simulation
CALL update_rent(26, 2, 9, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-02', 'YYYY-MM-DD'));

DROP PROCEDURE update_rent;

-------------------------DELETE_RENT-------------------------

CREATE OR REPLACE PROCEDURE delete_rent
    (
        p_rent_id IN rent_table.rent_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM rent_table WHERE rent_id = p_rent_id;
    IF (cnt != 0) THEN
        DELETE FROM rent_table WHERE rent_id = p_rent_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20051, 'Rent is not found');
    END IF;
END delete_rent;

--Successful delete
CALL delete_rent(26);

--Deleting a non-existent rent. Error simulation
CALL delete_rent(404);

DROP PROCEDURE delete_rent;


-------------------------CREATE_SUBSCRIPTION-------------------------

CREATE OR REPLACE PROCEDURE create_subscription
    (
        p_subscription_service_id IN subscription_table.subscription_service_id%TYPE,
        p_subscription_booking_id IN subscription_table.subscription_booking_id%TYPE,
        p_subscription_start_date IN subscription_table.subscription_start_date%TYPE,
        p_subscription_end_date IN subscription_table.subscription_end_date%TYPE
    )
IS
    cnt NUMBER;
    booking_start_date DATE;
    booking_end_date DATE;
BEGIN
    SELECT COUNT(*) INTO cnt FROM subscription_table WHERE subscription_service_id = p_subscription_service_id AND subscription_booking_id = p_subscription_booking_id AND subscription_start_date = p_subscription_start_date AND subscription_end_date = p_subscription_end_date;
    IF (cnt = 0) THEN
        SELECT COUNT(*) INTO cnt FROM service_table WHERE service_id = p_subscription_service_id;
        IF (cnt != 0) THEN
            SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_subscription_booking_id;
            IF (cnt != 0) THEN
                SELECT booking_start_date, booking_end_date INTO booking_start_date, booking_end_date FROM booking_table WHERE booking_id = p_subscription_booking_id AND ROWNUM = 1;
                IF (p_subscription_start_date >= booking_start_date AND p_subscription_end_date <= booking_end_date) THEN
                    IF (p_subscription_start_date <= p_subscription_end_date) THEN
                        INSERT INTO subscription_table(subscription_service_id, subscription_booking_id, subscription_start_date, subscription_end_date)
                        VALUES(p_subscription_service_id, p_subscription_booking_id, p_subscription_start_date, p_subscription_end_date);
                        COMMIT;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20058, 'Subscription end date less than subscription start date');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20059, 'Subscription period is outside booking period');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
            END IF;
        ELSE
            RAISE_APPLICATION_ERROR(-20036, 'Service is not found');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20057, 'This subscription already exists');
    END IF;
END create_subscription;

--First launch - successful create. Second - error simulation
CALL create_subscription(2, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Creating with a non-existent service. Error simulation
CALL create_subscription(404, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Creating with a non-existent booking. Error simulation
CALL create_subscription(2, 404, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Creating with subscription period which outside booking period. Error simulation
CALL create_subscription(3, 10, TO_DATE('2022-12-01', 'YYYY-MM-DD'), TO_DATE('2022-12-04', 'YYYY-MM-DD'));

--Creating with end date less than start date. Error simulation
CALL create_subscription(3, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-02', 'YYYY-MM-DD'));

DROP PROCEDURE create_subscription;

-------------------------UPDATE_SUBSCRIPTION-------------------------

CREATE OR REPLACE PROCEDURE update_subscription
    (
        p_subscription_id IN subscription_table.subscription_id%TYPE,
        p_subscription_service_id IN subscription_table.subscription_service_id%TYPE,
        p_subscription_booking_id IN subscription_table.subscription_booking_id%TYPE,
        p_subscription_start_date IN subscription_table.subscription_start_date%TYPE,
        p_subscription_end_date IN subscription_table.subscription_end_date%TYPE
    )
IS
    cnt NUMBER;
    booking_start_date DATE;
    booking_end_date DATE;
BEGIN
    SELECT COUNT(*) INTO cnt FROM subscription_table WHERE subscription_id = p_subscription_id;
    IF (cnt != 0) THEN
        SELECT COUNT(*) INTO cnt FROM subscription_table WHERE subscription_service_id = p_subscription_service_id AND subscription_booking_id = p_subscription_booking_id AND subscription_start_date = p_subscription_start_date AND subscription_end_date = p_subscription_end_date AND subscription_id != p_subscription_id;
        IF (cnt = 0) THEN
            SELECT COUNT(*) INTO cnt FROM service_table WHERE service_id = p_subscription_service_id;
            IF (cnt != 0) THEN
                SELECT COUNT(*) INTO cnt FROM booking_table WHERE booking_id = p_subscription_booking_id;
                IF (cnt != 0) THEN
                    SELECT booking_start_date, booking_end_date INTO booking_start_date, booking_end_date FROM booking_table WHERE booking_id = p_subscription_booking_id AND ROWNUM = 1;
                    IF (p_subscription_start_date >= booking_start_date AND p_subscription_end_date <= booking_end_date) THEN
                        IF (p_subscription_start_date <= p_subscription_end_date) THEN
                            UPDATE subscription_table SET subscription_service_id = p_subscription_service_id, subscription_booking_id = p_subscription_booking_id, subscription_start_date = p_subscription_start_date, subscription_end_date = p_subscription_end_date WHERE subscription_id = p_subscription_id;
                            COMMIT;
                        ELSE
                            RAISE_APPLICATION_ERROR(-20058, 'Subscription end date less than subscription start date');
                        END IF;
                    ELSE
                        RAISE_APPLICATION_ERROR(-20059, 'Subscription period is outside booking period');
                    END IF;
                ELSE
                    RAISE_APPLICATION_ERROR(-20041, 'Booking is not found');
                END IF;
            ELSE
                RAISE_APPLICATION_ERROR(-20036, 'Service is not found');
            END IF;
        ELSE
             RAISE_APPLICATION_ERROR(-20057, 'This subscription already exists');
        END IF;
    ELSE
        RAISE_APPLICATION_ERROR(-20056, 'Subscription is not found');
    END IF;
END update_subscription;

--Successful update
CALL update_subscription(1, 2, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating a non-existent subscription. Error simulation
CALL update_subscription(404, 2, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating an existing subscription. Error simulation
CALL update_subscription(1, 2, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating a non-existent service. Error simulation
CALL update_subscription(1, 404, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating a non-existent booking. Error simulation
CALL update_subscription(1, 2, 404, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-03', 'YYYY-MM-DD'));

--Updating with subscription period which outside booking period. Error simulation
CALL update_subscription(1, 4, 10, TO_DATE('2022-12-01', 'YYYY-MM-DD'), TO_DATE('2022-12-04', 'YYYY-MM-DD'));

--Updating with end date less than start date. Error simulation
CALL update_subscription(1, 4, 10, TO_DATE('2022-12-03', 'YYYY-MM-DD'), TO_DATE('2022-12-02', 'YYYY-MM-DD'));

DROP PROCEDURE update_subscription;

-------------------------DELETE_SUBSCRIPTION-------------------------

CREATE OR REPLACE PROCEDURE delete_subscription
    (
        p_subscription_id IN subscription_table.subscription_id%TYPE
    )
IS
    cnt NUMBER;
BEGIN
    SELECT COUNT(*) INTO cnt FROM subscription_table WHERE subscription_id = p_subscription_id;
    IF (cnt != 0) THEN
        DELETE FROM subscription_table WHERE subscription_id = p_subscription_id;
        COMMIT;
    ELSE
        RAISE_APPLICATION_ERROR(-20056, 'Subscription is not found');
    END IF;
END delete_subscription;

--Successful delete
CALL delete_subscription(1);

--Deleting a non-existent subscription. Error simulation
CALL delete_subscription(404);

DROP PROCEDURE delete_subscription;

-------------------------EXPORT_INVENTORY_TYPE-------------------------

CREATE OR REPLACE PROCEDURE export_inventory_type
IS
    rc sys_refcursor;
    doc DBMS_XMLDOM.DOMDocument;
BEGIN
    OPEN rc FOR SELECT inventory_type_id, inventory_type_name FROM HotelAdmin.inventory_type_table;
    doc := DBMS_XMLDOM.NewDOMDocument(XMLTYPE(rc));
    DBMS_XMLDOM.WRITETOFILE(doc, 'DATA_PUMP_DIR/export_inventory_type.xml');
END export_inventory_type;

CALL export_inventory_type();

DROP PROCEDURE export_inventory_type;

-------------------------IMPORT_INVENTORY_TYPE-------------------------

CREATE OR REPLACE PROCEDURE import_inventory_type
IS
BEGIN
    INSERT INTO inventory_type_table (inventory_type_name)
    SELECT ExtractValue(VALUE(inventory_type_xml), '//INVENTORY_TYPE_NAME') AS inventory_type_name
    FROM TABLE(XMLSequence(EXTRACT(XMLTYPE(bfilename('DATA_PUMP_DIR', 'import_inventory_type.xml'),
    nls_charset_id('UTF-8')),'/ROWSET/ROW'))) inventory_type_xml;
END import_inventory_type;

CALL import_inventory_type();

DROP PROCEDURE import_inventory_type;

-------------------------INSERT_100K-------------------------

CREATE OR REPLACE PROCEDURE insert_100k
IS
BEGIN
    FOR i IN 1 .. 100000 LOOP
    INSERT INTO role_table(role_name) VALUES('100k role');
    END LOOP;
    COMMIT;
END insert_100k;

CALL insert_100k();

DROP PROCEDURE insert_100k;

SELECT COUNT(*) FROM role_table;

-------------------------DELETE_100K-------------------------

CREATE OR REPLACE PROCEDURE delete_100k
IS
BEGIN
    DELETE FROM role_table WHERE role_id > 3;
    COMMIT;
END delete_100k;

CALL delete_100k();

DROP PROCEDURE delete_100k;

SELECT * FROM role_table;