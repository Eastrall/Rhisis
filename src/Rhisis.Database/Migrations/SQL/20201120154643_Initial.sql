-- Generated

CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);

CREATE TABLE `Attributes` (
    `Id` int NOT NULL,
    `Name` VARCHAR(20) NOT NULL,
    CONSTRAINT `PK_Attributes` PRIMARY KEY (`Id`)
);

CREATE TABLE `Items` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `GameItemId` int NOT NULL,
    `CreatorId` int NOT NULL,
    `Refine` TINYINT NULL,
    `Element` TINYINT NULL,
    `ElementRefine` TINYINT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    CONSTRAINT `PK_Items` PRIMARY KEY (`Id`)
);

CREATE TABLE `ItemStorageTypes` (
    `Id` int NOT NULL,
    `Name` VARCHAR(20) NOT NULL,
    CONSTRAINT `PK_ItemStorageTypes` PRIMARY KEY (`Id`)
);

CREATE TABLE `Users` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Username` NVARCHAR(32) NOT NULL,
    `Password` VARCHAR(32) NOT NULL,
    `Email` VARCHAR(255) NOT NULL,
    `EmailConfirmed` BIT NOT NULL DEFAULT 0,
    `CreatedAt` DATETIME NOT NULL,
    `LastConnectionTime` DATETIME NULL,
    `Authority` TINYINT NOT NULL,
    `IsDeleted` BIT NOT NULL,
    CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
);

CREATE TABLE `ItemAttributes` (
    `ItemId` int NOT NULL,
    `AttributeId` int NOT NULL,
    `Value` int NOT NULL,
    CONSTRAINT `PK_ItemAttributes` PRIMARY KEY (`ItemId`, `AttributeId`),
    CONSTRAINT `FK_ItemAttributes_Attributes_AttributeId` FOREIGN KEY (`AttributeId`) REFERENCES `Attributes` (`Id`),
    CONSTRAINT `FK_ItemAttributes_Items_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `Items` (`Id`)
);

CREATE TABLE `Characters` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Name` NVARCHAR(32) NOT NULL,
    `Gender` TINYINT NOT NULL,
    `Level` int NOT NULL DEFAULT 1,
    `Experience` BIGINT NOT NULL DEFAULT 0,
    `JobId` TINYINT NOT NULL DEFAULT 0,
    `Gold` int NOT NULL,
    `Slot` TINYINT NOT NULL,
    `Strength` SMALLINT NOT NULL,
    `Stamina` SMALLINT NOT NULL,
    `Dexterity` SMALLINT NOT NULL,
    `Intelligence` SMALLINT NOT NULL,
    `Hp` int NOT NULL,
    `Mp` int NOT NULL,
    `Fp` int NOT NULL,
    `SkinSetId` TINYINT NOT NULL,
    `HairId` TINYINT NOT NULL,
    `HairColor` int NOT NULL,
    `FaceId` TINYINT NOT NULL,
    `MapId` int NOT NULL,
    `MapLayerId` int NOT NULL,
    `PosX` float NOT NULL,
    `PosY` float NOT NULL,
    `PosZ` float NOT NULL,
    `Angle` float NOT NULL DEFAULT 0,
    `BankCode` SMALLINT(4) NOT NULL,
    `StatPoints` SMALLINT NOT NULL,
    `SkillPoints` SMALLINT NOT NULL,
    `LastConnectionTime` DATETIME NOT NULL,
    `PlayTime` BIGINT NOT NULL DEFAULT 0,
    `IsDeleted` BIT NOT NULL DEFAULT 0,
    `ClusterId` TINYINT NOT NULL,
    `UserId` int NOT NULL,
    CONSTRAINT `PK_Characters` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Characters_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`)
);

CREATE TABLE `ItemStorage` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `StorageTypeId` int NOT NULL,
    `CharacterId` int NOT NULL,
    `ItemId` int NOT NULL,
    `Slot` SMALLINT NOT NULL,
    `Quantity` SMALLINT NOT NULL,
    `Updated` DATETIME NOT NULL DEFAULT NOW(),
    `IsDeleted` BIT NOT NULL DEFAULT 0,
    CONSTRAINT `PK_ItemStorage` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_ItemStorage_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ItemStorage_Items_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `Items` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ItemStorage_ItemStorage_StorageTypeId` FOREIGN KEY (`StorageTypeId`) REFERENCES `ItemStorage` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Mails` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Text` longtext CHARACTER SET utf8mb4 NULL,
    `Gold` BIGINT NOT NULL,
    `ItemId` int NULL,
    `ItemQuantity` smallint NOT NULL,
    `HasBeenRead` tinyint(1) NOT NULL,
    `HasReceivedItem` tinyint(1) NOT NULL,
    `HasReceivedGold` tinyint(1) NOT NULL,
    `IsDeleted` tinyint(1) NOT NULL,
    `CreateTime` datetime(6) NOT NULL,
    `SenderId` int NOT NULL,
    `ReceiverId` int NOT NULL,
    CONSTRAINT `PK_Mails` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Mails_Items_ItemId` FOREIGN KEY (`ItemId`) REFERENCES `Items` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Mails_Characters_ReceiverId` FOREIGN KEY (`ReceiverId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Mails_Characters_SenderId` FOREIGN KEY (`SenderId`) REFERENCES `Characters` (`Id`) ON DELETE CASCADE
);

CREATE TABLE `Quests` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `QuestId` int NOT NULL,
    `Finished` BIT NOT NULL DEFAULT 0,
    `IsChecked` BIT NOT NULL DEFAULT 0,
    `IsDeleted` BIT NOT NULL DEFAULT 0,
    `StartTime` DATETIME NOT NULL,
    `IsPatrolDone` BIT NOT NULL DEFAULT 0,
    `MonsterKilled1` TINYINT NOT NULL,
    `MonsterKilled2` TINYINT NOT NULL,
    `CharacterId` int NOT NULL,
    CONSTRAINT `PK_Quests` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Quests_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`)
);

