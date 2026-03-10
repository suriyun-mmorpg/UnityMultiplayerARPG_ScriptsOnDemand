using Insthync.DevExtension;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        [DevExtMethods("Awake")]
        public void Awake_PvpGetExp()
        {
            onReceivedDamage += ReceivedDamage_PvpGetExp;
        }

        [DevExtMethods("OnDestroy")]
        public void OnDestroy_PvpGetExp()
        {
            onReceivedDamage -= ReceivedDamage_PvpGetExp;
        }

        private void ReceivedDamage_PvpGetExp(
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

            int rewardExp = 100;
            if (instigator.TryGetEntity(out BasePlayerCharacterEntity instigatorEntity))
                instigatorEntity.RewardExp(rewardExp, 1f, RewardGivenType.None, 1, 1);
        }
    }
}
