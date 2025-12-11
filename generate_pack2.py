#!/usr/bin/env python3
"""
Script to generate Base Pack 2 card selection from the Default pack
"""
import json
import copy

# Read the PlayerData.json file
with open('/Users/davidbarile/Unity Projects/Mage Wars/Assets/PlayerData/PlayerData.json', 'r') as f:
    data = json.load(f)

# Get Default pack cards
default_pack = data['PacksDict']['Default']['CardDatas']

# Card selection plan for Base Pack 2 (125 cards total)
# DeckCounts: [None, Mage, Fate, Enchanted, Mystic, Arcane, Quest]

# Helper function to find card by name pattern
def find_cards(card_list, name_pattern=None, card_type=None, level=None, limit=None):
    results = []
    for card in card_list:
        if name_pattern and name_pattern.lower() not in card['CardName'].lower():
            continue
        if card_type is not None and card['CardType']['value__'] != card_type:
            continue
        if level is not None and card['Level'] != level:
            continue
        results.append(card)
        if limit and len(results) >= limit:
            break
    return results

# Build Pack_02 card list
pack2_cards = []

# 1. MAGE DECK (6 cards) - Select 6 different mages
mage_indices = [0, 1, 3, 5, 10, 4]  # Select specific indices to get 6 different mages

for idx in mage_indices:
    mages = find_cards(default_pack, card_type=1)
    if idx < len(mages):
        card = copy.deepcopy(mages[idx])
        card['DeckCounts'] = [-1, 1, -1, -1, -1, -1, -1]
        pack2_cards.append(card)

# 2. FATE DECK (13 cards) - 10 Fate cards + 2 Curses + 1 Blessing
fate_cards = find_cards(default_pack, card_type=2, limit=10)  # Fate type
for card in fate_cards:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, 1, -1, -1, -1, -1]
    pack2_cards.append(new_card)

# Add 2 Level 1 curses to Fate deck
curses = find_cards(default_pack, card_type=10, level=1, limit=2)  # Curse type
for card in curses:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, 1, -1, -1, -1, -1]
    pack2_cards.append(new_card)

# Add 1 Level 1 blessing to Fate deck
blessings = find_cards(default_pack, card_type=11, level=1, limit=1)  # Blessing type
for card in blessings:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, 1, -1, -1, -1, -1]
    pack2_cards.append(new_card)

# 3. ENCHANTED DECK (38 cards) - Mostly Level 1
# 8 Creatures (L1)
creatures_l1 = find_cards(default_pack, card_type=6, level=1, limit=8)
for card in creatures_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, 1, -1, -1, -1]
    pack2_cards.append(new_card)

# 10 Items (L1)
items_l1 = find_cards(default_pack, card_type=7, level=1, limit=10)
for card in items_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, 1, -1, -1, -1]
    pack2_cards.append(new_card)

# 5 Magic (L1)
magic_l1 = find_cards(default_pack, card_type=8, level=1, limit=5)
for card in magic_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, 1, -1, -1, -1]
    pack2_cards.append(new_card)

# 10 Resources (L1)
resources_l1 = find_cards(default_pack, card_type=9, level=1, limit=10)
for card in resources_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, 1, -1, -1, -1]
    pack2_cards.append(new_card)

# 3 Curses (L1) - skip the ones already in Fate
curses_enchanted = [c for c in find_cards(default_pack, card_type=10, level=1) if c not in curses][:3]
for card in curses_enchanted:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, 1, -1, -1, -1]
    pack2_cards.append(new_card)

# 2 Blessings (L1) - skip the one already in Fate
blessings_enchanted = [c for c in find_cards(default_pack, card_type=11, level=1) if c not in blessings][:2]
for card in blessings_enchanted:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, 1, -1, -1, -1]
    pack2_cards.append(new_card)

# 4. MYSTIC DECK (42 cards) - Mix of L1 and L2  - Increased from 38 to 42
# 4 Creatures (3 L1, 1 L2)
creatures_mystic_l1 = [c for c in find_cards(default_pack, card_type=6, level=1) if c not in creatures_l1][:3]
for card in creatures_mystic_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

creatures_mystic_l2 = find_cards(default_pack, card_type=6, level=2, limit=1)
for card in creatures_mystic_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

# 10 Items (6 L1, 4 L2)
items_mystic_l1 = [c for c in find_cards(default_pack, card_type=7, level=1) if c not in items_l1][:6]
for card in items_mystic_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

items_mystic_l2 = find_cards(default_pack, card_type=7, level=2, limit=4)
for card in items_mystic_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

# 11 Magic (5 L1, 6 L2)  - Increased from 10 to 11
magic_mystic_l1 = [c for c in find_cards(default_pack, card_type=8, level=1) if c not in magic_l1][:5]
for card in magic_mystic_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

magic_mystic_l2 = find_cards(default_pack, card_type=8, level=2, limit=6)
for card in magic_mystic_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

# 13 Resources (10 L1, 3 L2)  - Increased from 12 to 13
resources_mystic_l1 = [c for c in find_cards(default_pack, card_type=9, level=1) if c not in resources_l1][:10]
for card in resources_mystic_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