CREATE TABLE `SkillBuffs` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CharacterId` int NOT NULL,
    `SkillId` int NOT NULL,
    `SkillLevel` TINYINT NOT NULL,
    `RemainingTime` int NOT NULL,
    CONSTRAINT `PK_SkillBuffs` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_SkillBuffs_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `Skills` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `SkillId` int NOT NULL,
    `Level` TINYINT NOT NULL,
    `CharacterId` int NOT NULL,
    CONSTRAINT `PK_Skills` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Skills_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`)
);

CREATE TABLE `TaskbarShortcuts` (
    `Slot` TINYINT NOT NULL,
    `SlotLevelIndex` SMALLINT NOT NULL,
    `CharacterId` int NOT NULL,
    `TargetTaskbar` TINYINT NOT NULL,
    `Type` TINYINT NOT NULL,
    `ObjectType` TINYINT NOT NULL,
    `ObjectItemSlot` SMALLINT NULL,
    `ObjectIndex` TINYINT NOT NULL,
    `UserId` SMALLINT NOT NULL,
    `ObjectData` SMALLINT NOT NULL,
    `Text` TEXT NOT NULL,
    CONSTRAINT `PK_TaskbarShortcuts` PRIMARY KEY (`CharacterId`, `Slot`, `SlotLevelIndex`),
    CONSTRAINT `FK_TaskbarShortcuts_Characters_CharacterId` FOREIGN KEY (`CharacterId`) REFERENCES `Characters` (`Id`) ON DELETE RESTRICT
);

CREATE TABLE `SkillBuffAttributes` (
    `SkillBuffId` int NOT NULL,
    `AttributeId` int NOT NULL,
    `Value` int NOT NULL,
    CONSTRAINT `PK_SkillBuffAttributes` PRIMARY KEY (`SkillBuffId`, `AttributeId`),
    CONSTRAINT `FK_SkillBuffAttributes_Attributes_AttributeId` FOREIGN KEY (`AttributeId`) REFERENCES `Attributes` (`Id`),
    CONSTRAINT `FK_SkillBuffAttributes_SkillBuffs_SkillBuffId` FOREIGN KEY (`SkillBuffId`) REFERENCES `SkillBuffs` (`Id`)
);

INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (1, 'STR');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (85, 'ONEHANDMASTER_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (83, 'ATKPOWER');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (82, 'HEAL');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (81, 'MELEE_STEALHP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (80, 'PVP_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (79, 'MONSTER_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (78, 'SKILL_LEVEL');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (77, 'CRITICAL_BONUS');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (76, 'CAST_CRITICAL_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (75, 'SPELL_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (74, 'FP_DEC_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (86, 'TWOHANDMASTER_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (73, 'MP_DEC_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (71, 'RECOVERY_EXP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (70, 'CHR_CHANCEBLEEDING');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (69, 'CHR_CHANCESTEALHP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (68, 'JUMPING');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (67, 'EXPERIENCE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (66, 'ATKPOWER_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (65, 'PARRY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (64, 'CHRSTATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (63, 'CHR_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (62, 'ADDMAGIC');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (61, 'IMMUNITY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (72, 'ADJDEF_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (60, 'CHR_CHANCEPOISON');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (87, 'YOYOMASTER_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (89, 'KNUCKLEMASTER_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10019, 'ALL_DEC_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10018, 'KILL_ALL_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10017, 'KILL_FP_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10016, 'KILL_MP_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10015, 'KILL_HP_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10014, 'KILL_ALL');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10013, 'ALL_RECOVERY_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10012, 'ALL_RECOVERY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10011, 'MASTRY_ALL');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10010, 'LOCOMOTION');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10009, 'FP_RECOVERY_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (88, 'BOWMASTER_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10008, 'MP_RECOVERY_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10006, 'CURECHR');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10005, 'DEFHITRATE_DOWN');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10004, 'HPDMG_UP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10003, 'STAT_ALLUP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10002, 'RESIST_ALL');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10001, 'PXP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10000, 'GOLD');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (93, 'MAX_ADJPARAMARY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (92, 'GIFTBOX');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (91, 'RESIST_MAGIC_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (90, 'HAWKEYE_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10007, 'HP_RECOVERY_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (58, 'AUTOHP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (59, 'CHR_CHANCEDARK');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (28, 'RESIST_ELECTRICITY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (27, 'RESIST_MAGIC');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (26, 'ADJDEF');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (25, 'SWD_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (24, 'ATTACKSPEED');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (22, 'PVP_DMG_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (21, 'KNUCKLE_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (20, 'MASTRY_WIND');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (19, 'MASTRY_ELECTRICITY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (18, 'MASTRY_WATER');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (17, 'MASTRY_FIRE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (16, 'STOP_MOVEMENT');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (57, 'CHR_CHANCESTUN');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (15, 'MASTRY_EARTH');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (13, 'ABILITY_MAX');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (12, 'ABILITY_MIN');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (11, 'SPEED');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (10, 'CHR_BLEEDING');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (9, 'CHR_CHANCECRITICAL');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (8, 'BLOCK_RANGE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (7, 'CHR_RANGE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (6, 'BOW_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (5, 'YOY_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (4, 'STA');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (3, 'INT');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (14, 'BLOCK_MELEE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (2, 'DEX');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (29, 'REFLECT_DAMAGE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (31, 'RESIST_WIND');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (56, 'CHR_STEALHP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (55, 'CHR_WEAEATKCHANGE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (54, 'FP_MAX_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (53, 'MP_MAX_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (52, 'HP_MAX_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (51, 'ATTACKSPEED_RATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (50, 'CHR_STEALHP_IMM');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (49, 'CLEARBUFF');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (47, 'ADJ_HITRATE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (46, 'KILL_FP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (45, 'KILL_MP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (30, 'RESIST_FIRE');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (44, 'KILL_HP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (42, 'MP_RECOVERY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (41, 'HP_RECOVERY');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (40, 'FP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (39, 'MP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (38, 'HP');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (37, 'FP_MAX');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (36, 'MP_MAX');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (35, 'HP_MAX');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (34, 'AXE_DMG');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (33, 'RESIST_EARTH');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (32, 'RESIST_WATER');
INSERT INTO `Attributes` (`Id`, `Name`)
VALUES (43, 'FP_RECOVERY');

INSERT INTO `ItemStorageTypes` (`Id`, `Name`)
VALUES (2, 'ExtraBag');
INSERT INTO `ItemStorageTypes` (`Id`, `Name`)
VALUES (3, 'Bank');
INSERT INTO `ItemStorageTypes` (`Id`, `Name`)
VALUES (1, 'Inventory');
INSERT INTO `ItemStorageTypes` (`Id`, `Name`)
VALUES (4, 'GuildBank');

CREATE INDEX `IX_Characters_UserId` ON `Characters` (`UserId`);

CREATE INDEX `IX_ItemAttributes_AttributeId` ON `ItemAttributes` (`AttributeId`);

CREATE UNIQUE INDEX `IX_ItemAttributes_ItemId_AttributeId` ON `ItemAttributes` (`ItemId`, `AttributeId`);

CREATE INDEX `IX_ItemStorage_CharacterId` ON `ItemStorage` (`CharacterId`);

CREATE INDEX `IX_ItemStorage_ItemId` ON `ItemStorage` (`ItemId`);

CREATE INDEX `IX_ItemStorage_StorageTypeId` ON `ItemStorage` (`StorageTypeId`);

CREATE INDEX `IX_Mails_ItemId` ON `Mails` (`ItemId`);

CREATE INDEX `IX_Mails_ReceiverId` ON `Mails` (`ReceiverId`);

CREATE INDEX `IX_Mails_SenderId` ON `Mails` (`SenderId`);

CREATE INDEX `IX_Quests_CharacterId` ON `Quests` (`CharacterId`);

CREATE UNIQUE INDEX `IX_Quests_QuestId_CharacterId` ON `Quests` (`QuestId`, `CharacterId`);

CREATE INDEX `IX_SkillBuffAttributes_AttributeId` ON `SkillBuffAttributes` (`AttributeId`);

CREATE INDEX `IX_SkillBuffAttributes_SkillBuffId_AttributeId` ON `SkillBuffAttributes` (`SkillBuffId`, `AttributeId`);

CREATE UNIQUE INDEX `IX_SkillBuffs_CharacterId_SkillId` ON `SkillBuffs` (`CharacterId`, `SkillId`);

CREATE INDEX `IX_Skills_CharacterId` ON `Skills` (`CharacterId`);

CREATE UNIQUE INDEX `IX_Skills_SkillId_CharacterId` ON `Skills` (`SkillId`, `CharacterId`);

CREATE UNIQUE INDEX `IX_TaskbarShortcuts_CharacterId_Slot_SlotLevelIndex` ON `TaskbarShortcuts` (`CharacterId`, `Slot`, `SlotLevelIndex`);

CREATE UNIQUE INDEX `IX_Users_Username_Email` ON `Users` (`Username`, `Email`);

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20201120154643_Initial', '3.1.3');


