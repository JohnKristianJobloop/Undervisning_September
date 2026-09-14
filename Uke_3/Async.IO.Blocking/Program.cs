using Async.Common;
using Async.IO.Blocking;

// Del 1: hvorfor async finnes i det hele tatt.
// Vi går fra "alt går via OS" til "blokkering koster" til "flere tråder er ikke løsningen".
await Menu.Run(args,
("Operativsystemresurser", Step1_OS.Run),
("Blocking Code", Step2_BlockingCode.Run),
("Splitte til flere tråder skalerer dårlig", Step3_Threads.Run));