# Roteiro de validação com Neon e Windows Forms

Execute apenas em branch/banco **didático isolado**, com dados fictícios. Estes casos exigem interação e acesso ao PostgreSQL. Marque “passou”, “falhou” ou “pendente” e registre resultados observados; a presença deste roteiro não significa que eles já foram executados.

## Preparação

1. Scripts 001, 002 e 003 executados na ordem, uma vez, em banco vazio.
2. `SUPERMERCADO_DB` configurada localmente; TLS com verificação completa.
3. Administrador preparado uma vez; duas contas de caixa criadas.
4. Produtos fictícios: arroz a 25,90 e feijão a 8,50, com estoque 10 cada; para concorrência, um produto separado com estoque 1.
5. Anote IDs, saldos e contagens iniciais de venda, venda_item e movimento_estoque. Use códigos únicos para não misturar com práticas anteriores.

## Fluxos de interface e integração

| Caso | Ação | Resultado esperado |
| --- | --- | --- |
| Login vazio | Entrar sem dados | Mensagem de preenchimento; sem sucesso |
| Login inválido | Conta inexistente ou senha errada | Mesma mensagem genérica |
| Perfis | Entrar com cada perfil | Janela e funções correspondentes |
| Troca de usuário | Sair do menu, entrar com outra conta | Nova sessão e campo de senha vazio |
| Cadastro | Novo produto com código 000123 | Zeros preservados; saldo inicial zero |
| Duplicidade | Repetir código em outro produto | Recusa, sem inserir segunda linha |
| Edição | Alterar nome/preço | Mesmo ID; saldo não muda |
| Inativação | Cancelar confirmação, depois confirmar | Primeiro preserva ativo; depois inativa |
| Reposição | Adicionar 10, depois 3, em produto novo | Saldo 13; movimentos +10 e +3 |
| Carrinho | Duas unidades de arroz e uma de feijão | Duas linhas e total 60,30 |
| Cancelamento | Limpar carrinho antes de finalizar | Nenhuma venda, saldo preservado |
| Produto inativo | Tentar adicionar | Recusa |
| Estoque | Pedir além do saldo | Recusa; carrinho anterior preservado |
| Dinheiro | Compra 60,30, recebido 100,00 | Total 60,30, troco 39,70 |
| Insuficiência | Recebido menor que total | Sem venda nem baixa |
| Histórico | Alterar preço após uma venda | Venda_item preserva preço/nome originais |
| Relatório | Consultar período com vendas conhecidas | Soma e quantidade correspondentes |
| Período vazio | Datas sem vendas | Zero linhas e total zero |
| Teclado | Tab, Shift+Tab e Enter no login | Ordem correta e um acionamento |
| Fora da IDE | Executar pasta publicada | Mesmos fluxos, com ambiente configurado |

## Concorrência: último item

Abra duas instâncias com contas de caixa diferentes e adicione a mesma unidade de um produto cujo saldo é 1. Os dois carrinhos podem aceitar a inclusão, pois ainda não reservam estoque. Finalize ambos, com chaves diferentes.

Espere uma venda confirmada e outra recusada, saldo final zero e um único movimento de saída. O segundo caixa não pode criar estoque negativo nem receber mensagem de sucesso. Confira também que o item recusado não gerou cabeçalho órfão.

## Idempotência: repetir a mesma operação

No depurador ou num botão temporário do laboratório, guarde um `Guid` em uma variável e chame duas vezes `VendaService.FinalizarAsync` com essa mesma chave, sessão, itens e recebido. Não use dois cliques depois de uma venda concluída: a interface corretamente gera uma chave nova para a próxima compra.

Espere o mesmo ID de venda, mesmo total, uma venda e uma única baixa. Para concorrência da **mesma chave**, faça duas chamadas aguardadas com `Task.WhenAll` e confirme as mesmas contagens. Remova o botão temporário ao terminar.

## Rollback após alteração real

Apenas no banco isolado de testes, sem outros operadores conectados, crie uma falha controlada no registro do movimento. O gatilho será removido no final; ele não deve existir no banco normal da turma.

```sql
CREATE FUNCTION falhar_movimento_laboratorio() RETURNS trigger
LANGUAGE plpgsql AS $$
BEGIN
    IF NEW.motivo = 'Venda' THEN
        RAISE EXCEPTION 'Falha controlada de laboratório';
    END IF;
    RETURN NEW;
END;
$$;
CREATE TRIGGER trg_falhar_movimento_laboratorio
BEFORE INSERT ON movimento_estoque
FOR EACH ROW EXECUTE FUNCTION falhar_movimento_laboratorio();
```

Anote as contagens e saldos e tente finalizar uma compra válida. A falha ocorre **depois de inserir o cabeçalho, inserir o item e executar o UPDATE de estoque**, antes de confirmar. Espere que tudo seja desfeito: contagens e saldos iguais aos iniciais. A tela não pode anunciar sucesso. Por se tratar de uma falha SQL, a referência a apresenta como tentativa a esclarecer.

Depois da conferência, remova somente o gatilho e a função criados para esse teste:

```sql
DROP TRIGGER trg_falhar_movimento_laboratorio ON movimento_estoque;
DROP FUNCTION falhar_movimento_laboratorio();
```

Repita a mesma tentativa pendente com os mesmos dados e chave. Agora ela deve confirmar uma vez. Se o teste for interrompido, remova o gatilho antes de retomar qualquer prática de venda.

## Falha de rede e resultado incerto

Desconecte a rede antes de uma consulta e confira tratamento e retorno do botão. Para observar uma confirmação de resultado incerto, é preciso interromper a comunicação no intervalo entre o commit e a chegada da resposta; simplesmente desligar a rede antes da chamada testa apenas indisponibilidade.

Se a falha ocorrer nesse intervalo, a tela deve preservar Guid, itens e recebido e bloquear nova edição. Restaure a rede e finalize novamente. Consulte `venda.operacao` para confirmar uma venda, não duas. Não afirme ter testado esse intervalo se só observou um timeout antes de abrir conexão.

Se o processo encerrar à força, consulte no banco o Guid anotado antes de reiniciar a compra. Recuperação automática após queda do processo não está implementada. Registre esse limite na demonstração.

## Autorização sem depender do menu

Com uma sessão de caixa, chame diretamente o serviço de produtos, reposição, relatório e criação de usuário no depurador. Cada chamada deve lançar `UnauthorizedAccessException` antes de qualquer SQL. As verificações automáticas também cobrem esses caminhos com fakes; a confirmação no laboratório permite observar ausência de mudanças no banco.

Essa proteção vale para o fluxo normal do cliente. O teste não comprova segurança contra aplicativo modificado ou credencial de banco obtida na máquina: esse cenário exige API com autorização no servidor.

## Registro

Para cada caso, preencha: data, branch, preparação, ação, esperado, observado, estado e evidência. Não registre segredos. Um teste pendente de rede, conta ou segunda máquina permanece pendente no relatório da turma.
