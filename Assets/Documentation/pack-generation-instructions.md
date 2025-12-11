# Mage Wars - Pack Generation Instructions

**Last Updated:** December 10, 2025

This document provides guidelines for how to generate pack of cards with balanced number of cards in each of the various decks, as well as balanced Resource to activation Requirements.

---

## Pack Generation Instructions

Generate a pack named “Base Pack 2” with 6 Mages, and with a total of 120 to 130 cards. Only modify the file Assets/PlayerData/PlayerData.json, not the card Scriptable Objects themselves.

Do not modify the Cards themselves, just distribute them throughout the various decks.

Create a pack that has a good balance of the type and number of cards in each deck (Mage, Fate, Enchanted, Mystic, Arcane and Quest).

-Mage deck contains only Mage cards.

-Fate deck contains Fate cards, and a few Curse and Blessing Cards.  It's very important to balance the Fate cards well, as they determine the number of cards the player draws each turn from the other 4 decks.

Think of:
-Enchanted as Difficulty 1
-Mystic as Difficulty 2 and 
-Arcane as Difficulty 3.
These three decks contain cards of type Item, Creature, Magic, Resource, Curse, Blessing, and Structure.  They can contain cards of level above or below their Difficulty level, but the majority should be of Level equal to the deck difficulty number.

-Quest deck contains Level cards, Challenge cards, a few Structure, and some Curse and Blessing cards. Possibly a few Resource cards.

The quantity of Curse and Blessing cards should be relatively low compared to other types of cards.  I don’t want the players to have a bad experience for drawing too many curses.  Curse/Blessing level should be somewhat correlated to deck difficulty - so Arcane deck curses and blessings tend more to higher level, while Mystic deck are medium level and Enchanted tend to be the lowest level.  Fate deck should have a range of low to medium curses and blessings, and Quest deck should have medium to high level.

You may start with the pack with “ID”: “Pack_01”, or create a new pack from scratch if you prefer, using the JSON format. You may delete cards from this pack.  Please error-check the JSON when done and fix errors.

---

**Document Version:** 1.0