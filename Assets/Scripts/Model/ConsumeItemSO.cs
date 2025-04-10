using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ConsumeItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        [SerializeField]
        private List<ModifierData > modifiersData = new List<ModifierData>();
        public string ActionName => "Consume";
        [field: SerializeField]
        public AudioClip actionSFX {get; private set; }
        [SerializeField]
        public GameObject prefab; // Thêm trường prefab

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            foreach(ModifierData data in modifiersData)
            {
                data.statModifer.AffectCharacter(character, data.value);
            }
            return true;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
    public interface IDestroyableItem
    {

    }
    public interface IItemAction
    {
        public string ActionName { get; }
        public AudioClip actionSFX { get; }
        bool PerformAction(GameObject character, List<ItemParameter> itemState);
    }
    [Serializable]
    public class ModifierData
    {
        public CharacterStatModifierSO statModifer;
        public float value;
    }
}