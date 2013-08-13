namespace Outbreak.Entities.Behaviours
{
    public enum GameEntityBehaviourEnum
    {
        MaxEngineEnumBehaviour = 100,
        OnTakeDamage,
        OnHearWeaponNoise,

            // behaviours for a specific slot being changed
        OnPrimaryWeaponChange,
        OnSecondaryWeaponChange,
        OnHeadSlotChange,

            // behaviour for the inventory 
        OnInventoryChange
    }
}