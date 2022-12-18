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