resources_mystic_l2 = find_cards(default_pack, card_type=9, level=2, limit=3)
for card in resources_mystic_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

# 4 Curses (3 L1, 1 L2)  - Increased from 3 to 4
curses_mystic_l1 = [c for c in find_cards(default_pack, card_type=10, level=1) if c not in curses and c not in curses_enchanted][:3]
for card in curses_mystic_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

curses_mystic_l2 = find_cards(default_pack, card_type=10, level=2, limit=1)
for card in curses_mystic_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

# 1 Blessing (L1)
blessings_mystic = [c for c in find_cards(default_pack, card_type=11, level=1) if c not in blessings and c not in blessings_enchanted][:1]
for card in blessings_mystic:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, 1, -1, -1]
    pack2_cards.append(new_card)

# 5. ARCANE DECK (31 cards) - Mostly L2-L3  - Increased from 27 to 31
# 4 Creatures (3 L2, 1 L3)  - Increased from 3 to 4
creatures_arcane_l2 = [c for c in find_cards(default_pack, card_type=6, level=2) if c not in creatures_mystic_l2][:3]
for card in creatures_arcane_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

creatures_arcane_l3 = find_cards(default_pack, card_type=6, level=3, limit=1)
for card in creatures_arcane_l3:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

# 6 Items (2 L2, 4 L3)
items_arcane_l2 = [c for c in find_cards(default_pack, card_type=7, level=2) if c not in items_mystic_l2][:2]
for card in items_arcane_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

items_arcane_l3 = find_cards(default_pack, card_type=7, level=3, limit=4)
for card in items_arcane_l3:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

# 6 Magic (2 L2, 4 L3)  - Increased from 5 to 6
magic_arcane_l2 = [c for c in find_cards(default_pack, card_type=8, level=2) if c not in magic_mystic_l2][:2]
for card in magic_arcane_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

magic_arcane_l3 = find_cards(default_pack, card_type=8, level=3, limit=4)
for card in magic_arcane_l3:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

# 11 Resources (8 L1, 1 L2, 2 L3)  - Increased from 9 to 11
resources_arcane_l1 = [c for c in find_cards(default_pack, card_type=9, level=1) if c not in resources_l1 and c not in resources_mystic_l1][:8]
for card in resources_arcane_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

resources_arcane_l2 = [c for c in find_cards(default_pack, card_type=9, level=2) if c not in resources_mystic_l2][:1]
for card in resources_arcane_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

resources_arcane_l3 = find_cards(default_pack, card_type=9, level=3, limit=2)
for card in resources_arcane_l3:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

# 2 Curses (1 L2, 1 L3)
curses_arcane_l2 = [c for c in find_cards(default_pack, card_type=10, level=2) if c not in curses_mystic_l2][:1]
for card in curses_arcane_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

curses_arcane_l3 = find_cards(default_pack, card_type=10, level=3, limit=1)
for card in curses_arcane_l3:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

# 2 Blessings (1 L1, 1 L2)
blessings_arcane_l1 = [c for c in find_cards(default_pack, card_type=11, level=1) if c not in blessings and c not in blessings_enchanted and c not in blessings_mystic][:1]
for card in blessings_arcane_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

blessings_arcane_l2 = find_cards(default_pack, card_type=11, level=2, limit=1)
for card in blessings_arcane_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, 1, -1]
    pack2_cards.append(new_card)

# 6. QUEST DECK (13 cards) - All Level cards
# 4 Level 1 cards
level_cards_l1 = find_cards(default_pack, card_type=3, level=1, limit=4)
for card in level_cards_l1:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, -1, 1]
    pack2_cards.append(new_card)

# 5 Level 2 cards
level_cards_l2 = find_cards(default_pack, card_type=3, level=2, limit=5)
for card in level_cards_l2:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, -1, 1]
    pack2_cards.append(new_card)

# 4 Level 3 cards
level_cards_l3 = find_cards(default_pack, card_type=3, level=3, limit=4)
for card in level_cards_l3:
    new_card = copy.deepcopy(card)
    new_card['DeckCounts'] = [-1, -1, -1, -1, -1, -1, 1]
    pack2_cards.append(new_card)

# Write the pack2 cards to a JSON file for inspection
output = {
    "total_cards": len(pack2_cards),
    "cards": pack2_cards
}

with open('/Users/davidbarile/Unity Projects/Mage Wars/pack2_cards.json', 'w') as f:
    json.dump(output, f, indent=2)

print(f"Generated {len(pack2_cards)} cards for Base Pack 2")
print(f"Card list saved to pack2_cards.json")

# Print summary by deck
deck_names = ['None', 'Mage', 'Fate', 'Enchanted', 'Mystic', 'Arcane', 'Quest']
deck_counts = [0] * 7
for card in pack2_cards:
    for i, count in enumerate(card['DeckCounts']):
        if count > 0:
            deck_counts[i] += count

print("\nDeck Distribution:")
for i, name in enumerate(deck_names):
    if deck_counts[i] > 0:
        print(f"  {name}: {deck_counts[i]} cards")
