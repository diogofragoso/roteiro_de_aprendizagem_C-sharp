-- Aula 20. Depende de 001 e 002. Não apague produtos/usuários referenciados.
BEGIN;
CREATE TABLE venda (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    operacao uuid NOT NULL UNIQUE,
    usuario_id integer NOT NULL REFERENCES usuario(id),
    criado_em timestamptz NOT NULL DEFAULT now(),
    total numeric(12,2) NOT NULL CHECK (total > 0),
    recebido numeric(12,2) NOT NULL CHECK (recebido >= total)
);
CREATE TABLE venda_item (
    venda_id integer NOT NULL REFERENCES venda(id),
    produto_id integer NOT NULL REFERENCES produto(id),
    nome varchar(120) NOT NULL,
    quantidade integer NOT NULL CHECK (quantidade > 0),
    preco_unitario numeric(10,2) NOT NULL CHECK (preco_unitario > 0),
    PRIMARY KEY (venda_id, produto_id)
);
CREATE INDEX ix_venda_criado_em ON venda(criado_em);
COMMIT;
