namespace CodeBase.Systems.DataPersistenceSystem.Data.Game.ServiceBuildingSlot
{
    public enum ServiceBuildingSlotState
    {
        Locked,    // All locked
        Unlocked,  // Player can buy a building
        Active     // Building is bought and active
    }
}