namespace SAGE.Framework.SDK
{
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEngine;

    public static class PlayerPrefsSafe
    {
        private const string _privateCode = "0QX6QXZ8gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9";

        private static string[] _encryptionKeys = new[]
        {
            "x6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9",
            "3fX6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9",
            "6fX6QXZ9gWTW9Z6Z9QX6QXZ9gWTW9Z6Z9"
        };

        private const string _typeInt = "int";
        private const string _typeFloat = "float";
        private const string _typeString = "string";
        private const string _typeBool = "bool";


        #region Setters

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            SaveEncryption(key, _typeInt, value.ToString());
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            SaveEncryption(key, _typeFloat, value.ToString());
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            SaveEncryption(key, _typeString, value);
        }

        public static void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            SaveEncryption(key, _typeBool, value ? "1" : "0");
        }

        #endregion

        #region Getters

        public static int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        public static float GetFloat(string key)
        {
            return GetFloat(key, 0f);
        }

        public static string GetString(string key)
        {
            return GetString(key, "");
        }

        public static bool GetBool(string key)
        {
            return GetBool(key, false);
        }

        private static int GetInt(string key, int defaultValue)
        {
            int value = PlayerPrefs.GetInt(key, defaultValue);
            if (!VerifyEncryption(key, _typeInt, value.ToString()))
            {
                return defaultValue;
            }

            return PlayerPrefs.GetInt(key, defaultValue);
        }

        private static float GetFloat(string key, float defaultValue)
        {
            float value = PlayerPrefs.GetFloat(key, defaultValue);
            if (!VerifyEncryption(key, _typeFloat, value.ToString(CultureInfo.InvariantCulture)))
            {
                return defaultValue;
            }

            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        private static string GetString(string key, string defaultValue)
        {
            string value = PlayerPrefs.GetString(key, defaultValue);
            if (!VerifyEncryption(key, _typeString, value))
            {
                return defaultValue;
            }

            return PlayerPrefs.GetString(key, defaultValue);
        }

        private static bool GetBool(string key, bool defaultValue)
        {
            int value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
            if (!VerifyEncryption(key, _typeBool, value.ToString()))
            {
                return defaultValue;
            }

            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        #endregion

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public static void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.DeleteKey($"{key}_encryption_check");
            PlayerPrefs.DeleteKey($"{key}_used_key");
        }

        private static void SaveEncryption(string key, string type, string value)
        {
            int keyIndex = (int)Mathf.Floor(Random.value * _encryptionKeys.Length);
            string keyToSave = _encryptionKeys[keyIndex];
            string encryptedValue = ComputeHash($"{type}_{_privateCode}_{keyToSave}_{value}");
            PlayerPrefs.SetString($"{key}_encryption_check", encryptedValue);
            PlayerPrefs.SetInt($"{key}_used_key", keyIndex);
        }

        private static bool VerifyEncryption(string key, string type, string value)
        {
            int keyIndex = PlayerPrefs.GetInt($"{key}_used_key", -1);
            if (keyIndex == -1)
            {
                //Debug.LogError("No key found");
                return false;
            }

            string keyToSave = _encryptionKeys[keyIndex];
            string encryptedValue = ComputeHash($"{type}_{_privateCode}_{keyToSave}_{value}");
            string savedValue = PlayerPrefs.GetString($"{key}_encryption_check", "");

            Debug.Log($"Saved value: {savedValue} - Encrypted value: {encryptedValue}");

            if (encryptedValue != savedValue)
            {
                Debug.LogError("Encryption check failed");
            }

            return encryptedValue == savedValue;
        }

        private static string ComputeHash(string str)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(str);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            StringBuilder hashStringBuilder = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashStringBuilder.Append((hashBytes[i].ToString("x2")));
            }

            return hashStringBuilder.ToString();
        }
    }
}