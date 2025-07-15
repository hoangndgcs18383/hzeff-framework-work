namespace SAGE.Framework.SDK
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
#if PLAY_ADS
    using PlayAd.SDK.Ads;
    using PlayAd.SDK.Leaderboard;
#endif
    using SAGE.Framework.Core;
    using UnityEngine;

    public static class UserProfileService
    {
        private static UserProfile _userProfile;

        public static UserProfile UserProfile
        {
            get { return _userProfile ??= new UserProfile(); }
            set => _userProfile = value;
        }

        private const string SavePath = "userProfile.profile";
        public static event Action<UserProfile> OnUserProfileChanged;
        public static event Action<UserProfile> OnUserProfileLoaded;
        public static event Action<TypeBooster, int> OnBoosterChanged = delegate { };

        public static event Action<string> OnUserNameChanged;
#if PLAY_ADS
        private static PlayAdUser _playAdUser;
#endif

        public static UniTask LoadUserProfile()
        {
#if PLAY_ADS
            _playAdUser = PlayAdSupport.GetUser();
#endif

            if (File.Exists(GetSavePath()))
            {
                byte[] data = File.ReadAllBytes(GetSavePath());
                UserProfile =
                    JsonConvert.DeserializeObject<UserProfile>(EncryptionUtility.DecryptStringFromBytes_Aes(data));
#if PLAY_ADS
                Debug.Log("User profile locally" + JsonConvert.SerializeObject(UserProfile)
                                                 + ", playAdUser: " + JsonConvert.SerializeObject(_playAdUser));
#else
                Debug.Log("User profile loaded from local file: " + JsonConvert.SerializeObject(UserProfile));
#endif
            }
            else if (!File.Exists(GetSavePath()))
            {
#if PLAY_ADS
                Debug.Log(JsonConvert.DeserializeObject<GameData>(_playAdUser.GameData));
                UserProfile = new UserProfile
                {
                    Gem = _playAdUser.Money,
                    Coins = _playAdUser.Coin,
                    HighScore = _playAdUser.HighRecord,
                    GameData = JsonConvert.DeserializeObject<GameData>(_playAdUser.GameData) ?? new GameData()
                };
#else
                UserProfile = new UserProfile
                {
                    Gem = 0,
                    Coins = 0,
                    HighScore = 0,
                    GameData = new GameData
                    {
                        CurrentLevel = 1,
                        CurrentTimeChange = 0,
                        DisplayName = $"Player_{Guid.NewGuid().ToString().Substring(0, 8)}",
                        BoosterDatas = new List<BoosterData>()
                    }
                };
#endif
                Debug.Log("User profile not found, load from server: " + JsonConvert.SerializeObject(UserProfile));
            }

            if (string.IsNullOrEmpty(UserProfile.GameData.DisplayName))
            {
                UserProfile.GameData.DisplayName = $"Player_{Guid.NewGuid().ToString().Substring(0, 8)}";
                SetDisplayName(UserProfile.GameData.DisplayName);
            }

            OnUserProfileLoaded?.Invoke(UserProfile);
            return UniTask.CompletedTask;
        }

        public static void LocalSync()
        {
            byte[] json = EncryptionUtility.EncryptStringToBytes_Aes(JsonConvert.SerializeObject(UserProfile));
            File.WriteAllBytes(GetSavePath(), json);
            OnUserProfileChanged?.Invoke(UserProfile);
        }

        public static async UniTask Sync()
        {
            LocalSync();
#if PLAY_ADS
            _playAdUser.GameData = JsonConvert.SerializeObject(UserProfile.GameData);
            await _playAdUser.SaveAllAsync().AsUniTask();
            Debug.Log("User profile synced to server: " + JsonConvert.SerializeObject(_playAdUser));
#else
            await UniTask.CompletedTask;
#endif
        }

        public static void DeleteUserProfile()
        {
            if (File.Exists(GetSavePath()))
            {
                File.Delete(GetSavePath());
#if PLAY_ADS
                _playAdUser.ResetData();
#endif
            }
        }

        public static string GetSavePath()
        {
            return $"{Application.persistentDataPath}/{SavePath}";
        }

        public static void AddGems(int amount)
        {
            UserProfile.Gem += amount;
#if PLAY_ADS
            _playAdUser.AddMoneyAsync(amount);
#endif
            Sync();
        }

        public static bool BuyItem(int price, ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Coin => BuyCoins(price),
                ResourceType.Gem => BuyGems(price),
                _ => false
            };
        }

        private static bool BuyCoins(int price)
        {
            if (UserProfile.Coins < price) return false;
            UserProfile.Coins -= price;

#if PLAY_ADS
            _playAdUser.Coin -= price;
#endif
            Sync();
            return true;
        }

        private static bool BuyGems(int price)
        {
            if (price <= 0) return false;
            if (UserProfile.Gem < price) return false;
            UserProfile.Gem -= price;
#if PLAY_ADS
            _playAdUser.UseMoneyAsync(price);
#endif
            Sync();
            return true;
        }

        public static void AddHighScore(int score)
        {
            if (score <= UserProfile.HighScore) return;
            UserProfile.HighScore = score;
#if PLAY_ADS
            _playAdUser.HighRecord = score;
            PlayadLeaderboard.SetScore(score);
            PlayadLeaderboard.ResetRankedUp();
#endif
            Sync();
        }

        public static int GetBestScore()
        {
/*#if USE_SDK
            return _playAdUser.HighRecord;
#endif*/
            return UserProfile.HighScore;
        }

        public static int GetCurrentLevel()
        {
            return UserProfile.GameData.CurrentLevel;
        }

        public static void SetCurrentLevel(int level)
        {
            UserProfile.GameData.CurrentLevel = level;
        }

        public static UserProfile GetUserProfile()
        {
            return _userProfile;
        }

        public static int GetResource(ResourceType resourceType)
        {
            return resourceType switch
            {
                ResourceType.Coin => UserProfile.Coins,
                ResourceType.Gem => UserProfile.Gem,
                _ => 0
            };
        }

        public static void AddCoins(int dataCoins, bool notify = true)
        {
            UserProfile.Coins += dataCoins;
#if PLAY_ADS
            _playAdUser.Coin += dataCoins;
#endif
            if (notify) Sync();
        }

        public static void SetDisplayName(string displayName)
        {
            UserProfile.GameData.DisplayName = displayName;
#if PLAY_ADS
            _playAdUser.GameData = JsonConvert.SerializeObject(UserProfile.GameData);
#endif
            OnUserNameChanged?.Invoke(displayName);
        }

        public static string GetDisplayName()
        {
            return UserProfile.GameData.DisplayName;
        }

        public static void AddBooster(TypeBooster type, int amount)
        {
            var boosterData = UserProfile.GameData.BoosterDatas.Find(b => b.Type == type);
            if (boosterData.Type == TypeBooster.None)
            {
                boosterData.Type = type;
                boosterData.Amount = amount;
                UserProfile.GameData.BoosterDatas.Add(boosterData);
            }
            else
            {
                int index = UserProfile.GameData.BoosterDatas.IndexOf(boosterData);
                boosterData.Amount += amount;
                UserProfile.GameData.BoosterDatas[index] = boosterData;
            }


            OnBoosterChanged.Invoke(type, boosterData.Amount);
            LocalSync();
        }

        public static int GetBoosterAmount(TypeBooster type)
        {
            var boosterData = UserProfile.GameData.BoosterDatas.Find(b => b.Type == type);
            return boosterData.Type != TypeBooster.None ? boosterData.Amount : 0;
        }

        public static void UseBooster(TypeBooster type, int amount)
        {
            var boosterData = UserProfile.GameData.BoosterDatas.Find(b => b.Type == type);
            if (boosterData.Type != TypeBooster.None && boosterData.Amount >= amount)
            {
                int index = UserProfile.GameData.BoosterDatas.IndexOf(boosterData);
                boosterData.Amount -= amount;
                UserProfile.GameData.BoosterDatas[index] = boosterData;
                if (boosterData.Amount <= 0)
                {
                    UserProfile.GameData.BoosterDatas.Remove(boosterData);
                }
            }

            OnBoosterChanged.Invoke(type, boosterData.Amount);
            LocalSync();
        }
    }

    public enum ResourceType
    {
        Coin,
        Gem
    }

    public class UserProfile
    {
        public int Coins;
        public int Gem;
        public int HighScore;
        public GameData GameData = new GameData();
    }

    public class GameData
    {
        public int CurrentLevel;
        public int CurrentTimeChange;
        public string DisplayName;
        public List<BoosterData> BoosterDatas = new List<BoosterData>();
    }

    public struct BoosterData : IEquatable<BoosterData>
    {
        public TypeBooster Type;
        public int Amount;

        public bool Equals(BoosterData other)
        {
            return Type == other.Type && Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            return obj is BoosterData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Amount);
        }
    }
}