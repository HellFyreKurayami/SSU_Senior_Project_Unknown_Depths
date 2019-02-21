using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharaType { ENEMY, PLAYER }

public class Entity : MonoBehaviour {

    public string EntityName;
    public int CurrentHealth;
    public int MaxHealth;
    public int CurrentMagicPoints;
    public int MaxMagicPoints;
    public int PhysAttack;
    public int PhysDefense;
    public int MagAttack;
    public int MagDefense;
    public int Speed;
    public CharaType Chara;
    public List<Element> Element;
    public List<Skill> Skills;
    public List<Effect> Effects;

    public void OnTurn()
    {
        foreach (Effect status in Effects)
        {
            if(status.Turns != 0)
            {
                status.Turns -= 1;
                
            }
            status.ability();
        }
    }

    public bool CastSpell(Skill s, Entity e)
    {
        bool cast = CurrentMagicPoints >= s.Cost;

        if (cast)
        {
            //Uncomment this once I add particle effects. for now just need the spell to deal damage
            //Skill toCast = Instantiate<Skill>(s, transform.position, Quaternion.identity);
            CurrentMagicPoints -= s.Cost;
            s.Cast(this, e);
        }

        return cast;
    }

    public void Damage(int amt)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - amt, 0);
        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(int amt)
    {
        CurrentHealth += amt;
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
        Debug.LogFormat("{0} has died...", EntityName);
    }
}
