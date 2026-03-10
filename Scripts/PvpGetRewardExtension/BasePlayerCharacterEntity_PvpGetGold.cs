using Insthync.DevExtension;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        [DevExtMethods("Awake")]
        public void Awake_PvpGetGold()
        {
            onReceivedDamage += ReceivedDamage_PvpGetGold;
        }

        [DevExtMethods("OnDestroy")]
        public void OnDestroy_PvpGetGold()
        {
            onReceivedDamage -= ReceivedDamage_PvpGetGold;
        }

        private void ReceivedDamage_PvpGetGold(
            HitBoxPosition position,
            Vector3 fromPosition,
            EntityInfo instigator,
            CombatAmountType combatAmountType,
            int damage,
            CharacterItem weapon,
            BaseSkill skill,
            int skillLevel,
            CharacterBuff buff,
            bool isDamageOverTime)
        {
            if (!IsServer)
                return;

            if (instigator == null || instigator.ObjectId == ObjectId || instigator.Type != EntityTypes.Player)
                return;

            if (DuelingComponent != null && DuelingComponent.DuelingStarted && DuelingComponent.DuelingCharacter != null && DuelingComponent.DuelingCharacter.ObjectId == instigator.ObjectId)
                return;

            if (!this.IsDead())
                return;

            int rewardGold = 100;
            if (instigator.TryGetEntity(out BasePlayerCharacterEntity instigatorEntity))
                instigatorEntity.RewardGold(rewardGold, 1f, RewardGivenType.None, 1, 1);
        }
    }
}
