using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable
{
    void Damage(float damage);
}

interface IHealable
{
    void Heal(float heal);
}

interface IStorage
{
    void Insert(int num);
    void Take(int num);
}
