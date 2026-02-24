using UnityEngine;

public class StressSytem
{
    public float currentStress = 0f;
    public float maxStress = 100f;
    public int cardCountStress = 3;

    public void Reset()
    {
        currentStress = 0;
    }

    public void ProcessWilds(int remainingWilds)
    {
        float stressToAdd = 12f * (3 - remainingWilds);

        currentStress += stressToAdd;
        currentStress = Mathf.Clamp(currentStress, 0f, maxStress);
    }

    public void ProcessCardResult(int cardCount, float totalValue)
    {
        float stressToAdd = 0f;

        if (cardCount >= cardCountStress)
        {
            stressToAdd = 10f;
        }

        if (totalValue >= 6 && totalValue <= 7)
        {
            stressToAdd += 10f;
        }

        currentStress += stressToAdd;
        currentStress = Mathf.Clamp(currentStress, 0f, maxStress);
    }

    public void ProcessDayResult(DayResult result, int day)
    {
        float stressToAdd = 0f;

        switch (result)
        {
            case DayResult.Bust:
                stressToAdd = 25f;
                break;
            case DayResult.Stand:
                stressToAdd = 10f;
                break;
            case DayResult.ExactWin:
                stressToAdd = 0f;
                break;
        }

        if (day >= 3)
        {
            stressToAdd += (day - 2) * 3;
        }

        currentStress += stressToAdd;

        currentStress = Mathf.Clamp(currentStress, 0f, maxStress);
        Debug.Log("Stress System -> day " + day + " result: " + result);
    }

    public bool IsCollapsed()
    {
        return currentStress >= maxStress;
    }
}
