using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellWindow : GenericWindow
{
    [SerializeField]
    private GameObject spellPanel;
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text spellText;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            BattleWindow.Instance.ToggleActionState(true);
            BattleWindow.Instance.SelectButton(true);
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
        
        foreach (Skill s in spells)
        {
            if (!s.SpellName.Equals("Basic Attack") && BattleWindow.Instance.GetCurrentCharacter().level >= s.UnlockLevel)
            {
                Button spellButton = Instantiate<Button>(button, spellPanel.transform);
                spellButton.GetComponentInChildren<Text>().text = s.SpellName;
                spellButton.onClick.AddListener(() => BattleWindow.Instance.SelectSpell(s));
                EventTrigger trigger = spellButton.gameObject.AddComponent<EventTrigger>();
                var hover = new EventTrigger.Entry();
                var select = new EventTrigger.Entry();
                hover.eventID = EventTriggerType.PointerEnter;
                select.eventID = EventTriggerType.Select;
                hover.callback.AddListener((e) => DisplayInformation(string.Format("Cost: {0} | {1}",s.Cost, s.Description)));
                select.callback.AddListener((e) => DisplayInformation(string.Format("Cost: {0} | {1}", s.Cost, s.Description)));
                trigger.triggers.Add(hover);
                trigger.triggers.Add(select);
                //spellButton.OnSelect();
                var nav = spellButton.navigation;
                if(spells.Count < 5)
                {
                    nav.mode = Navigation.Mode.Automatic;
                }
                else
                {
                    nav.mode = Navigation.Mode.Explicit;
                }
                spellButton.navigation = nav;
            }
        }
        
        Button[] temp = spellPanel.transform.GetComponentsInChildren<Button>();
        if(temp.Length > 0)
        {
            if(temp.Length > 4)
            {
                //Navigation Reference
                // %1      %0
                // 0(1)    1(2)
                // 2(3)    3(4)
                // 4(5)    5(6)
                // 6(7)    7(8)
                // 8(9)
                for(int i = 0; i < temp.Length; i++)
                {
                    var nav = temp[i].navigation;
                    if (i == 0)
                    {
                        nav.selectOnDown = temp[2];
                        nav.selectOnUp = null;
                        nav.selectOnLeft = null;
                        nav.selectOnRight = temp[1];
                    }
                    else if (i == 1)
                    {
                        nav.selectOnDown = temp[3];
                        nav.selectOnUp = null;
                        nav.selectOnLeft = temp[0];
                        nav.selectOnRight = null;
                    }
                    else if (i == temp.Length - 2)
                    {
                        if (temp.Length % 2 == 1)
                        {
                            nav.selectOnDown = temp[temp.Length - 1];
                            nav.selectOnUp = temp[temp.Length - 4];
                            nav.selectOnLeft = temp[temp.Length - 3];
                            nav.selectOnRight = null;
                        }
                        else
                        {
                            nav.selectOnDown = null;
                            nav.selectOnUp = temp[temp.Length - 4];
                            nav.selectOnLeft = null;
                            nav.selectOnRight = temp[temp.Length - 1];
                        }

                    }
                    else if (i == temp.Length - 1)
                    {
                        if (temp.Length % 2 == 1)
                        {
                            nav.selectOnDown = null;
                            nav.selectOnUp = temp[temp.Length - 3];
                            nav.selectOnLeft = null;
                            nav.selectOnRight = null;
                        }
                        else
                        {
                            nav.selectOnDown = null;
                            nav.selectOnUp = temp[temp.Length - 3];
                            nav.selectOnLeft = temp[temp.Length - 2];
                            nav.selectOnRight = null;
                        }
                    }
                    else
                    {
                        if ((i + 1) % 2 == 1)
                        {
                            nav.selectOnDown = temp[i + 2];
                            nav.selectOnUp = temp[i - 2];
                            nav.selectOnLeft = null;
                            nav.selectOnRight = temp[i + 1];
                        }
                        else
                        {
                            nav.selectOnDown = temp[i + 2];
                            nav.selectOnUp = temp[i - 2];
                            nav.selectOnLeft = temp[i - 1];
                            nav.selectOnRight = null;
                        }
                    }
                    temp[i].navigation = nav;
                }
            }
            firstSelected = temp[0].gameObject;
            eventSystem.SetSelectedGameObject(firstSelected);
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

    public void DisplayInformation(string text)
    {
        spellText.text = text;
    }
}
