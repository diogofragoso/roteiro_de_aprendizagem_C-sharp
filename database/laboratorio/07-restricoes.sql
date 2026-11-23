-- Aula 11. Execute inteiro numa nova sessão com conexão direta.
-- Tabelas temporárias; não cria contas nem modifica o esquema do aplicativo.
CREATE TEMP TABLE lab07_usuario (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    username varchar(50) NOT NULL UNIQUE CHECK (username = lower(username)),
    perfil varchar(20) NOT NULL CHECK (perfil IN ('Administrador', 'Caixa'))
);
INSERT INTO lab07_usuario(username, perfil) VALUES ('teste', 'Caixa');
DO $$
BEGIN
    BEGIN
        INSERT INTO lab07_usuario(username, perfil) VALUES ('teste', 'Caixa');
        RAISE EXCEPTION 'FALHOU: duplicidade foi aceita';
    EXCEPTION WHEN unique_violation THEN RAISE NOTICE 'OK: UNIQUE'; END;
    BEGIN
        INSERT INTO lab07_usuario(username, perfil) VALUES (NULL, 'Caixa');
        RAISE EXCEPTION 'FALHOU: NULL foi aceito';
    EXCEPTION WHEN not_null_violation THEN RAISE NOTICE 'OK: NOT NULL'; END;
    BEGIN
        INSERT INTO lab07_usuario(username, perfil) VALUES ('MAIUSCULO', 'Caixa');
        RAISE EXCEPTION 'FALHOU: maiuscula foi aceita';
    EXCEPTION WHEN check_violation THEN RAISE NOTICE 'OK: CHECK minusculas'; END;
    BEGIN
        INSERT INTO lab07_usuario(username, perfil) VALUES ('outro', 'Gerente');
        RAISE EXCEPTION 'FALHOU: perfil foi aceito';
    EXCEPTION WHEN check_violation THEN RAISE NOTICE 'OK: CHECK perfil'; END;
END $$;
-- Ausência e vazio são diferentes: esta tabela de exercício aceita ''.
INSERT INTO lab07_usuario(username, perfil) VALUES ('', 'Caixa');
SELECT id, username, perfil FROM lab07_usuario ORDER BY id;
BEGIN;
INSERT INTO lab07_usuario(username, perfil) VALUES ('revertido', 'Caixa');
ROLLBACK;
BEGIN;
INSERT INTO lab07_usuario(username, perfil) VALUES ('confirmado', 'Caixa');
COMMIT;
SELECT username FROM lab07_usuario ORDER BY id;
-- Esperado: teste, vazio e confirmado; revertido não permanece.
