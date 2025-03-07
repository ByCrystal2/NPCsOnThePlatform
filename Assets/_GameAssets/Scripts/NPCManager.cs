using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [SerializeField] Transform stationaryNPCContent;
    Dictionary<string, NPCTalkingController> _currentActiveTalkingNpcDic = new Dictionary<string, NPCTalkingController>();
    public static NPCManager instance { get; private set; }
    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public Vector3 GetRandomStationaryNPCTalkingPoint(NPCTalkingController talkingController)
    {
        int length = stationaryNPCContent.childCount;
        StationaryNPCBehaviour randomNPC = null;

        int attempts = 0;
        // Maksimum deneme sayýsý, sonsuz döngü olmasýný engellemek için
        int maxAttempts = 100;
        do
        {
            int randomIndex = UnityEngine.Random.Range(0, length);
            randomNPC = stationaryNPCContent.GetChild(randomIndex).GetComponent<StationaryNPCBehaviour>();
            attempts++;

            if (attempts > maxAttempts)
            {
                Debug.LogError("Çok fazla deneme yapýldý, boþta NPC bulunamadý.");
                talkingController.GetComponent<NPCBehaviour>().isWaitingMat = true;
                return Vector3.zero;  // Geçerli bir NPC bulunamazsa sýfýr noktasý döndür
            }
        } while (_currentActiveTalkingNpcDic.Values.Contains(randomNPC.GetComponent<NPCTalkingController>()));
        
        NPCTalkingController randomTalking = randomNPC.GetComponent<NPCTalkingController>();

        _currentActiveTalkingNpcDic.Add(talkingController.GetGuidID(), randomTalking);
        Vector3 randomTalkingPoint = randomNPC.GetTalkingPoint();
        return randomTalkingPoint;
    }
    public void MatchingNPCDialog(NPCBehaviour npcBehaviour, string guidID)
    {
        if(npcBehaviour.TryGetComponent(out NPCTalkingController talkingController))
        {
            npcBehaviour.AgentOtoRotateActive(false);
            NPCTalkingController _currentStationaryNpcTalking = _currentActiveTalkingNpcDic[guidID];
            talkingController.StartTalking(_currentStationaryNpcTalking.transform.position);
            _currentStationaryNpcTalking.StartTalking(npcBehaviour.transform.position);
        }
        else
        {
            Debug.Log(npcBehaviour.name + " adli npc'nin dialog ozelligi bulunmamaktadir.");
        }
    }
    public void EndDialog(NPCBehaviour behaviour, string guidID)
    {
        if (behaviour.TryGetComponent(out NPCTalkingController talkingController))
        {
            NPCTalkingController _currentStationaryNpcTalking = _currentActiveTalkingNpcDic[guidID];
            talkingController.StopTalking();
            _currentStationaryNpcTalking.StopTalking();
            behaviour.FindNewTarget();
            behaviour.AgentOtoRotateActive(true);

            _currentActiveTalkingNpcDic.Remove(guidID);
        }
        else
        {
            Debug.Log(behaviour.name + " adli npc'nin dialog ozelligi bulunmamaktadir.");
        }
    }
    public int GetTotalStationaryNPCCount()
    {
        return stationaryNPCContent.childCount;
    }
    public bool AreThereAnyNPCsAvailable()
    {
        return _currentActiveTalkingNpcDic.Count < stationaryNPCContent.childCount;
    }
}
