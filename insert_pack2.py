#!/usr/bin/env python3
"""
Script to insert Pack_02 cards into PlayerData.json
"""
import json

# Read the generated pack2 cards
with open('/Users/davidbarile/Unity Projects/Mage Wars/pack2_cards.json', 'r') as f:
    pack2_data = json.load(f)
    pack2_cards = pack2_data['cards']

# Read the current PlayerData.json
with open('/Users/davidbarile/Unity Projects/Mage Wars/Assets/PlayerData/PlayerData.json', 'r') as f:
    player_data = json.load(f)

# Insert the cards into Pack_02
player_data['PacksDict']['Pack_02']['CardDatas'] = pack2_cards

# Write back to PlayerData.json
with open('/Users/davidbarile/Unity Projects/Mage Wars/Assets/PlayerData/PlayerData.json', 'w') as f:
    json.dump(player_data, f, indent="\t")

print(f"Successfully inserted {len(pack2_cards)} cards into Pack_02")
print("Updated PlayerData.json")
