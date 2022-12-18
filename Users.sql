ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

-------------------------HOTELBADMIN-------------------------

CREATE USER HotelAdmin
IDENTIFIED BY Pa$$w0rd 
DEFAULT TABLESPACE Users 
QUOTA UNLIMITED ON Users;

GRANT CONNECT TO HotelAdmin;
GRANT CREATE TABLE TO HotelAdmin;
GRANT CREATE SEQUENCE TO HotelAdmin;
GRANT CREATE VIEW TO HotelAdmin;
GRANT CREATE INDEXTYPE TO HotelAdmin;
GRANT CREATE PROCEDURE TO HotelAdmin;
GRANT CREATE TRIGGER TO HotelAdmin;
GRANT CREATE SESSION TO HotelAdmin;
GRANT CREATE JOB TO HotelAdmin;

GRANT CREATE ANY DIRECTORY TO HotelAdmin;
GRANT DROP ANY DIRECTORY TO HotelAdmin;

GRANT EXECUTE ON sys.dbms_crypto TO HotelAdmin;

COMMIT;

DROP USER HotelAdmin;

-------------------------HOTELGUEST-------------------------

CREATE USER HotelGuest
IDENTIFIED BY Pa$$w0rd 
DEFAULT TABLESPACE Users 
QUOTA UNLIMITED ON Users;

GRANT CREATE SESSION TO HotelGuest;

GRANT SELECT ON HotelAdmin.room_type_table TO HotelGuest;
GRANT SELECT ON HotelAdmin.inventory_type_table TO HotelGuest;
GRANT SELECT ON HotelAdmin.service_type_table TO HotelGuest;

GRANT SELECT ON HotelAdmin.album_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.room_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.inventory_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.service_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.booking_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.resident_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.rent_view TO HotelGuest;
GRANT SELECT ON HotelAdmin.subscription_view TO HotelGuest;

GRANT EXECUTE ON HotelAdmin.create_album TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.delete_album TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.register_person TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.log_in_person TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.search_person TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.update_person TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.delete_person TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.encryption_password TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.decryption_password TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.guest_create_booking TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.create_resident TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.create_rent TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.create_subscription TO HotelGuest;
GRANT EXECUTE ON HotelAdmin.guest_delete_booking TO HotelGuest;

COMMIT;

DROP USER HotelGuest;