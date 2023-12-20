using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class QuestMonsterSpawnArea : MonsterSpawnArea
    {
        public Quest quest;
        [Tooltip("Delay before destroy monsters when there is no character with unfinished quest nearby")]
        public float destroyMonsterDelay = 5f;

        private Collider cacheCollider;
        private Collider2D cacheCollider2D;
        private readonly List<BaseMonsterCharacterEntity> monsterEntities = new List<BaseMonsterCharacterEntity>();
        private readonly HashSet<long> characterObjectIds = new HashSet<long>();
        private BasePlayerCharacterEntity tempPlayerCharacterEntity;
        private float lastRemoveCharacterTime;

        protected override void Awake()
        {
            base.Awake();
            cacheCollider = GetComponent<Collider>();
            cacheCollider2D = GetComponent<Collider2D>();

            if (cacheCollider != null)
            {
                cacheCollider.isTrigger = true;
            }

            if (cacheCollider2D != null)
            {
                cacheCollider2D.isTrigger = true;
            }

            BaseGameNetworkManager.Singleton.onUnregisterCharacter += OnUnregisterCharacter;
        }

        private void OnDestroy()
        {
            BaseGameNetworkManager.Singleton.onUnregisterCharacter -= OnUnregisterCharacter;
        }

        private void OnUnregisterCharacter(long connectionId)
        {
            characterObjectIds.Remove(connectionId);
        }

        private void OnTriggerEnter(Collider other)
        {
            AddCharacter(other.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            AddCharacter(collision.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            RemoveCharacter(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            RemoveCharacter(collision.gameObject);
        }

        void AddCharacter(GameObject gameObject)
        {
            if (!BaseGameNetworkManager.Singleton.IsServer)
                return;

            tempPlayerCharacterEntity = gameObject.GetComponent<BasePlayerCharacterEntity>();
            if (tempPlayerCharacterEntity != null && 
                !characterObjectIds.Contains(tempPlayerCharacterEntity.ConnectionId))
            {
                int indexOfQuest = tempPlayerCharacterEntity.IndexOfQuest(quest.DataId);
                if (indexOfQuest >= 0 &&
                    !tempPlayerCharacterEntity.Quests[indexOfQuest].isComplete &&
                    !tempPlayerCharacterEntity.Quests[indexOfQuest].IsAllTasksDone(tempPlayerCharacterEntity, out _))
                {
                    // Add characters which has quest, and quest is not complete yet
                    characterObjectIds.Add(tempPlayerCharacterEntity.ConnectionId);
                    if (monsterEntities.Count == 0)
                    {
                        // Will spawn all monsters when there is no monsters, and character is only 1 character in list
                        SpawnAll();
                    }
                }
            }
        }

        void RemoveCharacter(GameObject gameObject)
        {
            if (!BaseGameNetworkManager.Singleton.IsServer)
                return;

            tempPlayerCharacterEntity = gameObject.GetComponent<BasePlayerCharacterEntity>();
            if (tempPlayerCharacterEntity != null && 
                characterObjectIds.Remove(tempPlayerCharacterEntity.ConnectionId))
            {
                // Remove characters when they move out from this collider
                lastRemoveCharacterTime = Time.unscaledTime;
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (!BaseGameNetworkManager.Singleton.IsServer)
                return;

            if (characterObjectIds.Count == 0 &&
                monsterEntities.Count > 0 &&
                Time.unscaledTime - lastRemoveCharacterTime >= destroyMonsterDelay)
            {
                for (int i = monsterEntities.Count - 1; i >= 0; --i)
                {
                    monsterEntities[i].NetworkDestroy();
                }
                monsterEntities.Clear();
            }
        }

        protected override BaseMonsterCharacterEntity SpawnInternal(BaseMonsterCharacterEntity prefab, int level)
        {
            if (characterObjectIds.Count <= 0)
            {
                // Spawn monster when there are player characters inside this collider
                return null;
            }
            BaseMonsterCharacterEntity spawnedMonster = base.SpawnInternal(prefab, level);
            if (spawnedMonster != null)
            {
                monsterEntities.Add(spawnedMonster);
                return spawnedMonster;
            }
            return null;
        }
    }
}

