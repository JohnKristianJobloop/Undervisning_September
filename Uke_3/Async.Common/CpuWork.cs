namespace Async.Common;

// CPU-arbeid: her jobber tråden faktisk. Motsatt av I/O, der tråden bare venter.
// Nyttig kontrast: async hjelper ikke mot dette, det er ingen venting å gi fra seg.
public static class CpuWork
{
    // Teller primtall under limit ved prøvedivisjon. Ren regning, ingen OS-kall.
    public static int CountPrimes(int limit = 100000)
    {
        int found = 0;
        for (var number = 2; number < limit; number++)
        {
            var isPrime = true;
            for(var divisor = 2; divisor * divisor <= number; divisor++)
            {
                if (number % divisor == 0)
                {
                    isPrime = false;
                    break;
                } 
            }
            if (isPrime)
            {
                found++;
            }
        }
        return found;
    }
}