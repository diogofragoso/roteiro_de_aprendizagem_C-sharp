-- Aula 07. Execute em uma branch didática vazia, uma única vez.
BEGIN;
CREATE TABLE usuario (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    username varchar(50) NOT NULL UNIQUE CHECK (username = lower(username)),
    senha_hash text NOT NULL,
    perfil varchar(20) NOT NULL CHECK (perfil IN ('Administrador', 'Caixa'))
);
COMMIT;
