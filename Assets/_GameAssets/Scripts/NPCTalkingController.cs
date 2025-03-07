using System.Collections;
using UnityEngine;

public class NPCTalkingController : MonoBehaviour
{
    string _myGuidID;
    Animator _animator;
    private bool isTalking = false; // Konu�ma durumu kontrol�
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _myGuidID = System.Guid.NewGuid().ToString();
    }

    public void StartTalking(Vector3 target) => StartCoroutine(IEStartTalking(target));

    IEnumerator IEStartTalking(Vector3 target)
    {
        if (!isTalking)
        {
            isTalking = true;
            yield return StartCoroutine(RotateTowards(target)); // �nce d�n��� tamamla
            StartCoroutine(TalkingLoop()); // Sonra konu�may� ba�lat
        }
    }

    private IEnumerator RotateTowards(Vector3 target)
    {        

        Vector3 direction = (target - transform.position).normalized;

        // E�er hedef ile NPC'nin pozisyonu ayn�ysa d�n�� yapma
        if (direction.magnitude < 0.01f)
        {
            Debug.LogWarning(name + " i�in d�n�� iptal edildi! (Hedef ile ayn� konumda)");
            yield break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float rotationSpeed = 300f; // Daha h�zl� d�n�� i�in art�r�ld�

        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // E�er d�n�� tamamland�ysa ��k
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                Debug.Log(name + " d�n�� tamamland�!");

                yield break;
            }

            yield return null;
        }
    }




    public void StopTalking()
    {
        isTalking = false;
        int length = _animator.runtimeAnimatorController.animationClips.Length - 1; // Idle cikarildi.
        for (int i = 1; i < length; i++)
        {
            _animator.SetBool("Talking" + i, false);
        }
    }
    private IEnumerator TalkingLoop()
    {
        while (isTalking)
        {
            int length = _animator.runtimeAnimatorController.animationClips.Length;
            int randomIndex = Random.Range(1, length - 1); 

            // Yeni animasyonu ba�lat
            _animator.SetBool("Talking" + randomIndex, true);

            // O anki animasyonun s�resini al
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationDuration = stateInfo.length;

            yield return new WaitForSeconds(animationDuration);

            if (isTalking)
            {
                _animator.SetBool("Talking" + randomIndex, false);
                yield return new WaitForSeconds(0.1f); // K�sa bir bekleme s�resi
            }
        }
    }
    public bool IsTalking() 
    {
        return isTalking;
    }
    public string GetGuidID()
    {
        return _myGuidID;
    }
}
