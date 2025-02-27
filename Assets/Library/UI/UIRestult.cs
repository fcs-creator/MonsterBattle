using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRestult : MonoBehaviour
{
    public void SetWinMonster(Monster monster) 
    {
        if (monster != null)
        {
            Transform monsterBody = monster.transform.Find("Body");
            Transform monsterImage = transform.Find("MonsterImage");
            Transform monsterName = transform.Find("MonsterName");

            if (monsterBody)
            {
                monsterImage.transform.localScale = monsterBody.localScale;
                monsterImage.GetComponent<Image>().sprite = monsterBody.GetComponent<SpriteRenderer>().sprite;
            }
            
            monsterName.GetComponent<TMP_Text>().text = monster.gameObject.name;
        }
        else
        {
            Debug.LogError("Monster is null.");
        }
    }
}
