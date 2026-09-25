using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Quizzly.API.Data;
using Quizzly.API.Models;
using Quizzly.API.Services;
using Quizzly.Core.Interfaces;
using Quizzly.Core.Models;

namespace Quizzly.API.Extensions;

// Samler all registrering av tjenester for review-køen på ett sted,
// så Program.cs bare trenger én linje: builder.Services.AddReviewQueue(...).
public static class ReviewQueueServiceCollectionExtension
{
    // extension-blokk: alt inni her blir metoder du kan kalle rett på IServiceCollection,
    // som om de var en del av typen fra før.
    extension(IServiceCollection services)
    {
        public IServiceCollection AddReviewQueue(IConfiguration configuration)
        {
            // Kobler klassen QuestionStoreOptions til seksjonen "QuestionStore" i appsettings.json.
            services.Configure<QuestionStoreOptions>(configuration.GetSection(QuestionStoreOptions.SectionName));


            // Vi trenger verdiene allerede nå (før appen er bygget) for å velge implementasjon.
            // Manglende seksjon gir standardverdiene (?? new()).
            var storeOptions = configuration.GetSection(QuestionStoreOptions.SectionName).Get<QuestionStoreOptions>() ?? new();

            // Vi ber om interfacet, konfigurasjonen bestemmer hvilken implementasjon containeren leverer.
            switch (storeOptions.Provider)
            {
                case QuestionStoreProvider.Json:
                    services.AddSingleton<IQuestionStore, JsonQuestionStore>();
                    break;
                case QuestionStoreProvider.Sqlite:
                    services.AddSqliteQuestionStore(configuration);
                    break;
            }

            // Singleton = én kø for hele appen. Det MÅ være singleton her:
            // en kø per forespørsel ville glemt alt mellom hvert spørsmål.
            services.AddSingleton<IReviewQueue<Question>, ReviewQueue<Question>>();

            services.AddSingleton<ReviewQueueService>();

            // Kjører ved oppstart og fyller køen.
            services.AddHostedService<ReviewQueueLoader>();

            // Returnerer services slik at kall kan kjedes sammen.
            return services;

        }
        // Alt som trengs for å bruke SQLite som lagring.
        private void AddSqliteQuestionStore(IConfiguration configuration)
        {
           // Factory i stedet for AddDbContext: store-klassene er singleton og lager en kortlevd DbContext per operasjon.
           services.AddDbContextFactory<QuizDbContext>((serviceProvider, options) =>
           {
               var connectionString = configuration.GetConnectionString("QuizDb") ?? throw new InvalidOperationException("Mangler ConnectionString:QuizDb i appsettings.json");
               // Gjør den relative stien (Data/quiz.db) om til en full sti fra prosjektmappen,
               // så databasen havner samme sted uansett hvor appen startes fra.
               SqliteConnectionStringBuilder builder = new(connectionString);
               var contentRoot = serviceProvider.GetRequiredService<IHostEnvironment>().ContentRootPath;
               builder.DataSource = Path.Combine(contentRoot, builder.DataSource);

               options.UseSqlite(builder.ToString());
           });

           services.AddSingleton<IQuestionStore, SqliteQuestionStore>();

           // JSON-storen registreres fortsatt direkte, fordi initializeren bruker den til å seede databasen.
           services.AddSingleton<JsonQuestionStore>();

           // Fyller databasen fra JSON ved oppstart hvis den er tom.
           services.AddHostedService<QuizDatabaseInitializer>();
        }
    }
}
