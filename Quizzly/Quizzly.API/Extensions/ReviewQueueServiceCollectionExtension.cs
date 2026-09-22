using Quizzly.API.Data;
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

            // Vi ber om interfacet, containeren leverer JSON-implementasjonen.
            // Skal vi bytte til database senere, endrer vi bare denne linjen.
            services.AddSingleton<IQuestionStore, JsonQuestionStore>();

            // Singleton = én kø for hele appen. Det MÅ være singleton her:
            // en kø per forespørsel ville glemt alt mellom hvert spørsmål.
            services.AddSingleton<IReviewQueue<Question>, ReviewQueue<Question>>();

            services.AddSingleton<ReviewQueueService>();

            // Kjører ved oppstart og fyller køen.
            services.AddHostedService<ReviewQueueLoader>();

            // Returnerer services slik at kall kan kjedes sammen.
            return services;

        }
    }
}
