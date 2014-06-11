rem // To Migrate to most recent Migration run: 
rem // Migrate.cmd
rem // To Rollback to a specific version run:
rem // Migrate.cmd -t rollback:toversion --version <VERSION_NUMBER>
Migrate.exe -a Diebold.Migrations.dll -db sqlserver2008 -c "Data Source=10.153.108.228;Initial Catalog=portaldev;User ID=videomonitoring;Password=I9yKvQ35WB" --timeout 6000 %*