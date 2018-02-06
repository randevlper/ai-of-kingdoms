using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damage);
}

public interface IHealable
{
    void Heal(float heal);
}

public interface IStorage
{
    void Insert(int num);
}

public interface IAI
{
    void SetAI(int num, Material mat);
}
