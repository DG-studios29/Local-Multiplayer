using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private float maxHealth;
    [SerializeField] private int damage;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float targetPlayerRange;
    [SerializeField] private float openRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRate;
    [SerializeField] private float manaCost;

    public float MaxHealth => maxHealth;
    public int Damage => damage;
    public float MoveSpeed => moveSpeed;
    public float TargetPlayerRange => targetPlayerRange;
    public float OpenRange => openRange;
    public float AttackRange => attackRange;
    public float AttackRate => attackRate;
    public float ManaCost => manaCost;
    
}
