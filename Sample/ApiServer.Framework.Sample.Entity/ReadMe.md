反向命令, 在这个目录下执行

```
dotnet ef dbcontext scaffold "server=localhost;database=sample;uid=root;pwd=123456;" "Pomelo.EntityFrameworkCore.MySql" -o Models -c ApplicationDbContext -d -f

```