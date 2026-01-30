using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PauseScreenCardRenderer : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject pauseScreen;

    private void Start()
    {
        int x = 300;
        int y = 100;

        foreach (Upgrade upgrade in playerData.upgrades)
        {
            GameObject card = Instantiate(cardPrefab);
            card.transform.localScale = new Vector3(.75f,.75f,1);
            card.transform.SetParent(pauseScreen.transform,false);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(x,y);
            card.GetComponent<UpgradeCard>().upgrade = upgrade;
            Debug.Log(card);

            x -= 150;
            
            if (x < -300)
            {
                x = 300;
                y -= 200;
            }
        }
    }
}
