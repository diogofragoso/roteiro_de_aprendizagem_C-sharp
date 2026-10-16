-- Aula 11. Depende de 001. Código é texto para preservar zeros à esquerda.
BEGIN;
CREATE TABLE produto (
    id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    codigo varchar(30) NOT NULL UNIQUE CHECK (length(trim(codigo)) > 0),
    nome varchar(120) NOT NULL CHECK (length(trim(nome)) >= 2),
    preco numeric(10,2) NOT NULL CHECK (preco > 0),
    estoque integer NOT NULL DEFAULT 0 CHECK (estoque >= 0),
    ativo boolean NOT NULL DEFAULT true
);
CREATE TABLE movimento_estoque (
    id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    produto_id integer NOT NULL REFERENCES produto(id),
    usuario_id integer NOT NULL REFERENCES usuario(id),
    quantidade integer NOT NULL CHECK (quantidade <> 0),
    motivo varchar(20) NOT NULL CHECK (motivo IN ('Reposicao', 'Venda')),
    criado_em timestamptz NOT NULL DEFAULT now()
);
COMMIT;
