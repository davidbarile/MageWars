import json

# Load the PlayerData.json file
with open('/Users/davidbarile/Unity Projects/Mage Wars/Assets/PlayerData/PlayerData.json', 'r') as f:
    player_data = json.load(f)

# Get Default pack cards
default_cards = player_data['PacksDict']['Default']['CardDatas']

# Organize cards by type and level
cards_by_type_level = {}

for card in default_cards:
    card_type = card['CardType']['value__']
    level = card.get('Level', 1)
    
    key = (card_type, level)
    if key not in cards_by_type_level:
        cards_by_type_level[key] = []
    cards_by_type_level[key].append(card)

# Card type values:
# 1 = Mage
# 2 = Fate/Attack
# 3 = Level
# 4 = Challenge
# 5 = Structure
# 6 = Creature
# 7 = Item
# 8 = Magic
# 9 = Resource
# 10 = Curse
# 11 = Blessing

# DeckCounts indices:
# 0 = None
# 1 = Mage
# 2 = Fate
# 3 = Enchanted
# 4 = Mystic
# 5 = Arcane
# 6 = Quest

pack02_cards = []

def add_cards(card_type, level, count, deck_index):
    """Add cards to pack02_cards with modified DeckCounts"""
    key = (card_type, level)
    available = cards_by_type_level.get(key, [])
    
    # Select cards (cycling if needed)
    selected = []
    for i in range(count):
        if available:
            card = available[i % len(available)].copy()
            # Deep copy to avoid modifying original
            card = json.loads(json.dumps(card))
            # Modify DeckCounts
            card['DeckCounts'] = [-1, -1, -1, -1, -1, -1, -1]
            card['DeckCounts'][deck_index] = 1
            selected.append(card)
    
    return selected

# 1. MAGE DECK (index 1): 6 Mage cards
mages = cards_by_type_level.get((1, 1), [])
# Select 6 different mages with varied alignments/elements
mage_selection = []
for i in range(min(6, len(mages))):
    card = json.loads(json.dumps(mages[i]))
    card['DeckCounts'] = [-1, 1, -1, -1, -1, -1, -1]
    mage_selection.append(card)

pack02_cards.extend(mage_selection)
print(f"Added {len(mage_selection)} Mage cards")

# 2. FATE DECK (index 2): 13 cards total
# 10 Fate/Attack cards (value__ 2)
fate_cards = add_cards(2, 1, 10, 2)
pack02_cards.extend(fate_cards)
print(f"Added {len(fate_cards)} Fate/Attack cards")

# 2 Curse cards (Level 1)
curse_fate = add_cards(10, 1, 2, 2)
pack02_cards.extend(curse_fate)
print(f"Added {len(curse_fate)} Curse cards to Fate deck")

# 1 Blessing card (Level 1)
blessing_fate = add_cards(11, 1, 1, 2)
pack02_cards.extend(blessing_fate)
print(f"Added {len(blessing_fate)} Blessing cards to Fate deck")

# 3. ENCHANTED DECK (index 3): 38 cards total (mostly Level 1)
# 8 Creature cards (Level 1)
creature_ench = add_cards(6, 1, 8, 3)
pack02_cards.extend(creature_ench)
print(f"Added {len(creature_ench)} Creature cards to Enchanted deck")

# 10 Item cards (Level 1)
item_ench = add_cards(7, 1, 10, 3)
pack02_cards.extend(item_ench)
print(f"Added {len(item_ench)} Item cards to Enchanted deck")

# 5 Magic cards (Level 1)
magic_ench = add_cards(8, 1, 5, 3)
pack02_cards.extend(magic_ench)
print(f"Added {len(magic_ench)} Magic cards to Enchanted deck")

# 10 Resource cards (Level 1)
resource_ench = add_cards(9, 1, 10, 3)
pack02_cards.extend(resource_ench)
print(f"Added {len(resource_ench)} Resource cards to Enchanted deck")

# 3 Curse cards (Level 1)
curse_ench = add_cards(10, 1, 3, 3)
pack02_cards.extend(curse_ench)
print(f"Added {len(curse_ench)} Curse cards to Enchanted deck")

# 2 Blessing cards (Level 1)
blessing_ench = add_cards(11, 1, 2, 3)
pack02_cards.extend(blessing_ench)
print(f"Added {len(blessing_ench)} Blessing cards to Enchanted deck")

# 4. MYSTIC DECK (index 4): 34 cards total (mostly Level 2, some Level 1)
# 4 Creature cards (3 Level 1, 1 Level 2) - adjusted to reach 125 total
creature_myst_l1 = add_cards(6, 1, 3, 4)
pack02_cards.extend(creature_myst_l1)
creature_myst_l2 = add_cards(6, 2, 1, 4)
pack02_cards.extend(creature_myst_l2)
print(f"Added {len(creature_myst_l1) + len(creature_myst_l2)} Creature cards to Mystic deck")

