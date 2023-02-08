using BobPrecomputer;
using BobTheDiceMaster;
using BobTheDiceMaster.Precomputer;

Console.WriteLine(new BobSchoolAverageScorePrecomputer().Precompute());

Console.ReadLine();

BobSchoolDecisionsPrecomputer bobPrecomputer = new BobSchoolDecisionsPrecomputer(new RecursiveBruteForceBob());
bobPrecomputer.Precompute();

Console.WriteLine("Press any key...");
Console.ReadKey();