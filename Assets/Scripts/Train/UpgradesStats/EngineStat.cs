using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Train.UpgradesStats
{
    [CreateAssetMenu(fileName = "EngineStats", menuName = "Train/Upgrades/EngineStats")]
    public class EngineStat : ScriptableObject
    {
        public int cost;
        public int power;
        public int speed;
        public float maxHealth;
    }
}
