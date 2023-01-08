# Algorithm used by Bob the dice master for School

## Formalize the problem

AI for School can be expressed as a single function that maps game context to a decision. Context consists of:
A set of combinations that can be scored
Latest roll results
Number or rerolls left

> For solitare version it would be enough, but for multiplayer it should also include information about other players. It may be important for example if the other player has score significantly higher, and in this case it makes sense to risk more. Though it looks like it would be much more complex. For example there is whole a 2004 scientific paper that analyze such condition for much simplier dice game Pig: [Optimal Play of the Dice Game Pig](https://cupola.gettysburg.edu/csfac/4/).

Decision can be one of the following:
- Reroll dice with dice values to reroll specified
- Score a combination, with a specific combination specified
- Cross out a combination, with a specific combination specified

## Profit function

Now let's assign a real number for each decision, which would represent an average profit of this decision.

For each of the decisions it would be defined differently.
> For Score, a naive profit is amount of points it yields, but that would not be correct.
> Imagine that you get a roll 6,6,6,6,6. It can be scored as poker or as trash. Both will yield the same amount of points. But it's better to score poker because it is much less frequent combination. Trash can be scored for any roll, but poker is crossed out in almost every game.

More complex, but not optimal profit function is used by Bob.

### Grades score

Negative score of grades makes it harder to compare them to other combinations. Note that the game will be essentially the same if grades score will be positive, and will be calculated as sum of the dice values of corresponding grade, e.g. sum of all 6 dice for 6-th grade. The only thing that would change will be the final score that would be greater by 105 points, i.e. greater by k\*5 for each grade number k, k=1..6 (105 = Sum(k=1..6, k\*5)).

### Average profit

Let's define the average profit for a combination. Imagine that you aim to score a specific combination from scratch. It is the last round of the game i.e. only that single combination is left.

Then an average amount of points that it will yield given an optimal strategy (average across all the possible dice roll results) is used will be the average score of the combination. Algorithms for optimal strategy for a single target combination is straightforward and will not be described here.

The average score does not depend on game context, and I use precomputed values to save time.

Here are the precomputed average profit by combinations:
| Combination | Average score |
|-|-|
| Grade 1 | 2.1064814814814756 |
| Grade 2 | 4.2129629629629575 |
| Grade 3 | 6.3194444444444455 |
| Grade 4 | 8.425925925925913 |
| Grade 5 | 10.532407407407392 |
| Grade 6 | 12.6388888888889 |
| Pair | 15.865270870457234 |
| Set | 11.619899714332302 |
| Two pairs | 16.37926613378522 |
| Full | 7.702210020138647 |
| Care | 4.853220771449692 |
| Little straight | 4.2452235691873 |
| Big straight | 6.367835353780939 |
| Poker | 0.8804221388169133 |
| Trash | 35.100630144032884 |

This number show approximately how much benefit we on average get from each combinations during the game.

### Profit function for a cross out decision

After crossing out a combination, a player can't score it anymore, hence (on average) a player loses an AverageScore(combination)) points. It means that the score of CrossOut(combination) can be a negative average score of that combination: -AverageScore(combination).

### Profit function for a score decision

It makes sense to score a combination if scoring it right now it will yield more points than trying to roll corresponding combination from scratch, i.e. profit of Score(combination) can be defined as a difference between the amount of points that it yields and average profit of the combination: Score(combination) - AverageScore(combination).

### Profit function for a reroll decision

Here where it gets interesting.
A reroll profit can be defined recursively:

Now we know how to calculate profit for "Score" and "Cross out" decisions. If no rerolls are left, we can already choose the best decision by iterating across all the available combinations and choosing the decision+combination that yields the most profit. Let's call it Profit(roll, 0) where 0 means that zero rerolls are left. It would be the base of recursion.

1) Across all the possible reroll results, return sum of Probability(rerollResult) \* Profit(rerollResult, rerollsLeft - 1).

## Finding an optimal decision

Now that the profit function is defined, finding an optimal decision is straightforward using brute force:

1) Iterate across all the available combinations, calculate Profit(roll, Score(combination))
2) Iterate across all the available combinations, calculate Profit(roll, CrossOut(combination))
3) Iterate across all possible rerolls, calculate Profit(roll, Reroll(reroll))

Chose the highest value and return corresponding decision.

## Optimization
How much time such algorithm takes?

Interesting parameters that affects performance are 1 - number of rerolls left, and 2 - number of available combinations. There are always 5 dice and there 6 dice faces.
Without additional modifications, assymptotic is O(n\*C^k) where n is amount of available combinations, and k is number of rerolls left.

### Optimization 1: get rid of duplicates in reroll results.
Imagine rerolling 2 dice. Reroll results 1,6 and 6,1 yield to the same dice values, but when using the naive brute force this result is calculated twice. Instead of 6^k reroll values, less can be considered. Namely it is amount of k-combinations with repetitions of 6 elements, or C(6 + k - 1).

Such optimization does not affect assymptotics, still it's O(n\*C^k). But it affects constant C in expression n\*C^k, for 3 rerolls it reduces number of operations ~100 times.

### Optimization 2: dynamic programming.

Profit(rerollResult, rerollsLeft) is called multiple times with the same arguments, let's store the results in a dictionary that basically maps game context (available combinations, roll and rerollsLeft) to an optimal decision.

Now the algorithm becomes O(n\*k) on time and O(n\*k) on memory.

How big the dictionary will be, it is fine for an in-browser application?

There are only C(5 + 6 - 1, 5) = С(10, 5) = 252 available distincs combinations of 5 d6 dice. There may be 1,2 or 3 rerolls. For a single decision calculation, it will be 252\*3\*(size of a decision) = 756\*(size of a decision) bytes for a single availableCombinations value, hence, for a single decision calculation. In other words, its size would be order of kilobytes, that's completely fine, given that an almost empty blazor webassembly projects takes megabytes.

### Optimization 3: precompute all the decisions.

If implemented, it will be:
Constant on time, constant on memory, though it will take around k_max\*2^n_max of storage, where k_max is maximum number of rerolls (3) and n_max number of combinations (15)

Decisions for all possible game contexts can be precomputed, but how much space will the resulting lookup table get, would it be feasible for in-browser web app?

Game context is a triple (availableCombinations, currentRoll, rerollsLeft)

#### Idea 1: Split the data to several files and compress them as a zip archive.
Compress so that they take less memory, and split to several files so that a required part can be extracted without extracting much of not needed information.

#### Idea 2: Store only decision. Game context can be identified by file name and position of a decision.
Decision can be stored in a single byte. 2 bits for a decision type (one of 3: Reroll, Score, Cross Out) and 5 bytes for a decision payload: for Score and Cross Out, it's one of 15 combinations, which basically can be represented as a number 1-15, or 4 bits. For Reroll, it's a subset of 5 elements (some of the 5 dice that will re rerolled), which can be represented by 5 bits.

For example, file name can represent a (current roll, rerolls left) pair and be something like 12246_3.bin for a dice roll (1,2,2,4,5) and 3 rerolls left. availableCombinations is a subset of a 15 elements set (some of 15 possible combinations that are available), which can be represented as 15-bits or a number from 0 to 2^15 - 1.
Each file would be exactly 2^15 bytes = 32KB (uncompressed), where each byte represents the availableCombinations that corresponds to its position.

There would be 752 files, so all the file metadata does not take much space.

In total, the zip archive takes 643KB, which is completely acceptable for an in-browser application.