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