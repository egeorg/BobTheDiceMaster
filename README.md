
# School

## Summary

School is a dice game, and it bears some similarity to [Yahtzee](https://en.wikipedia.org/wiki/Yahtzee) and similar games ([Yacht](https://en.wikipedia.org/wiki/Yacht_(dice_game)), [Yatzy](https://en.wikipedia.org/wiki/Yatzy), [Generala](https://en.wikipedia.org/wiki/Generala), [Yamb](https://en.wikipedia.org/wiki/Yamb_(game)), [Poker dice](https://en.wikipedia.org/wiki/Poker_dice)). I did not find anything well-known with the same rules as in the school.

It's fun to play in a company of a several people, but it also can be played by two people or even as a solitaire by a single player.

## Rules

You have five d6 dice to roll.
You roll the dice, then you can either score a specific combination or reroll any amount of dice, then again either score a specific combination or reroll any dice one more time. After the second reroll you have to either score a combination or remove a combination from the list of combinations that you can score. Just like Yahtzee, it's played with a sheet of paper and a pen or pencil, so I may refer to removing as crossing it out,

The goal is to score as much points as possible.
You get the points by scoring combinations if your dice rolled in a specific way for each combination, more on that later.

Each combination can be scored only once. Each combination has to be scored or crossed out, thus each player has exactly 15 turns.

### Combinations

There are two types of combinations:
1. Grades (as grades in school, 1-6. Hence the name of the game)
2. All other combinations, 9 in total

#### Non-grade combinations

| Combination | Description | Example |
|-|-|-|
| Pair | Two dice of the same value | 1,2,3,5,5 |
| Set | Three dice of the same value | 1,2,4,4,4 |
| Two pairs | Two dice of the same value and another two dice of the same value | 1,2,2,5,5 |
| Full | Two dice of the same value and another three dice of the same value | 1,1,4,4,4 |
| Care | Four dice of the same value | 2,3,3,3,3 |
| Little straight | One of each value from 1 to 5 | 1,2,3,4,5 |
| Big straight | One of each value from 2 to 6 | 2,3,4,5,6 |
| Poker | Five dice of the same value | 6,6,6,6,6 |
| Trash | Any dice values | 3,5,4,5,6 |

#### Grades score

Any five dice roll can be scored as any grade. But unlike other combinations, grades don't yield points, grades take them away. It can be represented as negative score.
For grade n where n is from 1 to 6, it takes away n points for each dice which value is not n.

Examples:

> Roll 1,2,6,6,6, if player scores 6th grade, will yield -12 points (2 non-6 dice).

> Roll 2,3,4,5,5, if player scores 1th grade, will yield -5 points (5 non-1 dice).

#### Non-grade combinations score

All other combinations can be scored only if all the dice from that combination present in the roll.
They yield number of points equal to sum of all the dice that belong to the combination chosen.
If any non-grade combination is scored without rerolls, its score is doubled.

Examples:

> Roll 6,6,6,6,6, if player scores pair of 6, will yield 12 points (24 if rolled from the first try). If player scores care of 6, it will yield 24 points (48 without rerolls).

> Little straight always 1+2+3+4+5=15 points if it was scored after rerolls and 30 if it was scored without rerolls.

> Big straight always yields 2+3+4+5+6=20 points if it was scored after rerolls and 30 if it was scored without rerolls.

#### Game progress

For each player, game consists of two parts:
1. School. At this point, player can't score anything except for grades. To graduate from school and be able to score other combinations, player has to score at least three grades (any grades). Hence this part always consists of three turns because you can't cross out a grade.
2. After graduating from school, player can score any combinations (including remaining grades).

The whole game consists of 15 rounds. During each round, each player takes their turn one by one in fixed order, just like in almost any tabletop game. In the end, a player with the highest score wins.
