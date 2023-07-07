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

            if (Dueling.DuelingStarted && Dueling.DuelingCharacter != null && Dueling.DuelingCharacter.ObjectId == attacker.Entity.ObjectId)
                return;

            if (!this.IsDead())
                return;

            int rewardGold = 100;
            (attacker.Entity as BasePlayerCharacterEntity).RewardCurrencies(new Reward()
            {
                gold = rewardGold
            }, 1f, RewardGivenType.None, 1, 1);
        }
    }
}
