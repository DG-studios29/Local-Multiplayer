using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Ability
{
    public string abilityName;
    public GameObject projectilePrefab;
    public float cooldown;
    public int damage;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "NewHeroAbilities", menuName = "Heroes/HeroAbilities")]
public class HeroAbility : ScriptableObject
{
    public string heroName;
    public Ability ability1; // Basic attack (sphere projectile)
    public Ability ability2; // Unique ability
    public Ability ultimate; // Ultimate ability

    public ItemData itemRevolve;
 
}