# Active-Time-Battle

For the code review you should start either in the BattleMenuController- or the BattleActionProcessor-Class.

It is a relatively small prototype, which is why it is suitable for a code review.
By nature of the prototype there is no performance handling involved (despite normal workflow, like caching objects etc.).
But it should give you a good hint how i write and structure my code as well as unity projects in general.

Actual status:
Battle is uni-directional -> the enemies don't act, but they can die.
Therefore no logic is implemented to handle player death. 
You should not attack your own party members because neither UI nor the "battle logic" will care... which means you break the battle after killing one of your members ^^"

Used Unity Version: 2019.4.17f1
