using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellWindow : GenericWindow
{
    [SerializeField]
    private GameObject spellPanel;
    [SerializeField]
    private Button button;

    void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            BattleWindow.Instance.ToggleActionState(true);
            Close();
        }
    }

    public void BuildSpellList(List<Skill> spells)
    {
        //Remove visable spells in spell panel
        if (spellPanel.transform.childCount > 0)
        {
            foreach (Button b in spellPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }

        bool selected = false;
        foreach (Skill s in spells)
        {
            if (!s.SpellName.Equals("Basic Attack"))
            {
                Button spellButton = Instantiate<Button>(button, spellPanel.transform);
                spellButton.GetComponentInChildren<Text>().text = s.SpellName;
                spellButton.onClick.AddListener(() => BattleWindow.Instance.SelectSpell(s));
                var nav = spellButton.navigation;
                nav.mode = Navigation.Mode.Automatic;
                spellButton.navigation = nav;
                if (!selected)
                {
                   this.firstSelected = spellButton.gameObject;
                    selected = true;
                }
            }
        }
    }

    public override void Close()
    {
        if (spellPanel.transform.childCount > 0)
        {
            foreach (Button b in spellPanel.transform.GetComponentsInChildren<Button>())
            {
                Destroy(b.gameObject);
            }
        }
        base.Close();
    }
}
