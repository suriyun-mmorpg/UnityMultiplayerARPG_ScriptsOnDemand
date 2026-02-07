using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class OutOfCombatCharacterEntityPatch : MonoBehaviour
    {
        public enum State
        {
            None,
            Combating,
            Recoverying,
        }

        public float outOfCombatDelay = 20f;
        [Range(0.01f, 1f)]
        public float maxRecoverHpRate = 1f;
        [Range(0.01f, 1f)]
        public float recoveryHpRatePerSeconds = 0.1f;
        private BaseCharacterEntity _characterEntity;
        private float _lastCombatTime;
        private State _state;
        private float _lastRecoveryTime;
        private float _accumulatingHp;

        private void Awake()
        {
            _characterEntity = GetComponent<BaseCharacterEntity>();
            if (_characterEntity == null)
            {
                enabled = false;
                return;
            }
            _characterEntity.onReceivedDamage += _characterEntity_onReceivedDamage;
            _characterEntity.onLaunchDamageEntity += _characterEntity_onLaunchDamageEntity;
        }

        private void OnDestroy()
        {
            if (_characterEntity != null)
            {
                _characterEntity.onReceivedDamage -= _characterEntity_onReceivedDamage;
                _characterEntity.onLaunchDamageEntity -= _characterEntity_onLaunchDamageEntity;
            }
        }

        private void _characterEntity_onReceivedDamage(HitBoxPosition position, Vector3 fromPosition, IGameEntity attacker, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime)
        {
            _lastCombatTime = Time.unscaledTime;
            _state = State.Combating;
        }

        private void _characterEntity_onLaunchDamageEntity(bool isLeftHand, CharacterItem weapon, int simulateSeed, byte triggerIndex, byte spreadIndex, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, BaseSkill skill, int skillLevel, AimPosition aimPosition)
        {
            _lastCombatTime = Time.unscaledTime;
            _state = State.Combating;
        }

        private void Update()
        {
            if (!_characterEntity.IsServer)
            {
                return;
            }

            switch (_state)
            {
                case State.Combating:
                    if (Time.unscaledTime - _lastCombatTime > outOfCombatDelay)
                    {
                        _lastRecoveryTime = 0f;
                        _state = State.Recoverying;
                    }
                    break;
                case State.Recoverying:
                    if (Time.unscaledTime - _lastRecoveryTime > 1f)
                    {
                        _lastRecoveryTime = Time.unscaledTime;
                        _accumulatingHp += _characterEntity.MaxHp * recoveryHpRatePerSeconds;
                        int recoverHp = (int)_accumulatingHp;
                        _characterEntity.CallRpcAppendCombatText(CombatAmountType.HpRecovery, HitEffectsSourceType.None, 0, recoverHp);
                        _characterEntity.CurrentHp += recoverHp;
                        _characterEntity.ValidateRecovery(_characterEntity.GetInfo());
                    }
                    if (_characterEntity.HpRate >= maxRecoverHpRate)
                    {
                        _state = State.None;
                    }
                    break;
            }
        }
    }
}
