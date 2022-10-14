using Prime;
using System.Globalization;

var range = GetRange();

while (range.Lo > 0 && range.Hi > 0)
{
    try
    {
        var ctc = new CancellationTokenSource();

        ctc.Cancel();

        Task<int>[] tasks =
        {
            Task.Run(() => PrimeCount(range.Lo, range.Hi,ctc.Token),ctc.Token),
            Task.Run(() => PrimeCount(range.Lo, range.Hi,ctc.Token),ctc.Token),
            Task.Run(() => PrimeCount(range.Lo, range.Hi,ctc.Token),ctc.Token)
        };

        ReportTasks(tasks);

        var finisher = await Task.WhenAny(tasks);

        ReportTasks(tasks);

        ctc.Cancel();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        ReportTasks(tasks);

        Console.WriteLine();
        Console.WriteLine($"The calculation was done by algorithm number {Array.IndexOf(tasks, finisher)}");
        Console.WriteLine($"There are {finisher.Result:N0} primes between {range.Lo:N0} and {range.Hi:N0}.");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    range = GetRange(); 
}

int PrimeCount(ulong lo, ulong hi, CancellationToken token)
{
    int count = 0;

    var root = Math.Sqrt(hi);
    var rand = new Random();

    var primes = PrimeList.Primes.TakeWhile(p => p <= root).OrderBy(x => rand.Next()).ToArray();

    for (ulong num = lo; num <= hi; ++num)
    {
        token.ThrowIfCancellationRequested();

        if (IsPrime(num))
            ++count;
    }

    return count;

    bool IsPrime(ulong num)
    {
        foreach (ulong x in primes)
        {
            if (num % x == 0)
                return false;
        }

        return true;
    }
}

(ulong Lo, ulong Hi) GetRange()
{
    Console.Write("Lower bound: ");
    var lo = ulong.Parse(Console.ReadLine() ?? "", NumberStyles.AllowThousands);

    Console.Write("Upper bound: ");
    var hi = ulong.Parse(Console.ReadLine() ?? "", NumberStyles.AllowThousands);

    return (lo, hi);
}

void ReportTasks(Task<int>[] tasks)
{
    for (int i = 0; i < tasks.Length; ++i)
    {
        Console.WriteLine($"Task: {i}");
        Console.WriteLine($"IsCompleted: {tasks[i].IsCompleted}");
        Console.WriteLine($"IsCompletedSuccessfully: {tasks[i].IsCompletedSuccessfully}");
        Console.WriteLine($"IsCanceled: {tasks[i].IsCanceled}");
        Console.WriteLine($"IsFaulted: {tasks[i].IsFaulted}");
        Console.WriteLine();
    }
}