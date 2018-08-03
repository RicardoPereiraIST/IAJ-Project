using System.Collections.Generic;

public class DataManager{

    private static DataManager instance;
    private int games = 25;
    private static int wins, iterations = 1;
    private static float time;
    public Dictionary<int, int> PropertiesNames = new Dictionary<int, int>(22);

    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
                InitializeNames();
            }
            return instance;
        }
    }

    private static void InitializeNames()
    {
        DataManager.Instance.PropertiesNames.Add("HP".GetHashCode(), 5);
        DataManager.Instance.PropertiesNames.Add("Skeleton1".GetHashCode(), 9);
        DataManager.Instance.PropertiesNames.Add("Skeleton2".GetHashCode(), 10);
        DataManager.Instance.PropertiesNames.Add("Orc1".GetHashCode(), 11);
        DataManager.Instance.PropertiesNames.Add("Orc2".GetHashCode(), 12);
        DataManager.Instance.PropertiesNames.Add("Dragon".GetHashCode(), 13);
        DataManager.Instance.PropertiesNames.Add("Chest1".GetHashCode(), 15);
        DataManager.Instance.PropertiesNames.Add("Chest2".GetHashCode(), 16);
        DataManager.Instance.PropertiesNames.Add("Chest3".GetHashCode(), 17);
        DataManager.Instance.PropertiesNames.Add("Chest4".GetHashCode(), 18);
        DataManager.Instance.PropertiesNames.Add("Chest5".GetHashCode(), 19);
        DataManager.Instance.PropertiesNames.Add("ManaPotion1".GetHashCode(), 3);
        DataManager.Instance.PropertiesNames.Add("ManaPotion2".GetHashCode(), 4);
        DataManager.Instance.PropertiesNames.Add("HealthPotion1".GetHashCode(), 6);
        DataManager.Instance.PropertiesNames.Add("HealthPotion2".GetHashCode(), 7);
        DataManager.Instance.PropertiesNames.Add("Position".GetHashCode(), 0);
        DataManager.Instance.PropertiesNames.Add("Time".GetHashCode(), 1);
        DataManager.Instance.PropertiesNames.Add("Mana".GetHashCode(), 2);
        DataManager.Instance.PropertiesNames.Add("XP".GetHashCode(), 8);
        DataManager.Instance.PropertiesNames.Add("Money".GetHashCode(), 14);
        DataManager.Instance.PropertiesNames.Add("MAXHP".GetHashCode(), 20);
        DataManager.Instance.PropertiesNames.Add("Level".GetHashCode(), 21);

    }

    public int GetGames()
    {
        return games;
    }

    public void SetGames(int n)
    {
        games = n;
    }

    public int GetIterations()
    {
        return iterations;
    }

    private void AddIteration()
    {
        iterations++;
    }

    public void AddTime(float t)
    {
        time += t;
        AddIteration();
    }

    public float GetTime()
    {
        return time;
    }

    public void AddWin()
    {
        wins++;
    }

    public int GetWins()
    {
        return wins;
    }
}
