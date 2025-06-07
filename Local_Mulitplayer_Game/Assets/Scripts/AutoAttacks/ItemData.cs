using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]


public class ItemData : ScriptableObject
{
    public enum ItemBehavior { AUTOSHOOT_BASIC, REVOLUUTION_SHOOT };

    public string itemName;   //name of the specific item
    public Image itemIcon;    //icon of the item
    public float atkDamage;  //attack damage dealt
    public float atkRate; //attacks per second
    public float atkRadius; //radius from which attacks start to fire
    public ItemBehavior behavior;

    [TextArea(4, 4)]
    public string description; //detailed description of the item

    public GameObject objectInstance;
    
    public GameObject trailRenderer;
    public GameObject projectilePrefab;
    public GameObject particleFX;

}
  