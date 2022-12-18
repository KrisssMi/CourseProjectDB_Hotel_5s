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

select * from inventory_type_table;
delete from inventory_type_table where inventory_type_id = 5;
commit;

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




/*
docker exec -u root -t -i 0299d2276ad7cef6f9e64d084ed715d9249758d13c99c1edb01a1ee66073e98c /bin/bash
cd /opt/oracle/admin/XE/dpdump/

������� !!!
1) ls � ���������� - ����� ��� (����� ���� ������� rm inventory_type_export.xml)
2) �������� inventory_type_export ���������;
3) ������ �� inventory_type_table
4) ls � ���������� - ���� ��������, cat inventory_type_export.xml - ���������� ��������� �������

������ !!!
1) ������ �� inventory_type_table (����������, ��� ��� ��� ����������)
2) � ���������� ���� echo:

echo "<?xml version=\"1.0\"?><ROWSET><ROW><INVENTORY_TYPE_NAME>Cup</INVENTORY_TYPE_NAME></ROW></ROWSET>" > import_inventory_type.xml

3) �������� ��������� inventory_type_import, ����� ls, cat inventory_type_import.xml, ������ �� inventory_type_table (��� ��������� ����� ��������)
*/