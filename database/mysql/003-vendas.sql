-- Aula 16: depende de 001 e 002. Executar uma vez no mesmo banco.
CREATE TABLE venda (
    id int NOT NULL AUTO_INCREMENT PRIMARY KEY,
    operacao char(36) NOT NULL UNIQUE,
    usuario_id int NOT NULL,
    criado_em datetime(6) NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
    total decimal(12,2) NOT NULL CHECK (total > 0),
    recebido decimal(12,2) NOT NULL CHECK (recebido >= total),
    FOREIGN KEY (usuario_id) REFERENCES usuario(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

CREATE TABLE venda_item (
    venda_id int NOT NULL,
    produto_id int NOT NULL,
    nome varchar(120) NOT NULL,
    quantidade int NOT NULL CHECK (quantidade > 0),
    preco_unitario decimal(10,2) NOT NULL CHECK (preco_unitario > 0),
    PRIMARY KEY (venda_id, produto_id),
    FOREIGN KEY (venda_id) REFERENCES venda(id),
    FOREIGN KEY (produto_id) REFERENCES produto(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;

CREATE INDEX ix_venda_criado_em ON venda(criado_em);
