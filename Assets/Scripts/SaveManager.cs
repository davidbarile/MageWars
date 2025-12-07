using System.Collections.Generic;
using System.IO;
using Leguar.TotalJSON;
using UnityEngine;
using Sirenix.OdinInspector;

public class SaveManager : MonoBehaviour
{
    public static SaveManager IN;

    [Header("Persistent Settings")]
    [SerializeField] private bool clearPlayerPrefsOnPlay;
    [SerializeField] private bool createCardConfigsOnQuit;
    [SerializeField] private bool allowConfigsOverwrite;
    [SerializeField] private bool forceGenerateAllCardConfigs;
    [SerializeField] private bool shouldLoadPacksFromConfig;

    [Header("PLAYER DATA")]
    [SerializeField] private PlayerData playerData;

    public static bool IsApplicationQuiting;
    public static bool IsClearingPlayerPrefs;

    private string filePath = "Assets/PlayerData/PlayerData.json";
    private bool isApplicationPaused;

    [Header("Card Configs")]
    [SerializeField] private CardConfig defaultCardConfig;

    [Header("Saved Decks Config")]
    public SavedDecksConfig SavedDecksConfig;

    private CardData defaultCardData;
    private bool playerPrefsExist;
    private int newCardCounter = -1;
    private static List<List<CardData>> DeckCardDatas = new();

    private void Awake()
    {
        if (this.clearPlayerPrefsOnPlay)
        {
            Debug.Log($"<color=red>clearPlayerPrefs = TRUE  ---->  PlayerPrefs.DeleteAll()</color>");
            PlayerPrefs.DeleteAll();
        }

        PlayerData.Data = new();
    }

    public void LoadGame()
    {
        PlayerData.Data = this.playerData;

        if (!Application.isEditor)
            this.filePath = $"{Application.persistentDataPath}/PlayerData.json";

        this.playerPrefsExist = PlayerPrefsX.GetBool("playerPrefsExist");

        var color = this.playerPrefsExist ? "#00FF00" : "red";
        Debug.Log($"LoadGame()   <color={color}>playerPrefsExist = {playerPrefsExist}</color>");

        if (this.playerPrefsExist)
        {
            LoadPlayerData();
            PlayerData.Data.SetAllCardsClean(true);
        }
        else
            PlayerData.Data.ShouldShowAllResources = true;//hack to fix bug

        CardsManager.IN.LoadDataFromCardConfigs();

        //reimport card configs and replace the default pack cards
        DeckManager.IN.DefaultPack.CardDatas = new List<CardData>(CardsManager.IN.AllCardDatas);

        if (this.playerPrefsExist)
        {
            
        }
        else
        {
            //reimport card configs and replace the default pack cards
            //DeckManager.IN.DefaultPack.CardDatas = new List<CardData>(CardsManager.IN.AllCardDatas);
        }

        var isFirstLoadOnDevice = !Application.isEditor && !this.playerPrefsExist;

        if ((this.shouldLoadPacksFromConfig || isFirstLoadOnDevice) && this.SavedDecksConfig.SavedDecks.Count > 0)
        {
            PlayerData.Data.PacksDict.Clear();

            AddDefaultPack(false);

            for (int i = 1; i < this.SavedDecksConfig.SavedDecks.Count; i++)
            {
                var deckDictData = this.SavedDecksConfig.SavedDecks[i];
                PlayerData.Data.PacksDict.Add(deckDictData.PackId, deckDictData.Pack);
            }

            var activePackId = this.SavedDecksConfig.ActivePackId;

            if(PlayerData.Data.PacksDict.ContainsKey(activePackId))
                PlayerData.Data.ActivePackId = activePackId;
            else
                PlayerData.Data.ActivePackId = "Pack_01";
        }
        else
            AddDefaultPack(true);

        static void AddDefaultPack(bool inShouldAddBasePack)
        {
            var defaultPack = Pack.Clone(DeckManager.IN.DefaultPack);
            defaultPack.IsDefault = true;
            defaultPack.IsActiveInGame = false;
            defaultPack.ID = DeckManager.IN.DefaultPack.ID;

            if (PlayerData.Data.PacksDict.Count == 0)
                PlayerData.Data.PacksDict.Add(defaultPack.ID, defaultPack);
            else
                PlayerData.Data.PacksDict[defaultPack.ID] = defaultPack;

            if (inShouldAddBasePack && PlayerData.Data.PacksDict.Count == 1)
            {
                var activePack = Pack.Clone(DeckManager.IN.DefaultPack);
                activePack.IsDefault = false;
                activePack.IsActiveInGame = true;
                activePack.Name = "Base Pack";
                activePack.ID = "Pack_01";
                PlayerData.Data.PacksDict.Add(activePack.ID, activePack);
                PlayerData.Data.ActivePackId = activePack.ID;
            }

            RegeneratePackAttackNums(defaultPack);

            PlayerData.Data.ShouldSaveOnQuit = true;//how to only set this if new configs are added?
        }

        static void RegeneratePackAttackNums(Pack inPack)
        {
            //create list of lists of card datas for each deck type
            DeckCardDatas.Clear();

            for (var i = 0; i < inPack.AttackNumsPresets.Length; i++)
            {
                var deckType = UICardBacksPanel.GetDeckEnum(i);
                var cardDatas = UICardBacksPanel.GetCardsOfDeck(inPack.CardDatas, deckType);
                DeckCardDatas.Add(cardDatas);
            }

            var numsToAdd = new List<List<int>>();

            for (int i = 0; i < inPack.AttackNumsPresets.Length; i++)
            {
                numsToAdd.Add(new List<int>());

                for (int l = 0; l < 9; l++)
                {
                    var count = inPack.AttackNumsPresets[i].Counts[l];

                    for (int m = 0; m < count; m++)
                    {
                        var attackNum = l + 1;
                        numsToAdd[i].Add(attackNum);
                    }
                }
            }

            for (int i = 0; i < inPack.AttackNumsPresets.Length; i++)
            {
                var deckType = UICardBacksPanel.GetDeckEnum(i);
                var deckIndex = (int)deckType;

                numsToAdd[i].RandomizeList();

                foreach (var card in inPack.CardDatas)
                {
                    if (card.AreAttackNumbersLocked) continue;

                    var numCardsInDeck = card.DeckCounts[deckIndex];

                    if (numCardsInDeck < 1) continue;

                    card.AttackNumbersObjects[i].AttackNumValues.Clear();

                    for (int l = 0; l < numCardsInDeck; l++)
                    {
                        if(numsToAdd[i].Count < 1)
                        {
                            print($"<color=red> {card.Name}   {card.CardName}   numsToAdd[{i}].Count = {numsToAdd[i].Count}</color>    numCardsInDeck = {numCardsInDeck}    deckIndex = {deckIndex}    deckType = {deckType}");
                            break;
                        }
                        
                        card.AttackNumbersObjects[i].AttackNumValues.Add(numsToAdd[i][0]);
                        numsToAdd[i].RemoveAt(0);
                    }
                }
            }
        }
    }

