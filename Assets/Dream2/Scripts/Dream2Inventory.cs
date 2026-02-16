using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using Unity.VisualScripting;
using System.IO;

public class Dream2Inventory : MonoBehaviour
{
    public static Dream2Inventory Instance;

    [SerializeField] Animator bodyAnimator;
    [SerializeField] Animator legsAnimator;

    // What the player currently owns
    public List<Dream2Item> items = new List<Dream2Item>();

    public Dream2Item equippedOutfit;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged InventoryChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddItem(Dream2Item item)
    {
        items.Add(item);
        if (item.type == ItemType.File)
        {
            Dream2Manager.Instance.isFileGot = true;
        }
        InventoryChanged?.Invoke();
    }

    public bool HasItem(Dream2Item item)
    {
        return items.Contains(item);
    }

    public void RemoveItem(Dream2Item item)
    {
        items.Remove(item);
        InventoryChanged?.Invoke();
    }

    public void EquipItem(Dream2Item item)
    {
        if (item.type == ItemType.Clothing)
        {
            equippedOutfit = item;
            bodyAnimator.SetInteger("clothType", item.outfitIndex);
            legsAnimator.SetInteger("clothType", item.outfitIndex);
            Time.timeScale = 0.01f;
            Time.timeScale = 0f;
        }
        else if (item.type == ItemType.File)
        {
            items.Remove(item);
            Dream2Manager.Instance.fileDeleted = true;
            StartCoroutine(HintFadeIn(0.5f));
        }
        InventoryChanged?.Invoke();
    }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject missionOptional;
    [SerializeField] private GameObject missionOpitonal0;

    private IEnumerator HintFadeIn(float fadeDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
        missionOpitonal0.SetActive(false);
        StartCoroutine(AutoFadeOut());
    }

    private IEnumerator HintFateOut(float fadeDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
            yield return null;
        }
        canvasGroup.alpha = 0;
        missionOptional.SetActive(false);
    }

    private IEnumerator AutoFadeOut()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(HintFateOut(0.5f));
    }

    public List<Dream2Item> GetItemList()
    {
        return items;
    }
}
