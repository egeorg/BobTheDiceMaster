# Algorithm used by Bob the dice master for School

## Formalize the problem

AI for School can be expressed as a single function that maps game context to a decision. Context consists of:
A set of combination types that can be scored
Latest roll results
Number or rerolls left

> For solitare version it would be enough, but for multiplayer it should also include information about other players. It may be important for example if the other player has score significantly higher, and in this case it makes sense to risk more. though it looks like it would be much more complex. For example there is whole a 2004 scientific paper that analyze such condition for much simplier dice game Pig: [Optimal Play of the Dice Game Pig](https://cupola.gettysburg.edu/csfac/4/).

Decision can be one of the following:
Reroll dice with dice values to reroll specified
Score a combination, with a specific combination specified
Cross out a combination, with a specific combination specified

## Profit function

Now let's assign a real number for each decision, which would represent an average profit of this decision.

For each of the decisions it would be defined differently.
> For Score, a naive profit is amount of points it yields, but that would not be correct.
> Imagine that you get a roll 6,6,6,6,6. It can be scored as poker or as trash. Both will yield the same amount of points. But it's better to score poker because it is much less frequent combination. Trash can be scored for any roll, but poker is crossed out it almost every game.

More complex, but not optimal profit function is used by Bob.

### Average score

Let's define the average score for a combination. Imagine that you aim to score a specific combination from scratch. It it is the last round of the game when only a single combination left not scored.

Then an average amount of points that it will yield given an optimal strategy is used will be the average score of the combination. Optimal strategy is determined by brute force. TODO: how??

The average score does not depend on game context, and I use precomputed values to save time.
TODO: why grades are positive??
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

This number show approximately how much benefit we on average get from each combination during the game.

### Profit function for a cross out decision

TODO: explain things like AverageScore(combination)
TODO: combination vs combination type
After crossing out a combination, a player can't score it anymore, hence (on average) a player loses an AverageScore(combination)) points. It means that the score of CrossOut(combinationType) can be negative average score of that combination: -AverageScore(combinationType).

### Profit function for a score decision

It makes sense to score a combination if scoring it right now it will yield more points than trying to roll that combination from scratch, i.e. score of Score(combinationType) can be defined as a difference between the amount of points that it yields and average score of a the combination type: Score(combintionType) - AverateScore(combinationType).

### Profit function for a reroll decision

Here where it gets interesting.
A reroll profit can be defined recursively:

Now we know how to calculate profit for "Score" and "Cross out" decisions. If no rerolls are left, we can already choose the best decision by iterating across all the available combinations and choosing the decision+combination type that yields the most profit. Let's call it Profit(roll, 0) where 0 means that zero rerolls are left. It would be the base of recursion.

1) Across all the possible reroll results, return sum of Probability(rerollResult) \* Profit(rerollResult, rerollsLeft - 1).

## Finding an optimal decision

Now that the profit function is defined, finding an optimal decision is straightforward using brute force:

1) Iterate across all the available combinations, calculate Profit(roll, Score(combination))
2) Iterate across all the available combinations, calculate Profit(roll, CrossOut(combination))
3) Iterate across all possible rerolls (<=> all possible rerolls), calculate Profit(roll, Reroll(reroll))

Chose the highest value and return corresponding decision.

## Optimization