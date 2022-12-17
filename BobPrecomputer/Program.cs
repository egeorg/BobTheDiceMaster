using BobPrecomputer;
using BobTheDiceMaster;

BobSchoolPrecomputer bobPrecomputer = new BobSchoolPrecomputer(new RecursiveBruteForceBob());
bobPrecomputer.Precompute();

Console.WriteLine("Press any key...");
Console.ReadKey();