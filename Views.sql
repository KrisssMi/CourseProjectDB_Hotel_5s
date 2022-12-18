ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;

-------------------------ALBUM_VIEW-------------------------

CREATE VIEW album_view 
AS SELECT album_table.album_id, album_table.album_name, album_table.album_object, --album_table fields 
photo_table.photo_id, photo_source --photo_table fields 
FROM album_table INNER JOIN photo_table ON album_table.album_id = photo_table.photo_album_id;

SELECT * FROM album_view;

DROP VIEW album_view;

-------------------------PERSON_VIEW-------------------------

CREATE VIEW person_view 
AS SELECT person_table.person_id, person_table.person_email, person_table.person_password, person_table.person_first_name, person_table.person_last_name, person_table.person_father_name, --person_table fields
role_table.role_id, role_table.role_name, --role_table fields
album_table.album_id, album_table.album_name, album_table.album_object --album_table fields
FROM person_table INNER JOIN role_table ON person_table.person_role_id = role_table.role_id
LEFT OUTER JOIN album_table ON person_table.person_album_id = album_table.album_id;

SELECT * FROM person_view;

DROP VIEW person_view;

-------------------------ROOM_VIEW-------------------------

CREATE VIEW room_view 
AS SELECT room_table.room_id, room_table.room_number, room_table.room_description, --room_table fields 
room_type_table.room_type_id, room_type_table.room_type_name, room_type_table.room_type_capacity, room_type_table.room_type_daily_price, --room_type_table fields 
album_table.album_id, album_table.album_name, album_table.album_object --album_table fields
FROM room_table INNER JOIN room_type_table ON room_table.room_room_type_id = room_type_table.room_type_id
LEFT OUTER JOIN album_table ON room_table.room_album_id = album_table.album_id;

SELECT * FROM room_view;

DROP VIEW room_view;

-------------------------INVENTORY_VIEW-------------------------

CREATE VIEW inventory_view 
AS SELECT inventory_type_table.inventory_type_id, inventory_type_table.inventory_type_name, --inventory_type_table fields 
inventory_table.inventory_id, inventory_table.inventory_description, inventory_table.inventory_daily_price, --inventory_table fields
album_table.album_id, album_table.album_name, album_table.album_object --album_table fields
FROM inventory_type_table INNER JOIN inventory_table ON inventory_type_table.inventory_type_id = inventory_table.inventory_inventory_type_id
LEFT OUTER JOIN album_table ON inventory_table.inventory_album_id = album_table.album_id;

SELECT * FROM inventory_view;

DROP VIEW inventory_view;

-------------------------SERVICE_VIEW-------------------------

CREATE VIEW service_view 
AS SELECT service_type_table.service_type_id, service_type_table.service_type_name, service_type_table.service_type_daily_price, --service_type_table fields 
service_table.service_id, --service_table fields 
person_view.person_id, person_view.person_email, person_view.person_password, person_view.person_first_name, person_view.person_last_name, person_view.person_father_name, --person_table fields
person_view.role_id, person_view.role_name, --role_table fields
person_view.album_id, person_view.album_name, person_view.album_object --album_table fields
FROM service_type_table INNER JOIN service_table ON service_type_table.service_type_id = service_table.service_service_type_id
LEFT OUTER JOIN person_view ON service_table.service_person_id = person_view.person_id;

SELECT * FROM service_view;

DROP VIEW service_view;

-------------------------BOOKING_VIEW-------------------------

CREATE VIEW booking_view 
AS SELECT booking_table.booking_id, booking_table.booking_start_date, booking_table.booking_end_date, booking_table.booking_state, --booking_table fields 
booking_table.booking_end_date - booking_table.booking_start_date + 1 AS booking_duration, --booking_duration
( booking_table.booking_end_date - booking_table.booking_start_date + 1 ) * room_view.room_type_daily_price AS booking_price, --booking_price
room_view.room_id, room_view.room_number, room_view.room_description, --room_table fields 
room_view.room_type_id, room_view.room_type_name, room_view.room_type_capacity, room_view.room_type_daily_price, --room_type_table fields
room_view.album_id, room_view.album_name, room_view.album_object --album_table fields
FROM booking_table LEFT OUTER JOIN room_view ON booking_table.booking_room_id = room_view.room_id;

SELECT * FROM booking_view;

DROP VIEW booking_view;

-------------------------RESIDENT_VIEW-------------------------

CREATE VIEW resident_view 
AS SELECT resident_table.resident_id, --resident_table fields
person_view.person_id, person_view.person_email, person_view.person_password, person_view.person_first_name, person_view.person_last_name, person_view.person_father_name, --person_table fields
person_view.role_id, person_view.role_name, --role_table fields
person_view.album_id, person_view.album_name, person_view.album_object, --album_table fields
booking_view.booking_id, booking_view.booking_start_date, booking_view.booking_end_date, booking_view.booking_state, booking_duration, booking_price --booking_table fields 
FROM resident_table INNER JOIN booking_view ON resident_table.resident_booking_id = booking_view.booking_id
INNER JOIN person_view ON resident_table.resident_person_id = person_view.person_id;

SELECT * FROM resident_view;

DROP VIEW resident_view;

-------------------------RENT_VIEW-------------------------

CREATE VIEW rent_view 
AS SELECT rent_table.rent_id, rent_table.rent_start_date, rent_table.rent_end_date, --resident_table fields
rent_table.rent_end_date - rent_table.rent_start_date + 1 AS rent_duration, --rent_duration
( rent_table.rent_end_date - rent_table.rent_start_date + 1 ) * inventory_view.inventory_daily_price AS rent_price, --rent_price
booking_view.booking_id, --booking_table fields 
inventory_view.inventory_type_id, inventory_view.inventory_type_name, --inventory_type_table fields 
inventory_view.inventory_id, inventory_view.inventory_description, inventory_view.inventory_daily_price, --inventory_table fields
inventory_view.album_id, inventory_view.album_name, inventory_view.album_object --album_table fields
FROM rent_table INNER JOIN booking_view ON rent_table.rent_booking_id = booking_view.booking_id
INNER JOIN inventory_view ON rent_table.rent_inventory_id = inventory_view.inventory_id;

SELECT * FROM rent_view;

DROP VIEW rent_view;

-------------------------SUBSCRIPTION_VIEW-------------------------

CREATE VIEW subscription_view 
AS SELECT subscription_table.subscription_id, subscription_table.subscription_start_date, subscription_table.subscription_end_date, --subscription_table fields
subscription_table.subscription_end_date - subscription_table.subscription_start_date + 1 AS subscription_duration, --subscription_duration
( subscription_table.subscription_end_date - subscription_table.subscription_start_date + 1 ) * service_view.service_type_daily_price AS subscription_price, --subscription_price
booking_view.booking_id, --booking_table fields 
service_view.service_type_id, service_view.service_type_name, service_view.service_type_daily_price, --service_type_table fields 
service_view.service_id, --service_table fields 
service_view.person_id, service_view.person_email, service_view.person_password, service_view.person_first_name, service_view.person_last_name, service_view.person_father_name, --person_table fields
service_view.role_id, service_view.role_name, --role_table fields
service_view.album_id, service_view.album_name, service_view.album_object --album_table fields
FROM subscription_table INNER JOIN booking_view ON subscription_table.subscription_booking_id = booking_view.booking_id
INNER JOIN service_view ON subscription_table.subscription_service_id = service_view.service_id;

SELECT * FROM subscription_view;

DROP VIEW subscription_view;