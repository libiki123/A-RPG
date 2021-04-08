using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackedScrollingText : MonoBehaviour, IAttackable     
{
    public ScrollingText Text;
    public Color TextColor;

    public void OnAttack(GameObject attacker, Attack attack)
    {
        var text = attack.Damage.ToString();

        var scrollingText = Instantiate(Text, transform.position, Quaternion.identity);     // create scrolling text base on the attacker damage
        scrollingText.SetText(text);
        scrollingText.SetColor(TextColor);
    }
}
