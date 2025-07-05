using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public Animator animator; // Referencia al Animator del objeto vacío

    void Start()
    {
        StartCoroutine(WaitForAnimationAndChangeScene());
    }

    IEnumerator WaitForAnimationAndChangeScene()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(0);
    }
}
