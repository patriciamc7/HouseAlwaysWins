//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EventManager : MonoBehaviour
//{
//    [SerializeField] GameManager gameManager;

//    public DayEventType RollAndApplyEvent()
//    {
//        DayEventType ev = RollEvent();

//        ApplyEvent(ev);

//        return ev;
//    }

//    DayEventType RollEvent ()
//    {
//        float stress = gameManager.stressSystem.currentStress;
//        float maxStress = gameManager.stressSystem.maxStress;

//        float stress01 = stress / maxStress;

//        float badChance = Mathf.Lerp(0.15f, 0.60f, stress01);
//        float goodChance = Mathf.Lerp(0.35f, 0.10f, stress01);

//        float r = Random.value;

//        if (r < badChance)
//            return DayEventType.Bad;

//        if (r < badChance + goodChance)
//            return DayEventType.Good;

//        return DayEventType.Neutral;
//    }
//}
