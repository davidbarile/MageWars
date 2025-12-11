#!/usr/bin/env python3
"""
Script to clear all resources, requirements, and permanent resources from Pack_02_Resources
"""
import json

# Read PlayerData
with open('Assets/PlayerData/PlayerData.json', 'r') as f:
    data = json.load(f)

# Get Pack_02_Resources
pack = data['PacksDict']['Pack_02_Resources']

# Clear all resources, requirements, and permanent resources
cleared_count = 0
for card in pack['CardDatas']:
    card['ResourceNames'] = []
    card['RequirementNames'] = []
    card['PermanentResourceName'] = ''
    cleared_count += 1

# Write back
with open('Assets/PlayerData/PlayerData.json', 'w') as f:
    json.dump(data, f, indent='\t')

print(f'✓ Cleared all Resources, Requirements, and Permanent Resources from Pack_02_Resources')
print(f'Total cards cleared: {cleared_count}')
