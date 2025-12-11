#!/usr/bin/env python3
import json
from collections import Counter

# Load the PlayerData.json file
with open('Assets/PlayerData/PlayerData.json', 'r') as f:
    data = json.load(f)

# Get Pack_01 data
pack_01 = data['PacksDict']['Pack_01']
cards = pack_01['CardDatas']

# 1. Total number of cards
total_cards = len(cards)

# 2. Count by DeckCounts array position
# DeckCounts: [Mage, Fate, Enchanted, Mystic, Arcane, Quest, ?]
deck_counts_totals = [0, 0, 0, 0, 0, 0, 0]
for card in cards:
    deck_counts = card.get('DeckCounts', [])
    for i, count in enumerate(deck_counts):
        if count > 0:
            deck_counts_totals[i] += 1

# 3. Count by CardType value__
# 1=Mage, 2=Fate, 3=Level, 4=Challenge, 5=Structure, 6=Creature, 7=Item, 8=Magic, 9=Resource, 10=Curse, 11=Blessing
card_type_counter = Counter()
for card in cards:
    card_type = card.get('CardType', {}).get('value__')
    if card_type:
        card_type_counter[card_type] += 1

# 4. Level distribution
level_counter = Counter()
for card in cards:
    level = card.get('Level', 0)
    level_counter[level] += 1

# CardType mapping
card_type_names = {
    1: "Mage",
    2: "Fate",
    3: "Level",
    4: "Challenge",
    5: "Structure",
    6: "Creature",
    7: "Item",
    8: "Magic",
    9: "Resource",
    10: "Curse",
    11: "Blessing"
}

# Deck position names
deck_position_names = ["Mage", "Fate", "Enchanted", "Mystic", "Arcane", "Quest", "Unknown_6"]

# Print report
print("=" * 60)
print("PACK_01 ANALYSIS REPORT")
print("=" * 60)
print()

print(f"1. TOTAL NUMBER OF CARDS: {total_cards}")
print()

print("2. CARD COUNT BY DECK POSITION (DeckCounts array):")
print("-" * 60)
for i, count in enumerate(deck_counts_totals):
    if i < len(deck_position_names):
        print(f"   {deck_position_names[i]}: {count}")
print()

print("3. CARD COUNT BY CARD TYPE:")
print("-" * 60)
for card_type in sorted(card_type_counter.keys()):
    type_name = card_type_names.get(card_type, f"Unknown_{card_type}")
    count = card_type_counter[card_type]
    print(f"   {type_name} (value__ {card_type}): {count}")
print()

print("4. LEVEL DISTRIBUTION:")
print("-" * 60)
for level in sorted(level_counter.keys()):
    count = level_counter[level]
    print(f"   Level {level}: {count}")
print()

print("=" * 60)
print("ADDITIONAL STATISTICS")
print("=" * 60)
# Show some card names by type
print()
print("Sample cards by type:")
print("-" * 60)
for card_type in sorted(card_type_counter.keys()):
    type_name = card_type_names.get(card_type, f"Unknown_{card_type}")
    sample_cards = [card['CardName'] for card in cards if card.get('CardType', {}).get('value__') == card_type][:3]
    print(f"   {type_name}: {', '.join(sample_cards)}")
