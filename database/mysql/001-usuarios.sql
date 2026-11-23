-- Aula 11: executar uma vez no banco MySQL/MariaDB de laboratório selecionado.
-- DDL provoca commit implícito; não há rollback da criação das tabelas.
CREATE TABLE usuario (
    id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
    username varchar(50) NOT NULL UNIQUE CHECK (username = lower(username)),
    senha_hash text NOT NULL,
    perfil varchar(20) NOT NULL CHECK (perfil IN ('Administrador', 'Caixa'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
