
using UnityEngine;

public class Fireball : CardEffect
{
    [SerializeField]
    private int statusEffectDuration = 1;
    [SerializeField]
    private int statusEffectAmount = 1;

    public override void ApplyEffect(Enemy enemy, int amount)
    {
        // rounds a floating point number up to the nearest whole number.
        int adjustedDamage = Mathf.CeilToInt(amount * 1.5f);
        enemy.GetDamage(adjustedDamage);
        enemy.ApplyStatusEffect(new StatusEffect(StatusEffectType.DamageOverTime, statusEffectDuration, statusEffectAmount));
        Debug.Log($"This did: {adjustedDamage} FireBall Damage");
    }
}
