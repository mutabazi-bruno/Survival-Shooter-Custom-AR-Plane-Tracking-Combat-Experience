using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Local leaderboard. Keeps the latest 5 rounds in a JSON file on the device
// (Application.persistentDataPath), so they're still there after the app is closed.
public class Leaderboard : MonoBehaviour
{
    public const int MaxEntries = 5;
    const string FileName = "leaderboard.json";

    // JsonUtility can't save a bare list, so it gets wrapped in a class
    [Serializable]
    class SaveData
    {
        public List<SessionResult> sessions = new();
    }

    SaveData data = new();

    string SavePath => Path.Combine(Application.persistentDataPath, FileName);

    // newest first
    public IReadOnlyList<SessionResult> Sessions => data.sessions;

    public int BestScore
    {
        get
        {
            int best = 0;
            foreach (SessionResult session in data.sessions)
                best = Mathf.Max(best, session.score);
            return best;
        }
    }

    void Awake() => Load();
    void OnEnable() => GameEvents.RoundEnded += Record;
    void OnDisable() => GameEvents.RoundEnded -= Record;

    public bool IsNewBest(int score) => score > 0 && score > BestScore;

    void Record(SessionResult result)
    {
        data.sessions.Insert(0, result);

        if (data.sessions.Count > MaxEntries)
            data.sessions.RemoveRange(MaxEntries, data.sessions.Count - MaxEntries);

        Save();
    }

    void Load()
    {
        try
        {
            if (File.Exists(SavePath))
                data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath)) ?? new SaveData();
        }
        catch (Exception e)
        {
            // a broken file shouldn't stop the game from starting
            Debug.LogWarning($"Couldn't read the leaderboard, starting a new one. {e.Message}");
            data = new SaveData();
        }
    }

    void Save()
    {
        try
        {
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Couldn't save the leaderboard. {e.Message}");
        }
    }

    [ContextMenu("Clear Leaderboard")]
    void Clear()
    {
        data = new SaveData();
        Save();
    }
}
