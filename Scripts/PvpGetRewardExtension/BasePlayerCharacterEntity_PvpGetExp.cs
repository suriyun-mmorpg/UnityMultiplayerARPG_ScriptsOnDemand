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
            IGameEntity attacker,
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

            if (attacker == null || attacker.Entity == Entity || !(attacker.Entity is BasePlayerCharacterEntity))
                return;

            if (DuelingComponent != null && DuelingComponent.DuelingStarted && DuelingComponent.DuelingCharacter != null && DuelingComponent.DuelingCharacter.ObjectId == attacker.Entity.ObjectId)
                return;

            if (!this.IsDead())
                return;

            int rewardExp = 100;
            (attacker.Entity as BasePlayerCharacterEntity).RewardExp(rewardExp, 1f, RewardGivenType.None, 1, 1);
        }
    }
}
