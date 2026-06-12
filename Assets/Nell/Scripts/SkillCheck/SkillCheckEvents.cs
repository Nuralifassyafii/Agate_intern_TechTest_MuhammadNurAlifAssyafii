public struct DiceResult
{
    public int Score;
    public int Health;
    public int Sanity;
    public int EXP;
    public int OpponentLock;
    public int Reroll;

    public DiceResult(int score = 0, int health = 0, int sanity = 0, int exp = 0, int opponentLock = 0, int reroll = 0)
    {
        Score = score;
        Health = health;
        Sanity = sanity;
        EXP = exp;
        OpponentLock = opponentLock;
        Reroll = reroll;
    }
}

public struct SkillCheckUpdateUIEvent
{
    public int PlayerScore;
    public int NPCScore;
    public bool? Results;
    public bool RollButtonEnabled;
    public bool PreviewEnabled;
    public int RerollCount;
    public int HPChange;
    public int SanityChange;
    public int EXPChange;
    public SkillCheckUpdateUIEvent(int playerScore = 0, int npcScore = 0, bool? results = false, bool rollButtonEnabled = false, bool previewEnabled = false, int rerollCount = 0, int hpChange = 0, int sanityChange = 0, int expChange = 0)
    {
        PlayerScore = playerScore;
        NPCScore = npcScore;
        Results = results;
        RollButtonEnabled = rollButtonEnabled;
        PreviewEnabled = previewEnabled;
        RerollCount = rerollCount;
        HPChange = hpChange;
        SanityChange = sanityChange;
        EXPChange = expChange;
    }
}


public struct SkillCheckResetUIEvent
{
    public string NPCActionText;
    public string PlayerSkillText;
    public SkillCheckResetUIEvent(string npcActionText, string playerSkillText)
    {
        NPCActionText = npcActionText;
        PlayerSkillText = playerSkillText;
    }
}