using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    public Upgrade upgrade;
    [SerializeField] private PlayerData playerData;

    private Image _image;
    private GameObject _tooltip;
    private TMP_Text _name;
    private TMP_Text _rarity;
    private TMP_Text _desc;

    private void Start()
    {
        _image = GetComponent<Image>();
        _tooltip = transform.GetChild(0).gameObject;
        _name = _tooltip.transform.GetChild(0).GetComponent<TMP_Text>();
        _rarity = _tooltip.transform.GetChild(1).GetComponent<TMP_Text>();
        _desc = _tooltip.transform.GetChild(2).GetComponent<TMP_Text>();

        _image.material = upgrade.cardImage;
        _name.text = upgrade.name;
        _rarity.text = upgrade.rarity.ToString();
        _desc.text = upgrade.description;

        _tooltip.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        _tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData e)
    {
        _tooltip.SetActive(false);       
    }

    public void OnPointerMove(PointerEventData e)
    {
        _tooltip.transform.position = new Vector3(e.position.x,e.position.y,-100);

        _tooltip.GetComponent<RectTransform>().pivot = new Vector2(-0.01f,-0.01f);
    }

    public void OnPointerClick(PointerEventData e)
    {
        playerData.upgrades.Add(upgrade);

        SceneManager.LoadScene(1);
    }
}