    private void LoadPlayerData()
    {
        var reader = new StreamReader(this.filePath);
        var data = reader.ReadToEnd();
        reader.Close();

        var deserializeSettings = new DeserializeSettings()
        {
            RequireAllFieldsArePopulated = false
        };

        JSON jsonObject = JSON.ParseString(data);
        this.playerData = jsonObject.Deserialize<PlayerData>(deserializeSettings);

        PlayerData.Data = this.playerData;

        //Debug.Log($"Loaded: {data}");
    }

    private void SavePlayerData()
    {
        var playerDataJSON = JSON.Serialize(this.playerData);

        string jsonAsString = string.Empty;

        if (Application.isEditor)
            jsonAsString = playerDataJSON.CreatePrettyString();
        else
            jsonAsString = playerDataJSON.CreateString();

        var writer = new StreamWriter(this.filePath);
        writer.WriteLine(jsonAsString);
        writer.Close();
    }

    private void SaveGame()
    {
        this.playerPrefsExist = PlayerPrefsX.SetBool("playerPrefsExist", true);

        if (this.playerData.ShouldSaveOnQuit)
        {
            UICardBacksPanel.IN.SaveData();

            SavePlayerData();

#if UNITY_EDITOR
            if (this.createCardConfigsOnQuit)
            {
                ConvertPlayerDataToCardConfigs();
            }
#endif
        }

        PlayerData.Data.SetAllCardsClean(true);
    }

    private void OnApplicationPause(bool inIsPaused)
    {
        if (SaveManager.IsClearingPlayerPrefs) return;

        this.isApplicationPaused = inIsPaused;

#if !UNITY_EDITOR
        if (inIsPaused)
            SaveGame();
#endif
    }

