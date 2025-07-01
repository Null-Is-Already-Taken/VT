using UnityEngine;

namespace VT.SkillSystem
{
    /// <summary>
    /// MonoBehaviour component for executing skills in the game world
    /// </summary>
    public class SkillExecutor : MonoBehaviour
    {
        [Header("Skill Configuration")]
        [SerializeField] private Skill skill;
        [SerializeField] private KeyCode activationKey = KeyCode.Space;
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private bool autoTarget = true;
        
        private float lastExecutionTime;
        private bool canExecute => Time.time - lastExecutionTime >= cooldown;
        
        private void Update()
        {
            if (Input.GetKeyDown(activationKey) && canExecute)
            {
                ExecuteSkill();
            }
        }
        
        /// <summary>
        /// Execute the assigned skill
        /// </summary>
        public void ExecuteSkill()
        {
            if (skill == null)
            {
                Debug.LogWarning("No skill assigned to SkillExecutor!");
                return;
            }
            
            GameObject target = null;
            if (autoTarget)
            {
                target = FindNearestTarget();
            }
            
            skill.Execute(gameObject, target);
            lastExecutionTime = Time.time;
            
            Debug.Log($"Executed skill: {skill.SkillName}");
        }
        
        /// <summary>
        /// Execute a specific skill
        /// </summary>
        public void ExecuteSkill(Skill skillToExecute, GameObject target = null)
        {
            if (skillToExecute == null) return;
            
            skillToExecute.Execute(gameObject, target);
            lastExecutionTime = Time.time;
            
            Debug.Log($"Executed skill: {skillToExecute.SkillName}");
        }
        
        /// <summary>
        /// Find the nearest valid target
        /// </summary>
        private GameObject FindNearestTarget()
        {
            // Simple implementation - can be enhanced based on your game's needs
            var colliders = Physics.OverlapSphere(transform.position, 10f);
            GameObject nearest = null;
            float nearestDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject == gameObject) continue;
                
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearest = collider.gameObject;
                    nearestDistance = distance;
                }
            }
            
            return nearest;
        }
        
        /// <summary>
        /// Set the skill to execute
        /// </summary>
        public void SetSkill(Skill newSkill)
        {
            skill = newSkill;
        }
        
        /// <summary>
        /// Get the current skill
        /// </summary>
        public Skill GetSkill()
        {
            return skill;
        }
        
        /// <summary>
        /// Check if the skill can be executed (cooldown check)
        /// </summary>
        public bool CanExecute()
        {
            return canExecute;
        }
        
        /// <summary>
        /// Get the remaining cooldown time
        /// </summary>
        public float GetRemainingCooldown()
        {
            return Mathf.Max(0, cooldown - (Time.time - lastExecutionTime));
        }
    }
} 