using System.Collections;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public float typingSpeed = 0.05f;
    public float fadeOutDuration = 3f;

    private void Start()
    {
        ShowDialog("Hmm... I think I need to train more.");
    }

    public void ShowDialog(string message)
    {
        StopAllCoroutines();
        StartCoroutine(TypeDialog(message));
    }

    IEnumerator TypeDialog(string message)
    {
        dialogText.text = "";
        dialogText.alpha = 1f;

        // 한 글자씩 타이핑
        foreach (char letter in message.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        // 텍스트 완성 후 3초 대기
        yield return new WaitForSeconds(1f);

        // 페이드 아웃
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - (elapsedTime / fadeOutDuration);
            dialogText.alpha = alpha;
            yield return null;
        }

        dialogText.alpha = 0f;
    }
}
