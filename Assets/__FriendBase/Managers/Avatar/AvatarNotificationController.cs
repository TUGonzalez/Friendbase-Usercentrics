using System.Collections;
using TMPro;
using UnityEngine;

namespace Managers.Avatar
{
    public class AvatarNotificationController : MonoBehaviour
    {
        private bool isPlaying;
        
        [SerializeField] private GameObject sprite;
        [SerializeField] private TextMeshPro text;

        private void OnEnable()
        {
            sprite.SetActive(false);
            text.gameObject.SetActive(false);
        }

        public void PlayNotification()
        {
            if (!isPlaying) StartCoroutine(CoroutinePlayNotification());
        }

        IEnumerator CoroutinePlayNotification()
        {
            const float seconds = 10f; 
            isPlaying = true;
            
            sprite.SetActive(true);
            text.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(seconds);
            
            isPlaying = false;
            
            sprite.SetActive(false);
            text.gameObject.SetActive(false);

        }

    }
}
