using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rhisis.Core.Data;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Game;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.World.Game.Entities;
using Rhisis.World.Game.Structures;
using Rhisis.World.Packets;
using Rhisis.World.Systems.Battle;
using Rhisis.World.Systems.Battle.Arbiters;
using Rhisis.World.Systems.Inventory;
using Rhisis.World.Systems.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhisis.World.Systems.Skills
{
    [Injectable]
    internal class SkillSystem : ISkillSystem
    {
        /// <summary>
        /// Skill points usage based on the job type.
        /// </summary>
        private static readonly IReadOnlyDictionary<DefineJob.JobType, int> SkillPointUsage = new Dictionary<DefineJob.JobType, int>
        {
            { DefineJob.JobType.JTYPE_BASE, 1 },
            { DefineJob.JobType.JTYPE_EXPERT, 2 },
            { DefineJob.JobType.JTYPE_PRO, 3 },
            { DefineJob.JobType.JTYPE_MASTER, 3 },
            { DefineJob.JobType.JTYPE_HERO, 3 }
        };

        private readonly ILogger<SkillSystem> _logger;
        private readonly IRhisisDatabase _database;
        private readonly IGameResources _gameResources;
        private readonly IBattleSystem _battleSystem;
        private readonly IInventorySystem _inventorySystem;
        private readonly IProjectileSystem _projectileSystem;
        private readonly ISkillPacketFactory _skillPacketFactory;
        private readonly ITextPacketFactory _textPacketFactory;
        private readonly IPlayerPacketFactory _playerPacketFactory;
        private readonly ISpecialEffectPacketFactory _specialEffectPacketFactory;
        private readonly IMoverPacketFactory _moverPacketFactory;

        /// <inheritdoc />
        public int Order => 1;

        public SkillSystem(ILogger<SkillSystem> logger, IRhisisDatabase database, IGameResources gameResources, 
            IBattleSystem battleSystem, IInventorySystem inventorySystem, IProjectileSystem projectileSystem, 
            ISkillPacketFactory skillPacketFactory, ITextPacketFactory textPacketFactory, IPlayerPacketFactory playerPacketFactory,
            ISpecialEffectPacketFactory specialEffectPacketFactory, IMoverPacketFactory moverPacketFactory)
        {
            _logger = logger;
            _database = database;
            _gameResources = gameResources;
            _battleSystem = battleSystem;
            _inventorySystem = inventorySystem;
            _projectileSystem = projectileSystem;
            _skillPacketFactory = skillPacketFactory;
            _textPacketFactory = textPacketFactory;
            _playerPacketFactory = playerPacketFactory;
            _specialEffectPacketFactory = specialEffectPacketFactory;
            _moverPacketFactory = moverPacketFactory;
        }

        /// <inheritdoc />
        public void Initialize(IPlayerEntity player)
        {
            IEnumerable<SkillInfo> jobSkills = GetSkillsByJob(player.PlayerData.Job);
            IEnumerable<DbSkill> playerSkills = _database.Skills.Where(x => x.CharacterId == player.PlayerData.Id).AsNoTracking().AsEnumerable();

            player.SkillTree.Skills = (from x in jobSkills
                                       join s in playerSkills on x.SkillId equals s.SkillId into dbSkills
                                       from dbSkill in dbSkills.DefaultIfEmpty()
                                       select new SkillInfo(x.SkillId, player.PlayerData.Id, _gameResources.Skills[x.SkillId], dbSkill?.Level ?? default, dbSkill?.Id)).ToList();
        }

        /// <inheritdoc />
        public void Save(IPlayerEntity player)
        {
            var skillsSet = from x in _database.Skills.Where(x => x.CharacterId == player.PlayerData.Id).AsNoTracking().ToList()
                            join s in player.SkillTree.Skills on
                             new { x.SkillId, x.CharacterId }
                             equals
                             new { s.SkillId, s.CharacterId }
                            select new { DbSkill = x, PlayerSkill = s };

            foreach (var skillToUpdate in skillsSet)
            {
                skillToUpdate.DbSkill.Level = (byte)skillToUpdate.PlayerSkill.Level;

                _database.Skills.Update(skillToUpdate.DbSkill);
            }

            foreach (SkillInfo skill in player.SkillTree.Skills)
            {
                if (!skill.DatabaseId.HasValue && skill.Level > 0)
                {
                    var newSkill = new DbSkill
                    {
                        SkillId = skill.SkillId,
                        Level = (byte)skill.Level,
                        CharacterId = player.PlayerData.Id
                    };

                    _database.Skills.Add(newSkill);
                }
            }

            _database.SaveChanges();
        }

        public IEnumerable<SkillInfo> GetSkillsByJob(DefineJob.Job job)
        {
            var skillsList = new List<SkillInfo>();

            if (_gameResources.Jobs.TryGetValue(job, out JobData jobData) && jobData.Parent != null)
            {
                skillsList.AddRange(GetSkillsByJob(jobData.Parent.Id));
            }

            IEnumerable<SkillInfo> jobSkills = from x in _gameResources.Skills.Values
                                               where x.Job == jobData.Id &&
                                                     x.JobType != DefineJob.JobType.JTYPE_COMMON &&
                                                     x.JobType != DefineJob.JobType.JTYPE_TROUPE
                                               select new SkillInfo(x.Id, -1, x);

            skillsList.AddRange(jobSkills);

            return skillsList;
        }

        /// <inheritdoc />
        public void UpdateSkills(IPlayerEntity player, IReadOnlyDictionary<int, int> skillsToUpdate)
        {
            int requiredSkillPoints = 0;

            foreach (KeyValuePair<int, int> skill in skillsToUpdate)
            {
                int skillId = skill.Key;
                int skillLevel = skill.Value;
                SkillInfo playerSkill = player.SkillTree.GetSkill(skillId);

                if (playerSkill == null)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                    _logger.LogError($"Cannot find skill with id '{skillId}' for player '{player}'.");
                    return;
                }

                if (playerSkill.Level == skillLevel)
                {
                    // Skill hasn't change
                    continue;
                }

                if (player.Object.Level < playerSkill.Data.RequiredLevel)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                    _logger.LogError($"Cannot update skill with '{skillId}' for player '{player}'. Player need to be level '{playerSkill.Data.RequiredLevel}' to learn this skill.");
                    return;
                }

                if (!CheckRequiredSkill(playerSkill.Data.RequiredSkillId1, playerSkill.Data.RequiredSkillLevel1, skillsToUpdate))
                {
                    SkillData requiredSkill1 = _gameResources.Skills[playerSkill.Data.RequiredSkillId1];

                    _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                    _logger.LogError($"Cannot update skill with '{skillId}' for player '{player}'. Skill '{requiredSkill1.Name}' must be at least Lv.{requiredSkill1.RequiredSkillLevel1}");
                    return;
                }

                if (!CheckRequiredSkill(playerSkill.Data.RequiredSkillId2, playerSkill.Data.RequiredSkillLevel2, skillsToUpdate))
                {
                    SkillData requiredSkill2 = _gameResources.Skills[playerSkill.Data.RequiredSkillId2];

                    _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                    _logger.LogError($"Cannot update skill with '{skillId}' for player '{player}'. Skill '{requiredSkill2.Name}' must be at least Lv.{requiredSkill2.RequiredSkillLevel1}");
                    return;
                }

                if (skillLevel < 0 || skillLevel < playerSkill.Level || skillLevel > playerSkill.Data.MaxLevel)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                    _logger.LogError($"Cannot update skill with '{skillId}' for player '{player}'. The skill level is out of bounds.");
                    return;
                }

                if (!SkillPointUsage.TryGetValue(playerSkill.Data.JobType, out int requiredSkillPointAmount))
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                    _logger.LogError($"Cannot update skill with '{skillId}' for player '{player}'. Cannot find required skill point for job type '{playerSkill.Data.JobType}'.");
                    return;
                }

                requiredSkillPoints += (skillLevel - playerSkill.Level) * requiredSkillPointAmount;
            }

            if (player.Statistics.SkillPoints < requiredSkillPoints)
            {
                _logger.LogError($"Cannot update skills for player '{player}'. Not enough skill points.");
                _textPacketFactory.SendDefinedText(player, DefineText.TID_RESKILLPOINT_ERROR);
                return;
            }

            player.Statistics.SkillPoints -= (ushort)requiredSkillPoints;

            foreach (KeyValuePair<int, int> skill in skillsToUpdate)
            {
                int skillId = skill.Key;
                int skillLevel = skill.Value;

                player.SkillTree.SetSkillLevel(skillId, skillLevel);
            }

            _specialEffectPacketFactory.SendSpecialEffect(player, DefineSpecialEffects.XI_SYS_EXCHAN01, false);
            _skillPacketFactory.SendSkillTreeUpdate(player);
        }

        /// <inheritdoc />
        public void UseSkill(IPlayerEntity player, SkillInfo skill, uint targetObjectId, SkillUseType skillUseType)
        {
            if (skill == null)
            {
                _skillPacketFactory.SendSkillCancellation(player);
                return;
            }

            if (CanUseSkill(player, skill))
            {
                var target = player.FindEntity<ILivingEntity>(targetObjectId);

                if (target == null)
                {
                    _skillPacketFactory.SendSkillCancellation(player);
                    throw new ArgumentNullException(nameof(targetObjectId));
                }

                switch (skill.Data.ExecuteTarget)
                {
                    case SkillExecuteTargetType.MeleeAttack:
                        CastMeleeSkill(player, target, skill, skillUseType);
                        break;
                    case SkillExecuteTargetType.MagicAttack:
                        CastMagicSkill(player, target, skill, skillUseType);
                        break;
                    case SkillExecuteTargetType.MagicAttackShot:
                        CastMagicAttackShot(player, target, skill, skillUseType);
                        break;
                }
            }
            else
            {
                _logger.LogDebug($"Cannot use {skill.Data.Name}.");
                _skillPacketFactory.SendSkillCancellation(player);
            }
        }

        /// <inheritdoc />
        public bool CanUseSkill(IPlayerEntity player, SkillInfo skill)
        {
            if (skill.Level <= 0 || skill.Level > skill.Data.SkillLevels.Count)
            {
                return false;
            }

            if (!skill.IsCoolTimeElapsed())
            {
                _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_SKILLWAITTIME);
                return false;
            }

            if (skill.LevelData.RequiredMP > 0 && player.Attributes[DefineAttributes.MP] < skill.LevelData.RequiredMP)
            {
                _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REQMP);
                return false;
            }

            if (skill.LevelData.RequiredFP > 0 && player.Attributes[DefineAttributes.FP] < skill.LevelData.RequiredFP)
            {
                _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_REQFP);
                return false;
            }

            InventoryItem rightWeapon = player.Inventory.GetEquipedItem(ItemPartType.RightWeapon);

            if (skill.Data.LinkKind.HasValue)
            {
                bool playerHasCorrectWeapon;
                ItemKind3? rightWeaponKind = rightWeapon?.Data?.ItemKind3;

                switch (skill.Data.LinkKind)
                {
                    case ItemKind3.MAGICBOTH:
                        playerHasCorrectWeapon = !rightWeaponKind.HasValue ||
                            rightWeaponKind != ItemKind3.WAND || rightWeaponKind != ItemKind3.STAFF;
                        break;

                    case ItemKind3.YOBO:
                        playerHasCorrectWeapon = !rightWeaponKind.HasValue ||
                            rightWeaponKind != ItemKind3.YOYO || rightWeaponKind != ItemKind3.BOW;
                        break;

                    case ItemKind3.SHIELD:
                        InventoryItem leftWeapon = player.Inventory.GetEquipedItem(ItemPartType.LeftWeapon);

                        playerHasCorrectWeapon = leftWeapon == null || leftWeapon.Data?.ItemKind3 != ItemKind3.SHIELD;
                        break;

                    default:
                        playerHasCorrectWeapon = skill.Data.LinkKind != rightWeaponKind;
                        break;
                }

                if (!playerHasCorrectWeapon)
                {
                    _textPacketFactory.SendDefinedText(player, DefineText.TID_GAME_WRONGITEM);
                    return false;
                }
            }

            if (skill.Data.Handed.HasValue)
            {
                // TODO: handle dual weapons and two handed weapons
            }

            if (skill.Data.BulletLinkKind.HasValue)
            {
                InventoryItem bulletItem = player.Inventory.GetEquipedItem(ItemPartType.Bullet);

                if (bulletItem.Id == -1 || bulletItem.Data.ItemKind2 != skill.Data.BulletLinkKind)
                {
                    DefineText errorText = skill.Data.LinkKind == ItemKind3.BOW ? DefineText.TID_TIP_NEEDSATTACKITEM : DefineText.TID_TIP_NEEDSKILLITEM;

                    _textPacketFactory.SendDefinedText(player, errorText);
                    return false;
                }
            }

            // TODO: more skill checks

            return true;
        }

        public void Reskill(IPlayerEntity player)
        {
            foreach (SkillInfo skill in player.SkillTree.Skills)
            {
                player.Statistics.SkillPoints += (ushort)(skill.Level * SkillPointUsage[skill.Data.JobType]);
                skill.Level = 0;
            }

            _skillPacketFactory.SendSkillReset(player, player.Statistics.SkillPoints);
        }

        public void AddSkillPoints(IPlayerEntity player, ushort skillPoints)
        {
            if (skillPoints > 0)
            {
                player.Statistics.SkillPoints += skillPoints;
                _playerPacketFactory.SendPlayerExperience(player);
            }
        }

        /// <summary>
        /// Check if the required skill condition matches.
        /// </summary>
        /// <param name="requiredSkillId">Required skill id.</param>
        /// <param name="requiredSkillLevel">Required skill level.</param>
        /// <param name="skillsToUpdate">Dictionary of skills to update.</param>
        /// <returns>True if the requirement matches; false otherwise.</returns>
        private bool CheckRequiredSkill(int requiredSkillId, int requiredSkillLevel, IReadOnlyDictionary<int, int> skillsToUpdate)
        {
            if (requiredSkillId == -1)
            {
                return true;
            }

            if (skillsToUpdate.TryGetValue(requiredSkillId, out int skillLevel))
            {
                return skillLevel >= requiredSkillLevel;
            }

            return false;
        }

        /// <summary>
        /// Gets the skill casting time.
        /// </summary>
        /// <param name="caster">Entity casting the skill.</param>
        /// <param name="skill">Skill to be cast.</param>
        /// <returns>Skill casting time in milliseconds.</returns>
        private int GetSkillCastingTime(ILivingEntity caster, SkillInfo skill)
        {
            if (skill.Data.Type == SkillType.Skill)
            {
                return 1 * 1000;
            }
            else
            {
                int castingTime = (int)((skill.LevelData.CastingTime / 1000f) * (60 / 4));

                castingTime -= castingTime * (caster.Attributes[DefineAttributes.SPELL_RATE] / 100);

                return Math.Max(castingTime, 0);
            }
        }

        /// <summary>
        /// Casts a melee skill on a target.
        /// </summary>
        /// <param name="caster">Living entity casting a skill.</param>
        /// <param name="target">Living target entity touched by the skill.</param>
        /// <param name="skill">Skill to be casted.</param>
        /// <param name="skillUseType">Skill use type.</param>
        private void CastMeleeSkill(ILivingEntity caster, ILivingEntity target, SkillInfo skill, SkillUseType skillUseType)
        {
            int skillCastingTime = GetSkillCastingTime(caster, skill);

            if (skill.Data.SpellRegionType == SpellRegionType.Around)
            {
                // TODO: AoE
                _textPacketFactory.SendFeatureNotImplemented(caster as IPlayerEntity, "AoE skills");
            }
            else
            {
                _skillPacketFactory.SendUseSkill(caster, target, skill, skillCastingTime, skillUseType);

                caster.Delayer.DelayAction(TimeSpan.FromMilliseconds(skill.LevelData.ComboSkillTime), () =>
                {
                    ExecuteSkill(caster, target, skill);
                });
            }
        }

        /// <summary>
        /// Casts an instant magic skill on a living target entity.
        /// </summary>
        /// <param name="caster">Living entity casting a skill.</param>
        /// <param name="target">Living target entity touched by the skill.</param>
        /// <param name="skill">Skill to be casted.</param>
        /// <param name="skillUseType">Skill use type.</param>
        private void CastMagicSkill(ILivingEntity caster, ILivingEntity target, SkillInfo skill, SkillUseType skillUseType)
        {
            int skillCastingTime = GetSkillCastingTime(caster, skill);

            if (skill.Data.SpellRegionType == SpellRegionType.Around)
            {
                // TODO: AoE
                _textPacketFactory.SendFeatureNotImplemented(caster as IPlayerEntity, "AoE skills");
            }
            else
            {
                _skillPacketFactory.SendUseSkill(caster, target, skill, skillCastingTime, skillUseType);

                caster.Delayer.DelayAction(TimeSpan.FromMilliseconds(skill.LevelData.CastingTime), () =>
                {
                    ExecuteSkill(caster, target, skill);
                });
            }
        }

        /// <summary>
        /// Executes a melee or magic skill and inflicts the damages to the target.
        /// </summary>
        /// <param name="caster">Living entity casting a skill.</param>
        /// <param name="target">Living target entity touched by the skill.</param>
        /// <param name="skill">Skill to be casted.</param>
        /// <param name="reduceCasterPoints">Value that indicates if it should reduce caster points or not.</param>
        private void ExecuteSkill(ILivingEntity caster, ILivingEntity target, SkillInfo skill, bool reduceCasterPoints = true)
        {
            ObjectMessageType skillMessageType = ObjectMessageType.OBJMSG_NONE;
            IAttackArbiter attackArbiter = null;

            switch (skill.Data.Type)
            {
                case SkillType.Magic:
                    attackArbiter = new MagicSkillAttackArbiter(caster, target, skill);
                    skillMessageType = ObjectMessageType.OBJMSG_MAGICSKILL;
                    break;

                case SkillType.Skill:
                    attackArbiter = new MeleeSkillAttackArbiter(caster, target, skill);
                    skillMessageType = ObjectMessageType.OBJMSG_MELEESKILL;
                    break;
            }

            _battleSystem.DamageTarget(caster, target, attackArbiter, skillMessageType);

            if (skill.LevelData.CooldownTime > 0)
            {
                skill.SetCoolTime(skill.LevelData.CooldownTime);
            }

            if (reduceCasterPoints)
            {
                ReduceCasterPoints(caster, skill);
            }
        }

        /// <summary>
        /// Casts a magic attack shot projectile on a target.
        /// </summary>
        /// <param name="caster">Living entity casting the skill.</param>
        /// <param name="target">Target living entity.</param>
        /// <param name="skill">Skill to be casted as projectile.</param>
        /// <param name="skillUseType">Skill use type.</param>
        private void CastMagicAttackShot(ILivingEntity caster, ILivingEntity target, SkillInfo skill, SkillUseType skillUseType)
        {
            int skillCastingTime = GetSkillCastingTime(caster, skill);
            var projectile = new MagicSkillProjectileInfo(caster, target, skill, () =>
            {
                ExecuteSkill(caster, target, skill, reduceCasterPoints: false);
            });

            _projectileSystem.CreateProjectile(projectile);
            _skillPacketFactory.SendUseSkill(caster, target, skill, skillCastingTime, skillUseType);

            caster.Delayer.DelayAction(TimeSpan.FromMilliseconds(skill.LevelData.CastingTime), () =>
            {
                ReduceCasterPoints(caster, skill);
            });
        }

        /// <summary>
        /// Reduces caster fatigue points or mana points.
        /// </summary>
        /// <param name="caster">Living entity casting the skill.</param>
        /// <param name="skill">Casted skill by the living entity.</param>
        private void ReduceCasterPoints(ILivingEntity caster, SkillInfo skill)
        {
            if (skill.LevelData.RequiredFP > 0)
            {
                caster.Attributes[DefineAttributes.FP] -= skill.LevelData.RequiredFP;
                _moverPacketFactory.SendUpdateAttributes(caster, DefineAttributes.FP, caster.Attributes[DefineAttributes.FP]);
            }

            if (skill.LevelData.RequiredMP > 0)
            {
                caster.Attributes[DefineAttributes.MP] -= skill.LevelData.RequiredMP;
                _moverPacketFactory.SendUpdateAttributes(caster, DefineAttributes.MP, caster.Attributes[DefineAttributes.MP]);
            }
        }
    }
}