# 8 Item cards (5 Level 1, 3 Level 2)
item_myst_l1 = add_cards(7, 1, 5, 4)
pack02_cards.extend(item_myst_l1)
item_myst_l2 = add_cards(7, 2, 3, 4)
pack02_cards.extend(item_myst_l2)
print(f"Added {len(item_myst_l1) + len(item_myst_l2)} Item cards to Mystic deck")

# 6 Magic cards (3 Level 1, 3 Level 2)
magic_myst_l1 = add_cards(8, 1, 3, 4)
pack02_cards.extend(magic_myst_l1)
magic_myst_l2 = add_cards(8, 2, 3, 4)
pack02_cards.extend(magic_myst_l2)
print(f"Added {len(magic_myst_l1) + len(magic_myst_l2)} Magic cards to Mystic deck")

# 12 Resource cards (10 Level 1, 2 Level 2)
resource_myst_l1 = add_cards(9, 1, 10, 4)
pack02_cards.extend(resource_myst_l1)
resource_myst_l2 = add_cards(9, 2, 2, 4)
pack02_cards.extend(resource_myst_l2)
print(f"Added {len(resource_myst_l1) + len(resource_myst_l2)} Resource cards to Mystic deck")

# 3 Curse cards (2 Level 1, 1 Level 2)
curse_myst_l1 = add_cards(10, 1, 2, 4)
pack02_cards.extend(curse_myst_l1)
curse_myst_l2 = add_cards(10, 2, 1, 4)
pack02_cards.extend(curse_myst_l2)
print(f"Added {len(curse_myst_l1) + len(curse_myst_l2)} Curse cards to Mystic deck")

# 1 Blessing card (Level 1)
blessing_myst = add_cards(11, 1, 1, 4)
pack02_cards.extend(blessing_myst)
print(f"Added {len(blessing_myst)} Blessing cards to Mystic deck")

# 5. ARCANE DECK (index 5): 22 cards total (mostly Level 2-3)
# 1 Creature card (Level 3)
creature_arc = add_cards(6, 3, 1, 5)
pack02_cards.extend(creature_arc)
print(f"Added {len(creature_arc)} Creature cards to Arcane deck")

# 6 Item cards (2 Level 2, 4 Level 3)
item_arc_l2 = add_cards(7, 2, 2, 5)
pack02_cards.extend(item_arc_l2)
item_arc_l3 = add_cards(7, 3, 4, 5)
pack02_cards.extend(item_arc_l3)
print(f"Added {len(item_arc_l2) + len(item_arc_l3)} Item cards to Arcane deck")

# 3 Magic cards (1 Level 2, 2 Level 3)
magic_arc_l2 = add_cards(8, 2, 1, 5)
pack02_cards.extend(magic_arc_l2)
magic_arc_l3 = add_cards(8, 3, 2, 5)
pack02_cards.extend(magic_arc_l3)
print(f"Added {len(magic_arc_l2) + len(magic_arc_l3)} Magic cards to Arcane deck")

# 7 Resource cards (5 Level 1, 2 Level 3)
resource_arc_l1 = add_cards(9, 1, 5, 5)
pack02_cards.extend(resource_arc_l1)
resource_arc_l3 = add_cards(9, 3, 2, 5)
pack02_cards.extend(resource_arc_l3)
print(f"Added {len(resource_arc_l1) + len(resource_arc_l3)} Resource cards to Arcane deck")

# 3 Curse cards (2 Level 2, 1 Level 3)
curse_arc_l2 = add_cards(10, 2, 2, 5)
pack02_cards.extend(curse_arc_l2)
curse_arc_l3 = add_cards(10, 3, 1, 5)
pack02_cards.extend(curse_arc_l3)
print(f"Added {len(curse_arc_l2) + len(curse_arc_l3)} Curse cards to Arcane deck")

# 2 Blessing cards (1 Level 1, 1 Level 2)
blessing_arc_l1 = add_cards(11, 1, 1, 5)
pack02_cards.extend(blessing_arc_l1)
blessing_arc_l2 = add_cards(11, 2, 1, 5)
pack02_cards.extend(blessing_arc_l2)
print(f"Added {len(blessing_arc_l1) + len(blessing_arc_l2)} Blessing cards to Arcane deck")

# 6. QUEST DECK (index 6): 13 cards total
# 13 Level cards (4 Level 1, 5 Level 2, 4 Level 3)
level_quest_l1 = add_cards(3, 1, 4, 6)
pack02_cards.extend(level_quest_l1)
level_quest_l2 = add_cards(3, 2, 5, 6)
pack02_cards.extend(level_quest_l2)
level_quest_l3 = add_cards(3, 3, 4, 6)
pack02_cards.extend(level_quest_l3)
print(f"Added {len(level_quest_l1) + len(level_quest_l2) + len(level_quest_l3)} Level cards to Quest deck")

# Output the JSON array
print(f"\nTotal cards generated: {len(pack02_cards)}")
print("\n" + "="*60)
print("JSON OUTPUT (ready to paste into CardDatas field):")
print("="*60)
print(json.dumps(pack02_cards, indent=2))
