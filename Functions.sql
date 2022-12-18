ALTER SESSION SET "_ORACLE_SCRIPT" = TRUE;
--GRANT EXECUTE ON SYS.dbms_crypto TO MrWeebeez;

-------------------------ENCRYPTION_PASSWORD-------------------------

CREATE OR REPLACE FUNCTION encryption_password
    (
        p_person_password IN person_table.person_password%TYPE
    )
    RETURN person_table.person_password%TYPE
IS
    l_key VARCHAR2(2000) := '2901200316052000';
    l_in_val VARCHAR2(2000) := p_person_password;
    l_mod NUMBER := DBMS_CRYPTO.encrypt_aes128 + DBMS_CRYPTO.chain_cbc + DBMS_CRYPTO.pad_pkcs5;
    l_enc RAW(2000);
BEGIN
    l_enc := DBMS_CRYPTO.encrypt(utl_i18n.string_to_raw(l_in_val, 'AL32UTF8'), l_mod, utl_i18n.string_to_raw(l_key, 'AL32UTF8'));
RETURN RAWTOHEX(l_enc);
END encryption_password;

-------------------------DECRYPTION_PASSWORD-------------------------

CREATE OR REPLACE FUNCTION decryption_password
    (
        p_person_password IN person_table.person_password%TYPE
    )
    RETURN person_table.person_password%TYPE
IS
    l_key VARCHAR2(2000) := '2901200316052000';
    l_in_val RAW(2000) := HEXTORAW(p_person_password);
    l_mod NUMBER := DBMS_CRYPTO.encrypt_aes128 + DBMS_CRYPTO.chain_cbc + DBMS_CRYPTO.pad_pkcs5;
    l_dec RAW(2000);
BEGIN
    l_dec := DBMS_CRYPTO.decrypt(l_in_val, l_mod, utl_i18n.string_to_raw(l_key, 'AL32UTF8'));
RETURN utl_i18n.raw_to_char(l_dec);
END decryption_password;