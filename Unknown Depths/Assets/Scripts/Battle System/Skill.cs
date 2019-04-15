using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element { FIRE, WATER, WIND, EARTH, HOLY, DARK }

public class Skill : MonoBehaviour{
    public string SpellName;
    public string Description;
    public int Tier;
    public int Cost;
    public int Hits;
    public int UnlockLevel;
    public enum SpellType { ATTACK, HEAL, REVIVE }
    public enum TargetType { SINGLE, AOE }
    public enum SpellAttr { PHYSICAL, MAGIC, BASIC }
    public SpellType Type;
    public TargetType Target;
    public SpellAttr Attr;
    public List<Element> Elements;

    //private Vector3 targetPos;

    /*private void Update()
    {
        if (targetPos != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, .15f);
        }
        else
        {
            Destroy(this.gameObject, 1);
        }
    }*/

    public void Cast(Entity caster, Entity target, bool fromTargetMenu = false)
    {
        //Debug.Log(SpellName + " was cast on " + target);
        float modifier = 1.0f;
        //Uncomment once I get to adding particle effects
        //targetPos = target.transform.position;

        int amt = 0;
        if (Type == SpellType.ATTACK)
        {
            amt = 0;
            if (caster.Chara.Equals(CharaType.ENEMY))
            {
                modifier = 1.5f;
            }
            if (Attr == SpellAttr.MAGIC)
            {
                //Caster Attack is Weak vs Target Element
                if (Resistance(Elements, target.Element) == -1)
                {
                    for (int i = 0; i < Hits; i++)
                    {
                        amt = (int)((caster.MagAttack  * Random.Range(0.5f, 0.7f) * Tier) * modifier) - target.MagDefense;
                        amt = amt < 1 ? 1 : amt;
                        BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} against {2} and dealt {3} damage. How weak...", caster.EntityName, this.SpellName, target.EntityName, amt));
                        target.Damage(amt);
                    }
                }
                //Caster Attack is Strong vs Target Element
                else if (Resistance(Elements, target.Element) == 1)
                {
                    for (int i = 0; i < Hits; i++)
                    {
                        amt = (int)((caster.MagAttack * Random.Range(1.21f, 1.5f) * Tier) * modifier) - target.MagDefense;
                        amt = amt < 1 ? 1 : amt;
                        BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} against {2} and dealt {3} damage. Sugoi!", caster.EntityName, this.SpellName, target.EntityName, amt));
                        target.Damage(amt);
                    }
                }
                //Caster Attack is Even vs Target Element
                else
                {
                    for (int i = 0; i < Hits; i++)
                    {
                        amt = (int)((caster.MagAttack * Random.Range(1.0f, 1.2f) * Tier) * modifier) - target.MagDefense;
                        amt = amt < 1 ? 1 : amt;
                        BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} against {2} and dealt {3} damage.", caster.EntityName, this.SpellName, target.EntityName, amt));
                        target.Damage(amt);
                    }
                }
            }
            else if (Attr == SpellAttr.PHYSICAL)
            {
                for (int i = 0; i < Hits; i++)
                {
                    amt = (int)((caster.PhysAttack * Random.Range(1.2f, 1.5f) * Tier) * modifier) - target.PhysDefense;
                    amt = amt < 1 ? 1 : amt;
                    BattleWindow.Instance.UpdateAction(string.Format("{0} used {1} against {2} and dealt {3} damage.", caster.EntityName, this.SpellName, target.EntityName, amt));
                    target.Damage(amt);
                }
            }
            else
            {
                amt = (int)((caster.PhysAttack * Random.Range(0.7f, 0.9f) * Tier) * modifier) - target.PhysDefense;
                amt = amt < 1 ? 1 : amt;
                BattleWindow.Instance.UpdateAction(string.Format("{0} used a basic attack and dealt {1} damage to {2}", caster.EntityName, amt, target.EntityName));
                target.Damage(amt);
            }
        }
        else if(Type == SpellType.HEAL)
        {
            amt = (int)((target.MaxHealth * (Random.Range(0.15f, 0.30f)) * Tier) + (Random.Range(0.01f, 0.2f) * caster.MagAttack));
            amt = amt < 1 ? 1 : amt;
            if (!fromTargetMenu)
            {
                BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} and healed {2} for {3} HP", caster.EntityName, this.SpellName, target.EntityName, amt));
            }
            target.Heal(amt);
        }
        else if(Type == SpellType.REVIVE)
        {
            if(Tier == 1)
            {
                target.Heal((int)(target.MaxHealth / 2));
                BattleWindow.Instance.UpdateAction(string.Format("{0} cast {2} and brought {1} back to life! sort of...", caster.EntityName, target.EntityName, this.SpellName));
            }
            if(Tier == 2)
            {
                target.Heal(target.MaxHealth);
                BattleWindow.Instance.UpdateAction(string.Format("{0} cast {2} and brought {1} back to life! Completely!", caster.EntityName, target.EntityName, this.SpellName));
            }
        }
    }

    public void CastAOE(Entity caster, List<Entity> targets, bool fromTargetMenu = false)
    {
        int amt = 0;
        float modifier = 1.0f;
        if (Type == SpellType.ATTACK)
        {
            foreach(Entity target in targets)
            {
                amt = 0;
                if (caster.Chara.Equals(CharaType.ENEMY))
                {
                    modifier = 1.5f;
                }
                if (Attr == SpellAttr.MAGIC)
                {
                    //Caster Attack is Weak vs Target Element
                    if (Resistance(Elements, target.Element) == -1)
                    {
                        for (int i = 0; i < Hits; i++)
                        {
                            amt = (int)(caster.MagAttack * modifier * Random.Range(0.3f, 0.7f)) - target.MagDefense;
                            amt = amt < 1 ? 1 : amt;
                            BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} against {2} and dealt {3} damage. How weak...", caster.EntityName, this.SpellName, target.EntityName, amt));
                            target.Damage(amt);
                            Invoke("Dummy", 1.0f);
                        }
                    }
                    //Caster Attack is Strong vs Target Element
                    else if (Resistance(Elements, target.Element) == 1)
                    {
                        for (int i = 0; i < Hits; i++)
                        {
                            amt = (int)(caster.MagAttack * modifier * Random.Range(1.2f, 1.5f)) - target.MagDefense;
                            amt = amt < 1 ? 1 : amt;
                            BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} against {2} and dealt {3} damage. Sugoi!", caster.EntityName, this.SpellName, target.EntityName, amt));
                            target.Damage(amt);
                            Invoke("Dummy", 1.0f);
                        }
                    }
                    //Caster Attack is Even vs Target Element
                    else
                    {
                        for (int i = 0; i < Hits; i++)
                        {
                            amt = (int)(caster.MagAttack * modifier * Random.Range(1.0f, 1.2f)) - target.MagDefense;
                            amt = amt < 1 ? 1 : amt;
                            BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} against {2} and dealt {3} damage.", caster.EntityName, this.SpellName, target.EntityName, amt));
                            target.Damage(amt);
                            Invoke("Dummy", 1.0f);
                        }
                    }
                }
                else if (Attr == SpellAttr.PHYSICAL)
                {
                    for (int i = 0; i < Hits; i++)
                    {
                        amt = (int)(caster.PhysAttack * modifier * Random.Range(1.2f, 1.5f)) - target.PhysDefense;
                        amt = amt < 1 ? 1 : amt;
                        BattleWindow.Instance.UpdateAction(string.Format("{0} used {1} against {2} and dealt {3} damage.", caster.EntityName, this.SpellName, target.EntityName, amt));
                        target.Damage(amt);
                        Invoke("Dummy", 1.0f);
                    }
                }
            }
        }
        else if(Type == SpellType.HEAL)
        {
            amt = 0;
            foreach (Entity target in targets)
            {
                amt = (int)((target.MaxHealth * (Random.Range(0.07f, 0.1f) * Tier)) + (Random.Range(0.01f, 0.2f) * caster.MagAttack));
                amt = amt < 1 ? 1 : amt;
                if (!fromTargetMenu)
                {
                    BattleWindow.Instance.UpdateAction(string.Format("{0} cast {1} and healed {2} for {3} HP", caster.EntityName, this.SpellName, target.EntityName, amt));
                    Invoke("Dummy", 1.0f);
                }
                target.Heal(amt);
            }
        }
    }

    private IEnumerator DealDamage(Entity target, int amt)
    {
        yield return new WaitForSeconds(1.0f);
        target.Damage(amt);
    }

    private IEnumerator Restore(Entity target, int amt)
    {
        yield return new WaitForSeconds(1.0f);
        target.Heal(amt);
    }

    private void Dummy()
    {

    }

    private int Resistance(List<Element> caster, List<Element> target)
    {
        //Fire is weak to Water, but resistant to Earth
        //Water is weak to Wind, but resistant to Fire
        //Wind is weak to Earth, but resistant to Water
        //Earth is weak to Fire, but resistant to Wind
        //Holy and Dark are weak to the other
        int resistance = 0; //0 = No resistance, 1 = Caster Wins, -1 = Target Wins

        //Weak Against
        if (caster.Contains(Element.FIRE) && target.Contains(Element.WATER) || 
            caster.Contains(Element.WATER) && target.Contains(Element.WIND) || 
            caster.Contains(Element.WIND) && target.Contains(Element.EARTH) || 
            caster.Contains(Element.EARTH) && target.Contains(Element.FIRE))
        {
            resistance = -1;
        }
        //Strong Against
        else if (caster.Contains(Element.FIRE) && target.Contains(Element.EARTH) ||
            caster.Contains(Element.EARTH) && target.Contains(Element.WIND) ||
            caster.Contains(Element.WIND) && target.Contains(Element.WATER) ||
            caster.Contains(Element.WATER) && target.Contains(Element.FIRE) ||
            caster.Contains(Element.HOLY) && target.Contains(Element.DARK) ||
            caster.Contains(Element.DARK) && target.Contains(Element.HOLY))
        {
            resistance = 1;
        }
        //Neutral
        return resistance;
    }
}
