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
                //monsterImage.transform.localScale = monsterBody.localScale;
                Image image = monsterImage.GetComponent<Image>();
                image.sprite= monsterBody.GetComponent<SpriteRenderer>().sprite;
                RectTransform rectTransform = image.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(300, 300);
            }
            
            monsterName.GetComponent<TMP_Text>().text = monster.gameObject.name;
        }
        else
        {
            Debug.LogError("Monster is null.");
        }
    }
}
