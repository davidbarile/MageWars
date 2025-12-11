# Mage Wars - Project Coding Standards

**Last Updated:** December 10, 2025

This document outlines the coding conventions and standards for the Mage Wars Unity project. These rules are derived from existing codebase patterns and should be followed for consistency.

---

## Table of Contents

1. [Naming Conventions](#naming-conventions)
2. [Architectural Patterns](#architectural-patterns)
3. [Code Organization](#code-organization)
4. [Unity-Specific Conventions](#unity-specific-conventions)
5. [Serialization](#serialization)
6. [Event Handling](#event-handling)
7. [Documentation & Comments](#documentation--comments)
8. [Error Handling & Debugging](#error-handling--debugging)
9. [Properties & Fields](#properties--fields)
10. [Best Practices](#best-practices)
11. [Known Inconsistencies](#known-inconsistencies)

---

## 1. Naming Conventions

### Classes

| Pattern | Convention | Examples |
|---------|------------|----------|
| **Manager Classes** | `XxxManager` suffix | `GameManager`, `CardsManager`, `DeckManager`, `SaveManager`, `LocalizationManager` |
| **UI Components** | `UI` prefix | `UICardEditPanel`, `UIAttackPanel`, `UICardBacksPanel`, `UICardCell` |
| **Data Classes** | `Data` or `Config` suffix | `CardData`, `PlayerData`, `DeckConfig`, `SavedDecksConfig` |
| **Base Classes** | `Base` suffix | `CardConfigBase`, `CardDataBase`, `UICardFrontBase`, `UICardBackBase` |
| **General Classes** | PascalCase | `Card`, `Pack`, `ClickHandler`, `ExtensionMethods` |

### Fields & Variables

```csharp
// Private fields - camelCase
private int cardCount;
private GameObject cardPrefab;
private List<Card> activeCards;

// Public/Protected fields - PascalCase
public int MaxCards;
public CardData CurrentCard;

// Static fields - PascalCase
public static GameManager IN;  // Singleton instance
private static Dictionary<string, Sprite> spriteCache;

// Constants - UPPER_SNAKE_CASE
private const int MAX_HAND_SIZE = 7;
private const string SAVE_FILE_PATH = "saves/deck.json";
```

### Methods

```csharp
// All methods - PascalCase (public and private)
public void RefreshDisplay(CardData inData)
private void InitializeComponents()
protected virtual void OnDataChanged()
```

### Parameters

**Use `in` prefix for all method parameters:**

```csharp
public void AddCard(Card inCard, Transform inParent)
public void SetCardData(CardData inData, bool inRefresh = true)
private void UpdateUI(int inPlayerIndex, string inMessage)
```

### Enums

```csharp
// Enum types - PascalCase with 'E' prefix
public enum ECardType { Action, Challenge, Curse, Fate, Level, Mage }
public enum EAttackType { Melee, Ranged, Magic }
public enum EDeckState { Editing, Playing, Saved }

// Enum values - PascalCase
ECardType.Action
EAttackType.Melee
EDeckState.Editing
```

---

## 2. Architectural Patterns

### Singleton Pattern

**Use static `IN` field for singleton instances:**

```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager IN;
    
    private void Awake()
    {
        IN = this;
        // Other initialization...
    }
}

// Usage:
GameManager.IN.StartNewGame();
CardsManager.IN.ShuffleCards();
```

**Centralized Initialization:**
- The `SingletonManager` initializes all singletons via `InitSingletons()` method
- Called early in application lifecycle

### Manager Pattern

**All core system managers should:**
- Inherit from `MonoBehaviour`
- Implement singleton pattern with `IN` static field
- Handle one specific domain (Cards, Deck, Save, etc.)
- Be accessible globally via static reference

```csharp
public class CardsManager : MonoBehaviour
{
    public static CardsManager IN;
    
    private void Awake()
    {
        IN = this;
    }
    
    public void Initialize() { }
    // Domain-specific methods...
}
```

### Base Class Hierarchy

**Use inheritance hierarchies for related types:**

```csharp
// Configuration classes (ScriptableObjects)
ScriptableObject → CardConfigBase → CardConfig

// Data classes (Serializable)
CardDataBase → CardData

// UI classes
MonoBehaviour → UICardFrontBase → UICardFront_Action, UICardFront_Mage, etc.
MonoBehaviour → UICardBackBase → specific implementations
```

**Benefits:**
- Shared functionality in base classes
- Polymorphic collections
- Virtual methods for customization

---

## 3. Code Organization

### File Structure

**One class per file:**
- File name MUST match class name exactly
- Example: `GameManager.cs` contains `GameManager` class

**Nested classes:**
- Define within parent class file
- Example: `AttackData` class inside `PlayerData.cs`

### Using Statements

```csharp
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;  // When using Odin Inspector
```

### Region Usage

**Use sparingly, primarily for:**
- Enum groupings
- Large sections of related code

```csharp
#region Enums
public enum ECardType { ... }
public enum EAttackType { ... }
#endregion
```

---

## 4. Unity-Specific Conventions

### Component References

**Serialize private fields, expose via public properties:**

```csharp
[SerializeField] private Transform cardContainer;
[SerializeField] private Button saveButton;
[SerializeField] private TextMeshProUGUI titleText;

// Lazy-loaded components
private Canvas canvas;
public Canvas Canvas => canvas ??= GetComponent<Canvas>();
```

### Lifecycle Methods

**Awake()** - Use for:
- Setting up singleton instances
- Initializing core references
- Clearing temporary data

```csharp
private void Awake()
{
    IN = this;
    PlayerPrefs.DeleteKey("TempData");
}
```

**Start()** - Use for:
- Deferred initialization
- Starting coroutines
- Accessing other singletons

```csharp
private void Start()
{
    StartCoroutine(InitializeAsync());
    RefreshDisplay();
}
```

**OnValidate()** - Use for:
- Editor-time validation
- Updating serialized data
- Refreshing inspector display

```csharp
private void OnValidate()
{
    RefreshCardDisplay();
    ValidateCardData();
}
```

### Prefab Instantiation

**Standard pattern:**

```csharp
var prefab = Resources.Load($"Card Prefab/Card");
var cardGo = Instantiate(prefab, inParent) as GameObject;
var card = cardGo.GetComponent<Card>();
card.SetData(cardData);
```

---

## 5. Serialization

### Unity Serialization

**Use `[SerializeField]` for private fields:**

```csharp
[SerializeField] private int maxHandSize = 7;
[SerializeField] private CardData defaultCard;
[SerializeField] private List<Card> activeCards = new();
```

### Odin Inspector Attributes

**Common patterns:**

```csharp
// Button in inspector
[Button("Refresh Display")]
private void RefreshDisplay() { }

// Conditional visibility
[ShowIf("isEditing")]
[SerializeField] private GameObject editPanel;

// Value dropdown
[ValueDropdown("GetCardTypes")]
[SerializeField] private string cardType;

// Required field validation
[Required]
[SerializeField] private CardData cardData;

// Asset list dropdown
[AssetList]
[SerializeField] private CardConfig cardConfig;

// Range slider
[Range(0, 10)]
[SerializeField] private int attackValue;
```

### ScriptableObjects

**Use for configuration data:**

```csharp
[CreateAssetMenu(fileName = "NewCardConfig", menuName = "Mage Wars/Card Config")]
public class CardConfig : CardConfigBase
{
    // Configuration fields...
}

// Loading:
var config = Resources.Load<CardConfig>("Configs/CardConfig");
```

### Serializable Classes

**Use for runtime data:**

```csharp
[Serializable]
public class CardData : CardDataBase
{
    public string cardName;
    public int attackValue;
    public List<string> abilities = new();
}
```

### JSON Serialization

**Use TotalJSON for custom serialization:**

```csharp
// Serializing
var json = JSON.Serialize(playerData);
PlayerPrefs.SetString("PlayerData", json.CreateString());

// Deserializing
var jsonObj = JSON.ParseString(jsonString);
var playerData = jsonObj.Deserialize<PlayerData>();
```

---

## 6. Event Handling

### Static Action Events

**Use for loose coupling between systems:**

```csharp
public class CardsManager : MonoBehaviour
{
    // Define events
    public static Action OnCardDataChanged;
    public static Action<int> OnActivePacksChanged;
    public static Action<CardData, bool> OnCardSelected;
    
    // Invoke events
    private void UpdateCardData()
    {
        // ... update logic
        OnCardDataChanged?.Invoke();
    }
}

// Subscribe/Unsubscribe
private void OnEnable()
{
    CardsManager.OnCardDataChanged += HandleCardDataChanged;
}

private void OnDisable()
{
    CardsManager.OnCardDataChanged -= HandleCardDataChanged;
}

private void HandleCardDataChanged()
{
    RefreshDisplay();
}
```

---

## 7. Documentation & Comments

### Comment Style

**Single-line comments:**

```csharp
// Disable old system for now
// gameObject.SetActive(false);

// Hack to fix rendering bug with transparent cards
Canvas.ForceUpdateCanvases();
```

**Inline clarification:**

```csharp
cardCount = shuffledCards.Count;  // Store before shuffle
```

**TODO comments:**

```csharp
// TODO: Optimize card pooling for large decks
// TODO: Add multiplayer sync for this method
```

### When to Comment

- Explain *why*, not *what* (code should be self-explanatory)
- Complex algorithms or business logic
- Workarounds for known issues
- Future improvements (TODO)

---

## 8. Error Handling & Debugging

### Debug Logging

**Use color-coded Debug.Log for visibility:**

```csharp
// Error messages - RED
Debug.Log($"<color=red>ShuffledCardDatas Count = ZERO</color>");
Debug.LogError($"<color=red>CardPrefabLookupDict already contains entry for {key}</color>");

// Success messages - GREEN
Debug.Log($"<color=green>Cards loaded successfully: {cardCount}</color>");

// Warning messages - YELLOW
Debug.Log($"<color=yellow>Using fallback card data</color>");

// Info messages - DEFAULT or CYAN
Debug.Log($"<color=cyan>Initializing deck with {cardCount} cards</color>");
```

### Defensive Programming

**Null checks before operations:**

```csharp
if (cardData == null)
{
    Debug.LogError("<color=red>CardData is null</color>");
    return;
}

// Null-conditional operator
OnCardDataChanged?.Invoke();

// Null-coalescing
var cardName = cardData?.cardName ?? "Unknown";
```

### Validation

**Range validation with attributes:**

```csharp
[Range(0, 100)]
[SerializeField] private int healthPoints = 50;

[Range(1, 10)]
[SerializeField] private int attackPower = 5;
```

**Editor-time validation:**

```csharp
private void OnValidate()
{
    if (maxCards < minCards)
    {
        Debug.LogWarning("MaxCards cannot be less than MinCards");
        maxCards = minCards;
    }
}
```

---

## 9. Properties & Fields

### Property Patterns

**Read-only public properties:**

```csharp
public int CardCount { get; private set; }
public CardData CurrentCard { get; private set; }
```

**Computed properties:**

```csharp
// Lambda expression
public UIPlayerBoard CurrentPlayerBoard => PlayerBoards[currentPlayerIndex];

// Property body
public bool IsPlayersTurn => currentPlayerIndex == 0;
public int RemainingCards => deckSize - cardsDrawn;
```

**Lazy initialization:**

```csharp
private Canvas canvas;
public Canvas Canvas => canvas ??= GetComponent<Canvas>();

private Dictionary<string, Sprite> spriteCache;
public Dictionary<string, Sprite> SpriteCache => spriteCache ??= new();
```

### When to Use Fields vs Properties

**Public fields** - Acceptable for:
- Unity-serialized data
- Simple data containers
- Performance-critical code

**Properties** - Preferred for:
- Controlled access (private set)
- Computed values
- Lazy initialization
- Future-proofing (can add logic later)

---

## 10. Best Practices

### Collection Initialization

**Use modern C# syntax:**

```csharp
// Target-typed new expressions (C# 9+)
private List<Card> activeCards = new();
private Dictionary<string, CardData> cardLookup = new();

// Collection initializer when needed
private List<string> defaultAbilities = new() { "Strike", "Block", "Parry" };
```

### String Formatting

**Prefer string interpolation:**

```csharp
// Good
Debug.Log($"Loading {cardCount} cards for player {playerName}");
var path = $"Saves/{fileName}.json";

// Avoid
Debug.Log("Loading " + cardCount + " cards for player " + playerName);
```

### Factory Methods

**Use static Create() methods:**

```csharp
public static CardData Create(string inName, ECardType inType)
{
    var data = new CardData
    {
        cardName = inName,
        cardType = inType
    };
    return data;
}
```

**Deep copy with Clone() methods:**

```csharp
public CardData Clone()
{
    var clone = new CardData
    {
        cardName = this.cardName,
        cardType = this.cardType,
        // ... copy all fields
    };
    return clone;
}
```

### Dirty Tracking

**Manual change tracking:**

```csharp
private bool isDirty = false;

public void SetCardData(CardData inData)
{
    cardData = inData;
    isDirty = true;
    OnDataChanged?.Invoke();
}

public void Save()
{
    if (isDirty)
    {
        // Perform save
        isDirty = false;
    }
}
```

### Virtual Methods in Base Classes

**Allow customization via overrides:**

```csharp
// Base class
public virtual void RefreshDisplay(CardData inData)
{
    // Default implementation
}

// Derived class
public override void RefreshDisplay(CardData inData)
{
    base.RefreshDisplay(inData);
    // Additional custom logic
}
```

---

## 11. Known Inconsistencies

The following inconsistencies exist in the current codebase. When writing new code, prefer the **recommended** approach:

### Debug vs Print

**Current:** Mix of `Debug.Log()` and `print()`  
**Recommended:** Use `Debug.Log()` consistently

### Access Modifiers on Unity Messages

**Current:** Mix of `private` and no modifier for Awake/Start  
**Recommended:** Explicitly use `private` for clarity

```csharp
// Preferred
private void Awake() { }
private void Start() { }
private void OnEnable() { }
```

### Public Fields vs Properties

**Current:** Some data exposed as public fields that could be properties  
**Recommended:** Use properties with private setters for better encapsulation

```csharp
// Instead of:
public int cardCount;

// Prefer:
public int CardCount { get; private set; }
```

### Method Visibility

**Current:** Heavy use of `public` for methods that might not need external access  
**Recommended:** Use minimum necessary visibility (private/internal when possible)

### Enum Prefixing

**Current:** Most enums use `E` prefix, some nested types don't  
**Recommended:** Consistently use `E` prefix for all enum types

---

## Quick Reference

### Naming Cheat Sheet

```csharp
// Classes
public class GameManager { }              // Managers
public class UICardEditPanel { }          // UI components
public class CardData { }                 // Data classes
public class CardConfigBase { }           // Base classes

// Fields & Properties
private int cardCount;                    // Private fields (camelCase)
public int MaxCards;                      // Public fields (PascalCase)
public static GameManager IN;             // Singleton (PascalCase)
private const int MAX_SIZE = 100;         // Constants (UPPER_SNAKE_CASE)

// Methods & Parameters
public void AddCard(Card inCard) { }      // Methods (PascalCase), params (in prefix)

// Enums
public enum ECardType { Action, Mage }    // Type (E prefix), Values (PascalCase)
```

### Common Patterns

```csharp
// Singleton
public static GameManager IN;
private void Awake() { IN = this; }

// Events
public static Action OnDataChanged;
OnDataChanged?.Invoke();

// Lazy Property
private Canvas canvas;
public Canvas Canvas => canvas ??= GetComponent<Canvas>();

// Debug Logging
Debug.Log($"<color=red>Error: {message}</color>");
Debug.Log($"<color=green>Success: {message}</color>");
```

---

## Enforcement

- Code reviews should check adherence to these standards
- Use consistent formatting (consider EditorConfig or IDE settings)
- When modifying existing code, bring it in line with current standards
- Exceptions should be documented with comments explaining why

---

**Document Version:** 1.0  
**Next Review:** March 2026
