using System.Collections.Generic;
using UnityEngine;

namespace Configs.GamePlay.Scripts
{
    [CreateAssetMenu(fileName = "LevelsOfMainBuildingToUnlockSlots", menuName = "Config/LevelsOfMainBuildingToUnlockSlots", order = 0)]
    public class LevelsOfMainBuildingToUnlockSlots : ScriptableObject
    {
        [SerializeField] private List<int> levelsToUnlock;

        public List<int> LevelsToUnlock => levelsToUnlock;
    }
}