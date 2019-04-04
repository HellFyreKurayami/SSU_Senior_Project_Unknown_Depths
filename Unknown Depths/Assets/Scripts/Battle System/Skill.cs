using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element { FIRE, WATER, WIND, EARTH, HOLY, DARK }

public class Skill : MonoBehaviour{
    public string SpellName;
    public int Tier;
    public int Cost;
    public int Hits;
    public int UnlockLevel;
    public enum SpellType { ATTACK, HEAL }
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

    public void Cast(Entity caster, Entity target)
    {
        //Debug.Log(SpellName + " was cast on " + target);
        float modifier = 1.0f;
        //Uncomment once I get to adding particle effects
        //targetPos = target.transform.position;

        if (Type == SpellType.ATTACK)
        {
            if (caster.Chara.Equals(CharaType.ENEMY))
            {
                modifier = 1.5f;
            }
            int amt = 0;
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
                }
            }
            else
            {
                amt = (int)(caster.PhysAttack * modifier * Random.Range(0.8f, 1.1f)) - target.PhysDefense;
                amt = amt < 1 ? 1 : amt;
                BattleWindow.Instance.UpdateAction(string.Format("{0} used a basic attack and dealt {1} damage to {2}", caster.EntityName, amt, target.EntityName));
                target.Damage(amt);
            }
        }
        else if(Type == SpellType.HEAL)
        {
            if (Target == TargetType.SINGLE)
            {
                target.Heal((int)((target.MaxHealth*(Random.Range(0.07f, 0.1f) * Tier)) + (Random.Range(0.01f, 0.2f) * caster.MagAttack)));
            }
        }
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
