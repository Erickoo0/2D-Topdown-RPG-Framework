using System.Collections.Generic;
using UnityEngine;

namespace Quest
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }

        private List<QuestSo> _questList = new List<QuestSo>();
        public List<QuestSo> QuestList => _questList;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                Debug.unityLogger.Log("Multiple QuestManagers detected. Disabling script.");
                return;
            }

            Instance = this;
        }
        
        public void AddQuest(QuestSo quest)
        {
            _questList.Add(quest);
        }
        
        public void RemoveQuest(QuestSo quest)
        {
            _questList.Remove(quest);
        }

    }
}