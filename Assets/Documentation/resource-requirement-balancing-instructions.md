# Mage Wars - Resource/Requirement Balancing Instructions

**Last Updated:** December 10, 2025

This document provides guidelines for how to balance Resource to activation Requirements.

---

## Pack Generation Instructions

Many cards in the game have Resources listed as icons along the side of the card.  These cards may be played as a form of currency to fulfill the Requirement icons listed on other cards in order to activate them.

I want to balance the Resources and Requirements in the cards in the game, and I need your help as a master card game designer.

There is a Scriptable Object called AllSpritesConfig in Assets/Resources/Configs/ with all the possible sprites to use in the game.

However, I only want to use a few resource sprites. The reason is I do not want the resources to be so sparsely scattered through the deck that the chances of every drawing the resource needed for a recipe is ridiculously small.  The Resources and Recipes should be balanced in such a way that the weaker cards require fewer and/or more common Resources to activate.  The more powerful cards require more and/or rarer Resources to activate.

Fate, Challenge, Curse and Blessing cards have neither Resources, Permanent Resources, nor Requirements.

Mage cards have no Resources, however some - not all - of the mages should have a Permanent Resource.  Use your mastery as a game designer to decide if these Permanent Resources should be more common or rare. The Requirements of a Mage card must be met in order for a player whose Mage has died to Resurrect.

Item cards, Creature cards, Magic cards should be relatively easy to craft, with the only factor being their level.

Level cards should be in general harder to craft, and even more difficult as their level increases.

Structure cards should be hard to craft, requiring many and/or rare Resources to activate because these cards are contributed to by all players collectively.

Resource cards require resources to activate, but once activated, offer various rare Resources, or one Permanent Resource, which is automatically applied to any recipe a player attemts to activate in the future.

Resource (and Permanent Resource) Sprites to Use:
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
All of the above mentioned sprites, plus the following possible options:
@OR
@Any1
@Any2
@Any3
@Any4
@Any5

If two or more Resources are listed in a card's Requirements, all listed Resources must be fulfilled to activate the card.

Additionally, you may use the following icons to incorporate "or" logic to cards, or Any1, Any2, etc.

Here are some Requirement examples:
"Fire1"
"Coin_Gold, Seed1"
"Feather1, Feather1"
"Beast1, @OR, Potion2, Coin_Silver, Coin_Silver"
"Gem4, @OR, @Any3"
"Bug3, @Any2"

The maximum number of Resources for a card should be 4, and up to 6 for Structure or Level cards.

Create a copy of the pack with “ID”: “Pack_02”, and edit it to have "Name": "Base Pack Resources". If any of the existing Cards have Resources, Permanent Resources or Resurrection requirements, erase them and start your balancing algorithm from scratch.  Please error-check the JSON when done and fix errors.

---

**Document Version:** 1.0 