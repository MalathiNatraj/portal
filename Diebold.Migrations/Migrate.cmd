rem // To Migrate to most recent Migration run: 
rem // Migrate.cmd
rem // To Rollback to a specific version run:
rem // Migrate.cmd -t rollback:toversion --version <VERSION_NUMBER>
Migrate.exe -a Diebold.Migrations.dll -db sqlserver2008 -c "Data Source=localhost;Initial Catalog=Diebold;User ID=sa;Password=AAbb00" --timeout 6000 %*