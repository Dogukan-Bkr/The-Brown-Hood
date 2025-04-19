[System.Serializable]
public class AdmobRewardedIDs
{
    public string spear;
    public string arrow;
    public string coin;
    public string health;
}

[System.Serializable]
public class AdmobConfig
{
    public string appId;
    public string testId;
    public AdmobRewardedIDs rewarded;
}

[System.Serializable]
public class ConfigData
{
    public AdmobConfig admob;
}
