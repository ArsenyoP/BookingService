# Code Review 14/06/2026

1. Можна перевести кожен неймспейс у scoped namespace;

2. Якщо створюємо серілог логгер:

```csharp
Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
```


То треба врапнути код до цього у трай файналлі й у файналлі клірити логгер!

```csharp
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                
            }
            finally
            {
                Log.CloseAndFlush();
            }
```

3. Прибрати в docker-compose.yml сенситив:

```yaml
Jwt__SigningKey
```

4. Like за аутпут кеш

5. 