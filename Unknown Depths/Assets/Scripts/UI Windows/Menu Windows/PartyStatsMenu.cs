using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartyStatsMenu : GenericWindow
{
    [SerializeField]
    private List<Button> partyButtons;
    [SerializeField]
    private SpriteRenderer sprite;
    [SerializeField]
    private Text Name, Level, cEXP, nEXP, HP, MP, PAtk, MAtk, PDef, MDef, Spd;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            manager.Open((int)Windows.MenuWindow - 1, false);
            Close();
        }
    }

    public override void Open()
    {
        foreach(Button b in partyButtons)
        {
            EventTrigger et = b.gameObject.AddComponent<EventTrigger>();
            var hover = new EventTrigger.Entry();
            var select = new EventTrigger.Entry();
            hover.eventID = EventTriggerType.PointerEnter;
            select.eventID = EventTriggerType.Select;
            hover.callback.AddListener((e) => DisplayStats(b.GetComponentInChildren<Text>().text));
            select.callback.AddListener((e) => DisplayStats(b.GetComponentInChildren<Text>().text));
            et.triggers.Add(hover);
            et.triggers.Add(select);
        }
        base.Open();
    }

    public void DisplayStats(string name)
    {
        Entity selected = MapMaker.Instance.Players.Find(x => x.EntityName.Equals(name));
        //Debug.Log(selected.EntityName);
        sprite.GetComponent<SpriteRenderer>().sprite = selected.GetComponent<SpriteRenderer>().sprite;
        Name.text = selected.EntityName;
        Level.text = string.Format("Level: {0}", selected.level);
        cEXP.text = string.Format("Experience: {0}", selected.experience);
        nEXP.text = string.Format("To Next Level: {0}", (Mathf.FloorToInt((selected.level) * (30 * (Mathf.Pow(selected.level + 1, 1.5f)))) - selected.experience));
        HP.text = string.Format("Health: {0} / {1}", selected.CurrentHealth, selected.MaxHealth);
        MP.text = string.Format("Magia Points: {0} / {1}", selected.CurrentMagicPoints, selected.MaxMagicPoints);
        PAtk.text = string.Format("Phys. Atk: {0}", selected.PhysAttack);
        MAtk.text = string.Format("Mag. Atk: {0}", selected.MagAttack);
        PDef.text = string.Format("Phys. Def: {0}", selected.PhysDefense);
        MDef.text = string.Format("Mag. Def: {0}", selected.MagDefense);
        Spd.text = string.Format("Speed: {0}", selected.Speed);
    }

}
