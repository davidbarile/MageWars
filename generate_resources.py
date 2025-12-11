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
resources = ['Beast1', 'Book2', 'Bug3', 'Coin_Bronze', 'Coin_Silver', 'Coin_Gold', 'Feather1', 
             'Fire1', 'Flower1', 'Gem4', 'Light1', 'Plant5', 'Potion2', 'Seed1']

# Common resources (appear more frequently)
common_resources = ['Coin_Bronze', 'Seed1', 'Flower1', 'Plant5', 'Fire1']

# Uncommon resources
uncommon_resources = ['Beast1', 'Bug3', 'Coin_Silver', 'Feather1', 'Book2', 'Potion2']

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
    """Create balanced requirements for a card with proper @OR and @Any logic"""
    card_type = card['CardType']['value__']
    level = card.get('Level', 1)
    
    # Card types: 1=Mage, 2=Fate, 3=Level, 4=Challenge, 5=Structure, 
    #             6=Creature, 7=Item, 8=Magic, 9=Resource, 10=Curse, 11=Blessing
    
    # No requirements for Fate, Challenge, Curse, Blessing
    if card_type in [2, 4, 10, 11]:
        return []
    
    # Mage cards - ALL must have Resurrection requirements
    # "Mage Resurrection requirements must range between 2 (if rare) and 5 (for more common)"
    if card_type == 1:
        # ALL mages must have resurrection requirements (not 50%)
        num_reqs = random.randint(2, 5)
        
        # If 2 requirements, use rare resources (harder)
        # If 5 requirements, use common resources (easier but need more)
        if num_reqs <= 3:
            # Harder resurrections use rarer resources
            pool = rare_resources * 2 + uncommon_resources
        else:
            # Easier resurrections need more common resources
            pool = common_resources * 2 + uncommon_resources
        
        # Decide between normal, @OR, or @Any (only one special operator per requirement)
        use_or = random.random() < 0.25 and num_reqs >= 4
        use_any = not use_or and random.random() < 0.15
        
        if use_or:
            # Split into two balanced alternatives
            half = num_reqs // 2
            left_side = random.sample(pool, half)
            # Right side: could be resources or @Any
            if random.random() < 0.5:
                right_side = random.sample(pool, num_reqs - half)
            else:
                # Use @Any for right side, scaled to be similarly difficult
                any_count = half + random.randint(0, 1)
                right_side = ['@Any' + str(any_count)]
            reqs = left_side + ['@OR'] + right_side
        elif use_any:
            # Add @Any at the end, scaled appropriately (1-2 for mages)
            reqs = random.sample(pool, num_reqs - 1)
            reqs.append('@Any' + str(random.randint(1, 2)))
        else:
            reqs = random.sample(pool, num_reqs)
        
        return reqs
    
    # Level cards - Harder to craft, increase with level
    # "Level cards should be in general harder to craft"
    if card_type == 3:
        if level == 1:
            num_reqs = random.randint(2, 4)
        elif level == 2:
            num_reqs = random.randint(3, 5)
        else:  # level 3
            num_reqs = random.randint(4, 6)
        
        # Decide between normal, @OR, or @Any
        use_or = random.random() < 0.2 and num_reqs >= 4
        use_any = not use_or and random.random() < 0.3
        
        if use_or:
            # Split into two balanced alternatives for harder cards
            # Example: "Gem4, Coin_Gold, @OR, @Any4"
            half = num_reqs // 2
            left_side = get_resources_for_level(level, card_type, half)
            # Right side scales with difficulty
            if random.random() < 0.6:
                # Use @Any for flexibility, scaled to level
                any_count = half + level - 1
                right_side = ['@Any' + str(min(5, any_count))]
            else:
                right_side = get_resources_for_level(level, card_type, num_reqs - half)
            reqs = left_side + ['@OR'] + right_side
        elif use_any:
            # @Any scales with level: level 1=1-2, level 2=2-3, level 3=3-4
            any_count = level + random.randint(0, 1)
            reqs = get_resources_for_level(level, card_type, num_reqs - 1)
            reqs.append('@Any' + str(min(5, any_count)))
        else:
            reqs = get_resources_for_level(level, card_type, num_reqs)
        
        return reqs
    
    # Structure cards - Hard to craft
    # "Structure cards should be hard to craft, requiring many and/or rare Resources"
    if card_type == 5:
        num_reqs = random.randint(4, 6)
        
        # Structures are hard - higher chance of flexibility with @OR or @Any
        use_or = random.random() < 0.25 and num_reqs >= 4
        use_any = not use_or and random.random() < 0.4
        
        if use_or:
            # Offer two difficult alternatives
            # Example: "Gem4, Light1, Coin_Gold, @OR, @Any5"
            half = num_reqs // 2
            left_side = get_resources_for_level(3, card_type, half)
            # Right side: either high @Any or rare resources
            if random.random() < 0.7:
                # High @Any count for structures (4-5)
                any_count = random.randint(4, 5)
                right_side = ['@Any' + str(any_count)]
            else:
                right_side = get_resources_for_level(3, card_type, num_reqs - half)
            reqs = left_side + ['@OR'] + right_side
        elif use_any:
            # High @Any count for structures (3-5)
            any_count = random.randint(3, 5)
            reqs = get_resources_for_level(3, card_type, num_reqs - 1)
            reqs.append('@Any' + str(any_count))
        else:
            reqs = get_resources_for_level(3, card_type, num_reqs)
        
        return reqs
    
    # Item, Creature, Magic - Based on level
    # "should be relatively easy to craft, with the only factor being their level"
    # Max 4 requirements
    if card_type in [6, 7, 8]:
        if level == 1:
            num_reqs = random.randint(1, 2)
        elif level == 2:
            num_reqs = random.randint(2, 3)
        else:  # level 3
            num_reqs = random.randint(3, 4)  # Max 4
        
        # Lower chance of special operators for easier cards
        use_or = random.random() < 0.15 and num_reqs >= 3
        use_any = not use_or and random.random() < 0.1 and level >= 2
        
        if use_or:
            # Balanced alternatives for items/creatures/magic
            half = max(1, num_reqs // 2)
            left_side = get_resources_for_level(level, card_type, half)
            right_side = get_resources_for_level(level, card_type, num_reqs - half)
            reqs = left_side + ['@OR'] + right_side
        elif use_any:
            # Low @Any count for easy cards (1-2)
            any_count = random.randint(1, min(2, level))
            reqs = get_resources_for_level(level, card_type, num_reqs - 1)
            reqs.append('@Any' + str(any_count))
        else:
            reqs = get_resources_for_level(level, card_type, num_reqs)
        
        return reqs
    
    # Resource cards - Moderate requirements
    # "Resource cards require resources to activate"
    # Max 4 requirements
    if card_type == 9:
        if level == 1:
            num_reqs = random.randint(1, 2)
        elif level == 2:
            num_reqs = random.randint(2, 3)
        else:
            num_reqs = random.randint(2, 4)  # Max 4
        
        # Moderate chance for flexibility
        use_or = random.random() < 0.15 and num_reqs >= 3
        use_any = not use_or and random.random() < 0.1
        
        if use_or:
            # Balanced alternatives
            half = max(1, num_reqs // 2)
            left_side = get_resources_for_level(level, card_type, half)
            right_side = get_resources_for_level(level, card_type, num_reqs - half)
            reqs = left_side + ['@OR'] + right_side
        elif use_any:
            # Low @Any count (1-2)
            any_count = random.randint(1, 2)
            reqs = get_resources_for_level(level, card_type, num_reqs - 1)
            reqs.append('@Any' + str(any_count))
        else:
            reqs = get_resources_for_level(level, card_type, num_reqs)
        
        return reqs
    
    return []

def create_resources(card):
    """Create resources that a card provides"""
    card_type = card['CardType']['value__']
    level = card.get('Level', 1)
    
    # No resources for Mage, Fate, Challenge, Curse, Blessing cards
    if card_type in [1, 2, 4, 10, 11]:
        return []
    
    # Only Item, Creature, Magic, Resource, Level, Structure cards can have resources
    if card_type not in [3, 5, 6, 7, 8, 9]:
        return []
    
    # "In general, cards with Resources should have between 3 and 5 Resources"
    # "3 if more lower level card or resource more rare, to 5 if more common resource or higher level card"
    
    # Resource cards provide resources (high chance)
    if card_type == 9:
        chance = 1.0  # 100% of resource cards provide resources
        # Resource cards: 3-5 resources based on level
        if level == 1:
            num_resources = 3
        elif level == 2:
            num_resources = 4
        else:
            num_resources = 5
        
        return get_resources_for_level(level, card_type, num_resources)
    
    # Level, Structure, Item, Creature, Magic cards - lower chance
    # These can optionally provide resources
    if card_type in [3, 5]:
        chance = 0.4  # 40% chance for Level/Structure
    else:
        chance = 0.3  # 30% chance for Item/Creature/Magic
    
    if random.random() < chance:
        # 3-5 resources based on level
        if level == 1:
            num_resources = 3
        elif level == 2:
            num_resources = random.randint(3, 4)
        else:
            num_resources = random.randint(4, 5)
        
        return get_resources_for_level(level, card_type, num_resources)
    
    return []

def create_permanent_resource(card):
    """Create permanent resource for a card"""
    card_type = card['CardType']['value__']
    
    # No permanent resources for Fate, Challenge, Curse, Blessing cards
    if card_type in [2, 4, 10, 11]:
        return ''
    
    # "some (15-25%) of the mages should have a Permanent Resource"
    # "Use your mastery as a game designer to decide if these Permanent Resources should be more common or rare"
    # Decision: Mage permanent resources should be uncommon/rare to make them valuable
    if card_type == 1:
        if random.random() < 0.2:  # 20% have permanent resources (within 15-25% range)
            # Permanent resources for mages are uncommon/rare - this makes them valuable
            return random.choice(uncommon_resources + rare_resources)
        return ''
    
    # "Resource cards... once activated, offer various rare Resources, or one Permanent Resource"
    # Resource cards that don't provide regular resources should provide a permanent resource
    if card_type == 9:
        # 50-60% of resource cards give permanent resource (mutually exclusive with providing multiple resources in some cases)
        if random.random() < 0.55:
            # Permanent resources should be uncommon/rare to be valuable long-term
            return random.choice(uncommon_resources + rare_resources)
        return ''
    
    # Level cards - some provide permanent resources (rare bonus)
    if card_type == 3:
        if random.random() < 0.25:
            return random.choice(uncommon_resources + rare_resources)
        return ''
    
    # Structure cards - some provide permanent resources (higher chance, high value)
    if card_type == 5:
        if random.random() < 0.4:
            return random.choice(rare_resources + uncommon_resources)
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
mages_total = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 1)
mages_with_reqs = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 1 and c.get('RequirementNames'))
mages_with_perm = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 1 and c.get('PermanentResourceName'))
resource_cards_total = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 9)
resources_with_perm = sum(1 for c in pack_resources['CardDatas'] if c['CardType']['value__'] == 9 and c.get('PermanentResourceName'))
cards_with_resources = sum(1 for c in pack_resources['CardDatas'] if c.get('ResourceNames'))
cards_with_requirements = sum(1 for c in pack_resources['CardDatas'] if c.get('RequirementNames'))
reqs_with_or = sum(1 for c in pack_resources['CardDatas'] if '@OR' in c.get('RequirementNames', []))
reqs_with_any = sum(1 for c in pack_resources['CardDatas'] if any('@Any' in str(r) for r in c.get('RequirementNames', [])))

print(f'\nStatistics:')
print(f'  Mages with resurrection requirements: {mages_with_reqs}/{mages_total} ({mages_with_reqs/max(mages_total,1)*100:.0f}%)')
print(f'  Mages with permanent resources: {mages_with_perm}/{mages_total} ({mages_with_perm/max(mages_total,1)*100:.0f}%)')
print(f'  Resource cards with permanent: {resources_with_perm}/{resource_cards_total} ({resources_with_perm/max(resource_cards_total,1)*100:.0f}%)')
print(f'  Cards providing resources: {cards_with_resources}')
print(f'  Cards with requirements: {cards_with_requirements}')
print(f'  Requirements using @OR: {reqs_with_or}')
print(f'  Requirements using @Any: {reqs_with_any}')
print('\nResources and requirements balanced successfully!')
