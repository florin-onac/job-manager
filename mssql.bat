docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=@dm!nistrAt0r" -p 1433:1433 --name mssql -v C:/mssql/data:/var/opt/mssql/data -v C:/mssql/log:/var/opt/mssql/log -d mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04