/opt/mssql/bin/sqlservr &

echo "Aguardando o SQL Server iniciar..."
sleep 20

echo "Executando script SQL..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d master -i /usr/src/app/sqlserver.sql

if [[ $? -ne 0 ]]; then
    echo "Falha ao executar o script SQL."
    exit 1
fi

echo "Script SQL executado com sucesso."
wait
