using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Infantry : MonoBehaviour
{
    [HideInInspector]
    public DamageReceiver DR;
    [HideInInspector]
    public HealReceiver HR;

    public float start_Health, start_Armor, start_Shield;
    public float stats_Health, stats_Armor, stats_Shield;
    float end_Health, end_Armor, end_Shield;

    bool hasShield = true;

    bool canRechargeShield = true;
    float rechargeTimer = 0;
    public float rechargeTimerDelay = 60;

    public bool isDead = false;

    void Start()
    {
        DR = GetComponent<DamageReceiver>();
        HR = GetComponent<HealReceiver>();

        end_Health = start_Health * stats_Health;
        end_Armor = start_Armor * stats_Armor;
        end_Shield = start_Shield * stats_Shield;
    }

    void Update()
    {
        ApplyHeal(HR.currentHeal/3, HR.currentHeal / 3, HR.currentHeal / 3);
        ApplyDamage(DR.currentDamage);

        RechargeShield();

        if (end_Health <= 0)
        {
            isDead = true;
        }
    }

    void RechargeShield()
    {
        if (end_Shield < start_Shield * stats_Shield && canRechargeShield)
        {
            end_Shield += 5 * Time.deltaTime;
        }

        if (end_Shield >= stats_Shield * stats_Shield)
        {
            end_Shield = start_Shield * stats_Shield;
        }

        if (!canRechargeShield)
        {
            if (rechargeTimer < rechargeTimerDelay)
            {
                rechargeTimer += Time.deltaTime;
            } else if (rechargeTimer >= rechargeTimerDelay)
            {
                canRechargeShield = true;
            }
        }
    }

    void ApplyDamage(float dammage)
    {
        if (!hasShield)
        {
            end_Health -= dammage;
        }
        else
        {
            end_Shield -= dammage;
        }

        if (end_Health <= 0)
        {
            end_Health = 0;
        }

        if (end_Shield <= 0)
        {
            end_Shield = 0;
            hasShield = false;
        }
    }

    void ApplyHeal(float healHealth, float healArmor, float healShield)
    {
        if (end_Health < start_Health * stats_Health && end_Health > 0)
        {
            end_Health += healHealth;

            if (end_Health > start_Health * stats_Health)
            {
                end_Health = start_Health * stats_Health;
            }
        }

        if (end_Armor < start_Armor * stats_Armor)
        {
            end_Armor += healArmor;

            if (end_Armor > start_Armor * stats_Armor)
            {
                end_Armor = start_Armor * stats_Armor;
            }
        }

        if (end_Shield < start_Shield * stats_Shield)
        {
            end_Shield += healShield;

            if (end_Shield > start_Shield * stats_Shield)
            {
                end_Shield = start_Shield * stats_Shield;
            }
        }
    }
}
