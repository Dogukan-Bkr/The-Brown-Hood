using UnityEngine;

public static class ConfigLoader
{
    public static ConfigData LoadConfig()
    {
        TextAsset configText = Resources.Load<TextAsset>("config");
        if (configText == null)
        {
            Debug.LogError("Config JSON bulunamad�!");
            return null;
        }

        return JsonUtility.FromJson<ConfigData>(configText.text);
    }
}
