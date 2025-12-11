#!/usr/bin/env python3
"""
Script to generate balanced resources and requirements for Pack_02
"""
import json
import random
import copy

# Read PlayerData
with open('Assets/PlayerData/PlayerData.json', 'r') as f:
    data = json.load(f)

pack2 = data['PacksDict']['Pack_02']

# Available resource sprites
resources = ['Beast1', 'Book2', 'Bug3', 'Coin_Silver', 'Coin_Gold', 'Feather1', 
             'Fire1', 'Flower1', 'Gem4', 'Light1', 'Plant5', 'Potion2', 'Seed1']

# Common resources (appear more frequently)
common_resources = ['Coin_Silver', 'Seed1', 'Flower1', 'Plant5', 'Fire1']

# Uncommon resources
uncommon_resources = ['Beast1', 'Bug3', 'Feather1', 'Book2', 'Potion2']

# Rare resources
rare_resources = ['Coin_Gold', 'Gem4', 'Light1']

def get_resources_for_level(level, card_type, num_resources):
    """Get appropriate resources based on card level and type"""
    if level == 1:
        # Mostly common, some uncommon
        pool = common_resources * 3 + uncommon_resources
    elif level == 2:
        # Mix of common, uncommon, and rare
        pool = common_resources + uncommon_resources * 2 + rare_resources
    else:  # level 3
        # Mostly uncommon and rare
        pool = uncommon_resources * 2 + rare_resources * 2
    
    return random.sample(pool, min(num_resources, len(pool)))

def create_requirements(card):
    """Create balanced requirements for a card"""
    card_type = card['CardType']['value__']
    level = card.get('Level', 1)
    
    # Card types: 1=Mage, 2=Fate, 3=Level, 4=Challenge, 5=Structure, 
    #             6=Creature, 7=Item, 8=Magic, 9=Resource, 10=Curse, 11=Blessing
    
    # No requirements for Fate, Challenge, Curse, Blessing
    if card_type in [2, 4, 10, 11]:
        return []
    
    # Mage cards - Resurrection requirements (some have, some don't)
    if card_type == 1:
        if random.random() < 0.5:  # 50% of mages have resurrection requirements
            num_reqs = random.randint(2, 4)
            return get_resources_for_level(2, card_type, num_reqs)
        return []
    
    # Level cards - Harder to craft, increase with level
    if card_type == 3:
        if level == 1:
            num_reqs = random.randint(2, 3)
        elif level == 2:
            num_reqs = random.randint(3, 5)
        else:  # level 3
            num_reqs = random.randint(4, 6)
        reqs = get_resources_for_level(level, card_type, num_reqs)
        # Maybe add @Any
        if random.random() < 0.3 and num_reqs >= 3:
            reqs.append('@Any' + str(random.randint(1, 3)))
        return reqs
    
    # Structure cards - Hard to craft
    if card_type == 5:
        num_reqs = random.randint(4, 6)
        reqs = get_resources_for_level(3, card_type, num_reqs)
        if random.random() < 0.4:
            reqs.append('@Any' + str(random.randint(2, 4)))
        return reqs
    
    # Item, Creature, Magic - Based on level
    if card_type in [6, 7, 8]:
        if level == 1:
            num_reqs = random.randint(1, 2)
        elif level == 2:
            num_reqs = random.randint(2, 3)
        else:  # level 3
            num_reqs = random.randint(3, 4)
        
        reqs = get_resources_for_level(level, card_type, num_reqs)
        
        # Sometimes add @OR for variety
        if random.random() < 0.2 and num_reqs >= 2:
            # Insert @OR between two resources
            insert_pos = random.randint(1, len(reqs))
            reqs.insert(insert_pos, '@OR')
        
        return reqs
    
    # Resource cards - Moderate requirements
    if card_type == 9:
        if level == 1:
            num_reqs = random.randint(1, 2)
        elif level == 2:
            num_reqs = random.randint(2, 3)
        else:
            num_reqs = random.randint(2, 4)
        return get_resources_for_level(level, card_type, num_reqs)
    
    return []

def create_resources(card):
    """Create resources that a card provides"""
    card_type = card['CardType']['value__']
    level = card.get('Level', 1)
    
    # Only certain card types have resources
    # Typically Item, Creature, Magic, Resource cards can have resources
    if card_type not in [6, 7, 8, 9]:
        return []
    
    # Higher chance for Resource cards to provide resources
    if card_type == 9:
        chance = 0.9
    else:
        chance = 0.35  # 35% chance for other cards
    
    if random.random() < chance:
        num_resources = random.randint(1, min(2, level + 1))
        return get_resources_for_level(level, card_type, num_resources)
    
    return []

def create_permanent_resource(card):
    """Create permanent resource for a card"""
    card_type = card['CardType']['value__']
    
    # Mages - 50% have permanent resources (rare ones)
    if card_type == 1:
        if random.random() < 0.5:
            # Permanent resources tend to be rarer
            return random.choice(rare_resources + uncommon_resources)
        return ''
    
    # Resource cards - when activated, provide permanent resource
    if card_type == 9:
        if random.random() < 0.5:  # 50% of resource cards give permanent
            return random.choice(uncommon_resources + rare_resources)
        return ''
    
    # Level cards - some provide permanent resources
    if card_type == 3:
        if random.random() < 0.3:
            return random.choice(uncommon_resources + rare_resources)
        return ''
    
    # Structure cards - some provide permanent resources
    if card_type == 5:
        if random.random() < 0.6:
            return random.choice(rare_resources)
        return ''
    
    return ''

# Process all cards
random.seed(42)  # For reproducibility
for card in pack2['CardDatas']:
    # Clear existing resources/requirements
    card['ResourceNames'] = create_resources(card)
    card['RequirementNames'] = create_requirements(card)
    card['PermanentResourceName'] = create_permanent_resource(card)

# Create the new pack
pack_resources = copy.deepcopy(pack2)
pack_resources['Name'] = 'Base Pack 2 Resources'
pack_resources['ID'] = 'Pack_02_Resources'

# Add to PacksDict
data['PacksDict']['Pack_02_Resources'] = pack_resources

# Write back
with open('Assets/PlayerData/PlayerData.json', 'w') as f:
    json.dump(data, f, indent='\t')

print('✓ Created Base Pack 2 Resources')
print(f'Total cards: {len(pack_resources["CardDatas"])}')

# Print statistics
mages_with_res = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 1 and c.get('PermanentResourceName'))
resources_with_perm = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 9 and c.get('PermanentResourceName'))
cards_with_resources = sum(1 for c in pack_resources['CardDatas'] if c.get('ResourceNames'))
cards_with_requirements = sum(1 for c in pack_resources['CardDatas'] if c.get('RequirementNames'))

print(f'\nStatistics:')
print(f'  Mages with permanent resources: {mages_with_res}/6')
print(f'  Resource cards with permanent: {resources_with_perm}/19')
print(f'  Cards providing resources: {cards_with_resources}')
print(f'  Cards with requirements: {cards_with_requirements}')
print('\nResources and requirements balanced successfully!')
