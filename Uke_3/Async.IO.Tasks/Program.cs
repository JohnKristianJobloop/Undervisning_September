// See https://aka.ms/new-console-template for more information
using Async.Common;
using Async.IO.Tasks;

Console.WriteLine("Hello, World!");


// Del 2: verktøyene. Hva en Task er, hva await faktisk gjør,
// hvordan vi får ting til å skje samtidig, og hva som skjer når det går galt.
await Menu.Run(args,
    ("Hva er en Task?", Step1_Tasks.Run),
    ("await er ikke det samme som parallelt", Step2_AwaitNotParallel.Run),
    ("Start alt først, vent etterpå", Step3_StartAll.Run),
    ("Feil og avbrudd", Step4_Feil.Run));