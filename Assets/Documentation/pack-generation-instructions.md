# Mage Wars - Pack Generation Instructions


**Last Updated:** December 10, 2025


This document provides guidelines for how to generate pack of cards with balanced number of cards in each of the various decks, as well as balanced Resource to activation Requirements.


---


## Pack Generation Instructions


I want to balance the type and number of cards in each deck (Mage, Fate, Enchanted, Mystic, Arcane and Quest) as well as the Resources and Requirements in the cards in the game.


The card data are created as ScriptableObjects of type CardConfig located in Assets/Resources/Configs/Cards/ in subfolders based on card type.  There is a Scriptable Object called AllSpritesConfig in Assets/Resources/Configs/ with all the possible sprites to use in the game.


However, I only want to use a few resource sprites. The reason is I do not want the resources to be so sparsely scattered through the deck that the chances of every drawing the resource needed for a recipe is ridiculously small.  The resources and recipes should be balanced in such a way that the weaker cards require fewer and/or more common resources to activate.  The more powerful cards require more and/or rarer resources to activate.


Items, Creatures, Magic cards should be relatively easy to craft, with the only factor being their level.


Level cards should be in general harder to craft, and even more difficult as their level increases.
Structure cards should be hard to craft, requiring many and/or rare resources to activate because these cards are contributed to by all players collectively.


Resource cards require resources to craft, but offer a permanent resource.


Some - not all - of the mages should have a permanent resource.  Use your mastery as a game designer to decide if these permanent resources should be more common or rare.


Resource Sprites to Use:
Beast1
Book2
Bug3
Coin_Silver
Coin_Gold
Feather1
Fire1
Flower1
Gem4
Light1
Plant5
Potion2
Seed1


Recipe Sprites to Use:
@OR
@Any1
@Any2
@Any3


It is important to balance the number of each type of card in each deck. 
The Mage deck contains only Mage cards. 
Fate deck contains Fate cards, and a few Curse and Blessing Cards. 
Think of Enchanted as Level 1, Mystic as Level 2 and Arcane as Level 3.  These three decks contain cards of type Item, Creature, Magic, Resource, Curse, Blessing, and Structure.
Quest deck contains Level cards, Challenge cards, some Curse and Blessing cards.


The quantity of Curse and Blessing cards should be relatively low compared to other types of cards.  I don’t want the players to have a bad experience for drawing too many curses.  Curse/Blessing level should be somewhat correlated to deck difficulty - so Arcane deck curses and blessings tend more to higher level, while Mystic deck are medium level and Enchanted tend to be the lowest level.  Fate deck should have a range of low to medium curses and blessings, and Quest deck should have medium to high level.


Generate a pack named “Base Pack 2” with 6 Mages, and with a total of 120 to 130 cards. You are going to modify the file Assets/PlayerData/PlayerData.json, not the card Scriptable Objects themselves.


You may start with the pack with “ID”: “Pack_01”, or create a new pack from scratch if you prefer, using the JSON format.  If any of the Cards have Resources, Permanent Resources or Resurrection requirements, erase them and start your balancing algorithm from scratch.  You may delete cards from this pack.  Please error-check the JSON when done and fix errors.


---


**Document Version:** 1.0 
**Next Review:** March 2026