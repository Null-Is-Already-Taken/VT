using UnityEngine;
using VT.ReusableSystems.HealthSystem;

namespace VT.ReusableSystems.ProceduralSkillBuilder
{
    public class SkillTest : MonoBehaviour
    {
        public GameObject caster;
        public float aoeRadius = 5f;

        Skill simple;
        Skill medium;
        Skill high;
        Skill sub;
        Skill heal;
        Skill teleport;

        private void Awake()
        {
            caster = gameObject;
        }

        private void Start()
        {
            Debug.Log("--- Build Skill Tests ---");
            BuildSimpleSkill();
            BuildMediumSkill();
            BuildHighSkill();
            BuildSubSkill();
            BuildHealSkill();
            BuildTeleportSkill();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("--- Execute Simple Skill ---");
                simple.From(caster).Execute();
                Log(simple);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("--- Execute Medium Skill ---");
                medium.From(caster).Execute();
                Log(medium);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("--- Execute High Skill ---");
                high.From(caster).Execute();
                Log(high);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.Log("--- Execute Heal Skill ---");
                heal.From(caster).Execute();
                Log(heal);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.Log("--- Execute Teleport Skill ---");
                teleport.From(caster).Execute();
                Log(teleport);
            }
        }

        private void BuildSimpleSkill()
        {
            simple = Skill.Build("Simple Strike")
                .Target(t => t.Radius(aoeRadius, TargetingMode.Enemies))
                .Do(new DamageAction(10f, StatType.Strength, 1.0f, DamageType.Physical));
        }

        private void BuildMediumSkill()
        {
            medium = Skill.Build("Flame Burst")
                .Target(t => t.Radius(aoeRadius, TargetingMode.Enemies))
                .Do(new DamageAction()
                    .Base(20f)
                    .ScaleWith(StatType.Intelligence, 1.5f)
                    .AsType(DamageType.Fire))
                .Then(new StatusAction("Burning", 5f));
        }

        private void BuildHighSkill()
        {
            high = Skill.Build("Plague Nova")
                .Target(t => t.Radius(aoeRadius, TargetingMode.Enemies))
                .Do(new DamageAction()
                    .Base(15f)
                    .ScaleWith(StatType.Intelligence, 1.2f)
                    .AsType(DamageType.Poison))
                .Then(new StatusAction("Plagued", 6f))
                .Then(c =>
                {
                    foreach (var t in c.Targets)
                    {
                        t.OnDeath(() =>
                        {
                            sub.From(caster).Execute();
                        }, this, noDuplicate: true, context: c);
                    }
                });
        }

        private void BuildSubSkill()
        {
            sub = Skill.Build("Plague Spread")
                .Target(t => t.Radius(20f, TargetingMode.Enemies))
                .Do(new DamageAction()
                    .Base(10f)
                    .ScaleWith(StatType.Intelligence, 0.8f)
                    .AsType(DamageType.Poison));
        }

        private void BuildHealSkill()
        {
            heal = Skill.Build("Healing Wave")
                .Target(t => t.Radius(aoeRadius, TargetingMode.Allies))
                .Do(new HealAction()
                    .Base(25f)
                    .ScaleWith(StatType.Intelligence, 1.0f)
                    .PreventOverheal());
        }

        private void BuildTeleportSkill()
        {
            teleport = Skill.Build("Shadow Step")
                .Target(t => t.Radius(aoeRadius, TargetingMode.Enemies))
                .Do(new TeleportAction()
                    .Type(TeleportAction.TeleportType.BehindTarget)
                    .MaxDistance(10f)
                    .RequireLineOfSight(false))
                .Then(new DamageAction(5f, StatType.Dexterity, 0.5f, DamageType.Physical));
        }

        private void Log(Skill skill)
        {
            Debug.Log(skill);
        }
    }
}
