-- Aula 15: depende de 001. Executar uma vez no mesmo banco.
CREATE TABLE produto (
    id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
    codigo varchar(30) NOT NULL UNIQUE CHECK (length(trim(codigo)) > 0),
    nome varchar(120) NOT NULL CHECK (char_length(trim(nome)) >= 2),
    preco decimal(10,2) NOT NULL CHECK (preco > 0),
    estoque int NOT NULL DEFAULT 0 CHECK (estoque >= 0),
    ativo boolean NOT NULL DEFAULT true
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

CREATE TABLE movimento_estoque (
    id bigint NOT NULL AUTO_INCREMENT PRIMARY KEY,
    produto_id int NOT NULL,
    usuario_id int NOT NULL,
    quantidade int NOT NULL CHECK (quantidade <> 0),
    motivo varchar(20) NOT NULL CHECK (motivo IN ('Reposicao', 'Venda')),
    criado_em datetime(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
    FOREIGN KEY (produto_id) REFERENCES produto(id),
    FOREIGN KEY (usuario_id) REFERENCES usuario(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
