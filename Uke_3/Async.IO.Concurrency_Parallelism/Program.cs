using Async.Common;
using Async.IO.Concurrency_Parallelism;

await Menu.Run(
    args,
    ("Concurrency: mange oppgaver over få tråder.", Step1_Concurrency.Run),
    ("Parallellism: Flere kjerner samtidig", Step2_Parallellism.Run),
    ("Hva passer til hva", Step3_WhatWhere.Run)
);