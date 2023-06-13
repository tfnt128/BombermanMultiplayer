using UnityEngine;
using TMPro;

public class ChangePhrase : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public string[] phrases;

    private void Start()
    {
        ChangeText();
    }

    public void ChangeText()
    {
        if (phrases.Length > 0)
        {
            int randomIndex = Random.Range(0, phrases.Length);
            textMeshPro.text = phrases[randomIndex];
        }
        else
        {
            Debug.LogWarning("Nenhuma frase foi adicionada. Por favor, adicione frases no inspector.");
        }
    }
}