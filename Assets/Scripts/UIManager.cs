using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public GameObject magicDamageTextPrefab;
    public GameObject manaRecoveryTextPrefab;
    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas =FindObjectOfType<Canvas>();
       
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged +=(CharacterTookDamage);
        CharacterEvents.characterHealed += (CharacterHealed);
        CharacterEvents.characterMagicDamaged += (CharacterTookMagicDamage);
        CharacterEvents.characterManaRecovery += (CharacterManaRecovery);
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged-=(CharacterTookDamage);
        CharacterEvents.characterHealed -= (CharacterHealed);
        CharacterEvents.characterMagicDamaged -= (CharacterTookMagicDamage);
        CharacterEvents.characterManaRecovery -= (CharacterManaRecovery);
    }

    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text= damageReceived.ToString();    
    }
    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }
    public void CharacterTookMagicDamage(GameObject character, int magicdamageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(magicDamageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = magicdamageReceived.ToString();
    }
    public void CharacterManaRecovery(GameObject character, int manaRecoveryReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(manaRecoveryTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = manaRecoveryReceived.ToString();
    }
}
