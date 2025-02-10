using UnityEngine;

namespace Modules.Base.GameScreen.Scripts.GamePlay.Cars
{
    public class ParkingSlot : MonoBehaviour
    {
        [SerializeField] private GameObject carPosition;
        public bool IsUnlocked { get; set; } = false;
        public bool IsFree { get; set; } = false;

        public GameObject CarPosition => carPosition;
    }
}