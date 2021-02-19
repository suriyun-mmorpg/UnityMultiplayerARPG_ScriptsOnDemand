using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Exp Potion Item", menuName = "Create GameData/Item/Exp Potion Item", order = -4886)]
    public class ExpPotionItem : BaseItem, IPotionItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_POTION.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Potion; }
        }

        [Header("Potion Configs")]
        [SerializeField]
        private Buff buff;
        public Buff Buff
        {
            get { return buff; }
        }

        [SerializeField]
        private int exp;
        public int Exp
        {
            get { return exp; }
        }

        public void UseItem(BaseCharacterEntity characterEntity, short itemIndex, CharacterItem characterItem)
        {
            if (!characterEntity.CanUseItem() || characterItem.level <= 0 || !characterEntity.DecreaseItemsByIndex(itemIndex, 1))
                return;
            characterEntity.FillEmptySlots();
            characterEntity.ApplyBuff(DataId, BuffType.PotionBuff, characterItem.level, characterEntity.GetInfo());
            characterEntity.RewardExp(new Reward()
            {
                exp = Exp
            }, 1, RewardGivenType.None);
        }

        public bool HasCustomAimControls()
        {
            return false;
        }

        public Vector3? UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return null;
        }

        public void FinishAimControls(bool isCancel)
        {

        }
    }
}
