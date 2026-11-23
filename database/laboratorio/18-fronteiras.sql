-- Aula 22. Somente na branch/banco didático com scripts 001–003 e um Caixa.
-- Dados persistem para consultar na UI. Execute uma vez; não apaga dados anteriores.
BEGIN;
DO $$
DECLARE
    operador integer;
    produto_teste integer;
    venda_teste integer;
    instante timestamptz;
BEGIN
    SELECT id INTO operador FROM usuario WHERE perfil='Caixa' ORDER BY id LIMIT 1;
    IF operador IS NULL THEN RAISE EXCEPTION 'Prepare uma conta Caixa antes da massa'; END IF;
    IF EXISTS (SELECT 1 FROM produto WHERE codigo='LAB18-FRONTEIRA') THEN
        RAISE EXCEPTION 'Massa LAB18 ja existe; use outra branch vazia para repetir';
    END IF;
    INSERT INTO produto(codigo,nome,preco,estoque)
        VALUES ('LAB18-FRONTEIRA','Produto de fronteira',8.50,20) RETURNING id INTO produto_teste;
    INSERT INTO movimento_estoque(produto_id,usuario_id,quantidade,motivo)
        VALUES (produto_teste,operador,20,'Reposicao');
    FOREACH instante IN ARRAY ARRAY[
        TIMESTAMPTZ '2026-10-15 23:59:59.999999-03',
        TIMESTAMPTZ '2026-10-16 00:00:00-03',
        TIMESTAMPTZ '2026-10-16 23:59:59.999999-03',
        TIMESTAMPTZ '2026-10-17 00:00:00-03'
    ] LOOP
        INSERT INTO venda(operacao,usuario_id,criado_em,total,recebido)
            VALUES (gen_random_uuid(),operador,instante,8.50,10) RETURNING id INTO venda_teste;
        INSERT INTO venda_item(venda_id,produto_id,nome,quantidade,preco_unitario)
            VALUES (venda_teste,produto_teste,'Produto de fronteira',1,8.50);
        UPDATE produto SET estoque=estoque-1 WHERE id=produto_teste;
        INSERT INTO movimento_estoque(produto_id,usuario_id,quantidade,motivo,criado_em)
            VALUES (produto_teste,operador,-1,'Venda',instante);
    END LOOP;
END $$;
COMMIT;
-- Dia local fixo, independentemente do fuso da sessão do editor.
SELECT v.id, v.criado_em AT TIME ZONE 'America/Sao_Paulo' AS data_local, v.total
FROM venda v JOIN venda_item i ON i.venda_id=v.id JOIN produto p ON p.id=i.produto_id
WHERE p.codigo='LAB18-FRONTEIRA'
  AND v.criado_em >= (DATE '2026-10-16'::timestamp AT TIME ZONE 'America/Sao_Paulo')
  AND v.criado_em < (DATE '2026-10-17'::timestamp AT TIME ZONE 'America/Sao_Paulo')
ORDER BY v.criado_em;
-- Esperado: 2 linhas de 8,50, soma 17,00. Estoque 16, soma dos movimentos 16.
-- Na UI, escolha 16/10/2026 nos dois filtros. Outras vendas nesse dia também
-- entram no relatório; prefira banco preparado só para este ensaio.