    private void OnApplicationQuit()
    {
        SaveManager.IsApplicationQuiting = true;

        if (SaveManager.IsClearingPlayerPrefs || this.isApplicationPaused) return;

        SaveGame();
    }

#if UNITY_EDITOR
    [PropertySpace(10)]
    [Button(ButtonSizes.Large), GUIColor(1, 1, 0)]
    private void ConvertPlayerDataToCardConfigs()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("<color=red>This method can only be called in Play Mode!</color>");
            return;
        }

        this.defaultCardData = CardConfig.CreateCardData(this.defaultCardConfig, 0);

        var isGrandParentValid = UnityEditor.AssetDatabase.IsValidFolder("Assets/Configs");
        if (!isGrandParentValid)
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Configs");

        var isParentValid = UnityEditor.AssetDatabase.IsValidFolder("Assets/Configs/Cards");
        if (!isParentValid)
            UnityEditor.AssetDatabase.CreateFolder("Assets/Configs", "Cards");

        for (int i = 0; i < CardsManager.IN.AllCardDatas.Count; i++)
        {
            var cardData = CardsManager.IN.AllCardDatas[i];

            var color = cardData.ShouldSaveOnQuit ? "#00FF00" : "black";
            print($"<color={color}>CardData: {cardData.Name}  id = {cardData.ID}  res = {cardData.PermanentResourceName}</color>");

            if (cardData.ShouldSaveOnQuit || this.forceGenerateAllCardConfigs)
            {
                CreateCardConfig(cardData);
            }
        }
    }

    private void CreateCardConfig(CardData inCardData)
    {
        if (inCardData == null)
            inCardData = this.defaultCardData;

        var foundCardConfig = CardsManager.IN.AllCardConfigs.Find(config => config.ConfigID == inCardData.ID);

        if (foundCardConfig != null && !this.forceGenerateAllCardConfigs && this.allowConfigsOverwrite)
        {
            print($"<color=yellow>Updating existing CardConfig: {foundCardConfig.name}  id = {foundCardConfig.ConfigID}</color>");

            foundCardConfig.CopyFromCardData(inCardData);
            UnityEditor.EditorUtility.SetDirty(foundCardConfig);
        }
        else
        {
            var path = $"Assets/Configs/Cards/{inCardData.CardType}";
            var isFolderValid = UnityEditor.AssetDatabase.IsValidFolder(path);

            if (!isFolderValid)
                UnityEditor.AssetDatabase.CreateFolder("Assets/Configs/Cards", $"{inCardData.CardType}");

            var cardConfig = ScriptableObject.CreateInstance<CardConfig>();
            cardConfig.CopyFromCardData(inCardData);

            var cardNumString = (newCardCounter < 10) ? $"0{newCardCounter}" : newCardCounter.ToString();
            var fileName = $"{inCardData.CardType}_{inCardData.CardName}_{cardNumString}";

            if (!string.IsNullOrWhiteSpace(inCardData.Name))
                fileName = $"{inCardData.Name}_{cardNumString}";

            if (this.forceGenerateAllCardConfigs && foundCardConfig != null)
                fileName = foundCardConfig.name;

            var pathAndFileName = $"{path}/{fileName}.asset";

            UnityEditor.AssetDatabase.CreateAsset(cardConfig, pathAndFileName);//$"Assets/YourSavePath/YourObject.asset"
            UnityEditor.EditorUtility.SetDirty(cardConfig);

            print($"<color=#00FF00>Creating new CardConfig: {pathAndFileName}</color>");

            ++this.newCardCounter;
        }
    }

    public void SaveDecksToConfig()
    {
        //var savedDecksConfig = ScriptableObject.CreateInstance<SavedDecksConfig>();
        // savedDecksConfig.SavedDecks = new List<DeckDictData>();

        this.SavedDecksConfig.ActivePackId = PlayerData.Data.ActivePackId;

        this.SavedDecksConfig.SavedDecks.Clear();

        foreach (var kvp in PlayerData.Data.PacksDict)
        {
            var deckDictData = new DeckDictData
            {
                PackId = kvp.Key,
                Pack = kvp.Value
            };

            this.SavedDecksConfig.SavedDecks.Add(deckDictData);
        }

        //UnityEditor.AssetDatabase.CreateAsset(savedDecksConfig, "Assets/Configs/Saved Decks/SavedDecks.asset");
        UnityEditor.EditorUtility.SetDirty(this.SavedDecksConfig);
    }

    [Title("Caution!  (Confirm panel while running game)")]

    [PropertySpace(30), Button(ButtonSizes.Medium), GUIColor(1f, .1f, .1f)]
    private void ClearAllConfigResources()
    {
        if (!Application.isPlaying) return;

        UIConfirmPanel.IN.Show("Clear Config Resources?", "This will reset all Resources in all Card Configs.\nAre you sure you want to?", () =>
        {
            foreach (var cardConfig in CardsManager.IN.AllCardConfigs)
            {
                cardConfig.Resources.Clear();
                UnityEditor.EditorUtility.SetDirty(cardConfig);
            }
        });
    }

    [PropertySpace, Button(ButtonSizes.Medium), GUIColor(1f, .1f, .1f)]
    private void ClearAllConfigRequirements()
    {
        if (!Application.isPlaying) return;

        UIConfirmPanel.IN.Show("Clear Config Requirements?", "This will reset all Requirements in all Card Configs.\nAre you sure you want to?", () =>
        {
            foreach (var cardConfig in CardsManager.IN.AllCardConfigs)
            {
                cardConfig.Requirements.Clear();
                UnityEditor.EditorUtility.SetDirty(cardConfig);
            }
        });
    }

    [PropertySpace, Button(ButtonSizes.Medium), GUIColor(1f, .1f, .1f)]
    private void ClearAllConfigPermResources()
    {
        if (!Application.isPlaying) return;

        UIConfirmPanel.IN.Show("Clear Config Requirements?", "This will reset all Requirements in all Card Configs.\nAre you sure you want to?", () =>
        {
            foreach (var cardConfig in CardsManager.IN.AllCardConfigs)
            {
                cardConfig.PermanentResource = null;
                UnityEditor.EditorUtility.SetDirty(cardConfig);
            }
        });
    }
#endif
}