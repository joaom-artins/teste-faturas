## REQUISITOS

1. .NET 8
2. Docker

## CONFIGURANDO O AMBIENTE

1. Inicie o Docker;
2. Abra o terminal, acesse a pasta do projeto e execute: `docker-compose up -d`, isso fará com que os containers necessários para o funcionamento local do projeto seja iniciados
3. Abra o projeto com o VS Code e pressione F5 para rodar o projeto,todas as migrações serão aplicadas e seu projeto estára pronto para ser utilizado

## REGRAS DE NEGÓCIO

## FATURAS

1. Toda fatura precisa ter um cliente.
2. O vencimento de uma fatura não pode ser anterior ao dia de hoje.
3. Para uma fatura ser criada é necessário que aja pelo menos um item nela.
4. Após ser fechada uma fatura não pode ter itens adicioandos ou removidso dela.
5. Não é possível adicionar ou remover itens de uma fatura vencida.

## FATURA ITENS

1. O valor de um item nunca pode ser negativo.
2. Um item precisa sempre estar ligado a uma fatura.
3. A ordem precisa ser multiplo de 10, unicos na fatura e sem buracos 
4. Se o valor for maior que 1000, o item deverá ter um check vericação
5. Ter uma descriçao breve, obrigatoria, do item.

## PATTERNS UTILIZADOS

1. Notification pattern visando melhorar as resposta da API e também o seu desempenho tendo vista que retornar excessões aumenta o custo computacional.
2. Repository pattern visando fazer uma ponte entre a camada de acesso a dados e a API,assim se em algum momento trocarmos o banco ou ORM ficara mais facil fazer as mudanças sem quebrar o códgio.


# ARQUITETURA

Foi utilizado uma arquitetura dividida em 7 camadas com cada uma tendo sua responsabilidade o que facilita na manutenção e atualização do código.
1. API: Essa camada é onde estão localizada as controllers,filtros e a program.cs.
2. BUSINESS: Aqui estão localizadas todas as regras de negócio do projeto para validar as entidades.
3. COMMONS: Aqui estão funções comuns a toda a aplicação como configuração do notification e middlewares.
4. ENTITES: Aqui estão as models,requests,responses,validators e tudo relacionado as entidades. 
5. PERSISTENCE: Está é a camada responsável por todo acesso a dados da aplicação,aqui está configurado o DbContext que cuida do banco de dados,migrations,repositories e unit of work que é utilizado para cuidar dos commits e também das transações.
6. SERVICES: Aqui estão os métodos que vão ser chamados nas controllers e onde toda a lógica está centrada.
7. TEST: Está é a camada responsável pelos testes da aplicação,ela testa os métodos da services.
