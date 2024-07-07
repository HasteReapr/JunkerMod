using BepInEx.Configuration;
using EntityStates;
using JunkerMod.Modules;
using JunkerMod.Modules.Characters;
using JunkerMod.Survivors.Queen.Components;
using JunkerMod.Survivors.Queen.SkillStates;
using JunkerMod.Survivors.Queen.SkillStates.ShotgunSkills;
using JunkerMod.Survivors.Queen.SkillStates.KnifeSkills;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JunkerMod.Survivors.Queen
{
    public class QueenSurvivor : SurvivorBase<QueenSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "queen_assets"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "QueenBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "QueenMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlHenry"; //mdlQueen
        public override string displayPrefabName => "henryDisplay"; // queenDisplay

        public const string QUEEN_PREFIX = JunkerPlugin.DEVELOPER_PREFIX + "_QUEEN_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => QUEEN_PREFIX;
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = QUEEN_PREFIX + "NAME",
            subtitleNameToken = QUEEN_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texQueenIcon"),
            bodyColor = Color.white,
            sortPosition = 100,

            crosshair = Assets.LoadCrosshair("BanditCrosshair"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 150f,
            healthRegen = 1.5f,
            armor = 0f,
            moveSpeed = 6.5f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = assetBundle.LoadMaterial("matHenry"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }
        };

        public override UnlockableDef characterUnlockableDef => QueenUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new QueenItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Queen");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            QueenUnlockables.Init();

            base.InitializeCharacter();

            QueenConfig.Init();
            QueenStates.Init();
            QueenTokens.Init();

            QueenAssets.Init(assetBundle);
            QueenBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<QueenHealComponent>().Hook(); //add the component and then call Hook() instantly, so it activates
        }

        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details
            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordGroup", "SwordHitbox");
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Heal");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Shout");
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        private void AddPassiveSkill()
        {
            GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");
            SkillDef passiveSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "QueenPassive",
                skillNameToken = QUEEN_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = QUEEN_PREFIX + "PASSIVE_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

                activationState = new SerializableEntityStateType(typeof(PassiveHeal.AdrenalineHealState)),
                activationStateMachineName = "Heal",
                baseRechargeInterval = 0f,

                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
            });

            IL.RoR2.DotController.EvaluateDotStacksForType += PassiveHeal.dotDamageHook;

            var healMachine = EntityStateMachine.FindByCustomName(bodyPrefab, "Heal");
            if (healMachine)
            {
                healMachine.SetNextState(new PassiveHeal.AdrenalineHealState());
            }

            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, passiveSkillDef1);
        }

        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            ReloadSkillDef primarySkillDef1 = Skills.CreateSkillDef<ReloadSkillDef>(new SkillDefInfo
                {
                    skillName = "ShootShotgun",
                    skillNameToken = QUEEN_PREFIX + "PRIMARY_GUN_NAME",
                    skillDescriptionToken = QUEEN_PREFIX + "PRIMARY_GUN_DESCRIPTION",
                    skillIcon = assetBundle.LoadAsset<Sprite>("texPrimaryIcon"),
                    
                    activationState = new EntityStates.SerializableEntityStateType(typeof(Reload)),
                    activationStateMachineName = "Weapon",
                    
                    baseMaxStock = 8,
                    requiredStock = 1,
                    rechargeStock = 0,
                    stockToConsume = 1,
                    baseRechargeInterval = 0,

                    beginSkillCooldownOnSkillEnd = false,
                    dontAllowPastMaxStocks = true,
                    forceSprintDuringState = false,
                    fullRestockOnAssign = true,

                    interruptPriority = EntityStates.InterruptPriority.Skill,
                    
                    isCombatSkill = true,
                });

            primarySkillDef1.activationState = new SerializableEntityStateType(typeof(Shoot));
            primarySkillDef1.graceDuration = 0.4f;
            primarySkillDef1.reloadInterruptPriority = InterruptPriority.Any;
            primarySkillDef1.reloadState = new SerializableEntityStateType(typeof(EnterReload));

            Skills.AddPrimarySkills(bodyPrefab, primarySkillDef1);
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            SkillDef secondarySkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryGun",
                skillNameToken = QUEEN_PREFIX + "SECONDARY_KNIFE_NAME",
                skillDescriptionToken = QUEEN_PREFIX + "SECONDARY_KNIFE_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texSecondaryIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Knife)),
                activationStateMachineName = "Weapon2",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 6f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,

            });

            Skills.AddSecondarySkills(bodyPrefab, secondarySkillDef1);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            SkillDef utilitySkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "QueenShout",
                skillNameToken = QUEEN_PREFIX + "UTILITY_SHOUT_NAME",
                skillDescriptionToken = QUEEN_PREFIX + "UTILITY_SHOUT_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texUtilityIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(Shout)),
                activationStateMachineName = "Shout",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 12f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = false,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = false,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });

            Skills.AddUtilitySkills(bodyPrefab, utilitySkillDef1);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);

            SkillDef specialSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "QueenAxe",
                skillNameToken = QUEEN_PREFIX + "SPECIAL_AXE_NAME",
                skillDescriptionToken = QUEEN_PREFIX + "SPECIAL_AXE_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texSpecialIcon"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Axe)),
                //setting this to the "weapon2" EntityStateMachine allows us to cast this skill at the same time primary, which is set to the "weapon" EntityStateMachine
                activationStateMachineName = "Weapon", 
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseMaxStock = 1,
                baseRechargeInterval = 8f,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                mustKeyPress = false,
                canceledFromSprinting = true,
            });

            Skills.AddSpecialSkills(bodyPrefab, specialSkillDef1);
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Skins.CreateSkinDef("DEFAULT_SKIN",
                assetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
                //pass in meshes as they are named in your assetbundle
            //currently not needed as with only 1 skin they will simply take the default meshes
                //uncomment this when you have another skin
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");

            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion

            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            ////creating a new skindef as we did before
            //SkinDef masterySkin = Modules.Skins.CreateSkinDef(QUEEN_PREFIX + "MASTERY_SKIN_NAME",
            //    assetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
            //    defaultRendererinfos,
            //    prefabCharacterModel.gameObject,
            //    HenryUnlockables.masterySkinUnlockableDef);

            ////adding the mesh replacements as above. 
            ////if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            //masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(assetBundle, defaultRendererinfos,
            //    "meshHenrySwordAlt",
            //    null,//no gun mesh replacement. use same gun mesh
            //    "meshHenryAlt");

            ////masterySkin has a new set of RendererInfos (based on default rendererinfos)
            ////you can simply access the RendererInfos' materials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = assetBundle.LoadMaterial("matHenryAlt");

            ////here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            //masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            //{
            //    new SkinDef.GameObjectActivation
            //    {
            //        gameObject = childLocator.FindChildGameObject("GunModel"),
            //        shouldActivate = false,
            //    }
            //};
            ////simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            QueenAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(QueenBuffs.armorBuff))
            {
                args.armorAdd += 300;
            }
        }
    }
}