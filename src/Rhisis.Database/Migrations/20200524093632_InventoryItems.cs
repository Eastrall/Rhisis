using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rhisis.Database.Migrations
{
    public partial class InventoryItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_characters_users_UserId",
                table: "characters");

            migrationBuilder.DropForeignKey(
                name: "FK_items_characters_CharacterId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_mails_items_ItemId",
                table: "mails");

            migrationBuilder.DropForeignKey(
                name: "FK_mails_characters_ReceiverId",
                table: "mails");

            migrationBuilder.DropForeignKey(
                name: "FK_mails_characters_SenderId",
                table: "mails");

            migrationBuilder.DropForeignKey(
                name: "FK_quests_characters_CharacterId",
                table: "quests");

            migrationBuilder.DropForeignKey(
                name: "FK_shortcuts_characters_CharacterId",
                table: "shortcuts");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_characters_CharacterId",
                table: "Skills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_quests",
                table: "quests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_items",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_CharacterId",
                table: "items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_characters",
                table: "characters");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "quests",
                newName: "Quests");

            migrationBuilder.RenameTable(
                name: "items",
                newName: "Items");

            migrationBuilder.RenameTable(
                name: "characters",
                newName: "Characters");

            migrationBuilder.RenameIndex(
                name: "IX_users_Username_Email",
                table: "Users",
                newName: "IX_Users_Username_Email");

            migrationBuilder.RenameIndex(
                name: "IX_quests_QuestId_CharacterId",
                table: "Quests",
                newName: "IX_Quests_QuestId_CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_quests_CharacterId",
                table: "Quests",
                newName: "IX_Quests_CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_characters_UserId",
                table: "Characters",
                newName: "IX_Characters_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "NVARCHAR(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                type: "VARCHAR(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsDeleted",
                table: "Users",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<ulong>(
                name: "EmailConfirmed",
                table: "Users",
                type: "BIT",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "BIT");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "VARCHAR(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255) CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Authority",
                table: "Users",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Level",
                table: "Skills",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsPatrolDone",
                table: "Quests",
                type: "BIT",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "BIT");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsDeleted",
                table: "Quests",
                type: "BIT",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "BIT");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsChecked",
                table: "Quests",
                type: "BIT",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "BIT");

            migrationBuilder.AlterColumn<ulong>(
                name: "Finished",
                table: "Quests",
                type: "BIT",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(ulong),
                oldType: "BIT");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Refine",
                table: "Items",
                type: "TINYINT",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<sbyte>(
                name: "ElementRefine",
                table: "Items",
                type: "TINYINT",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Element",
                table: "Items",
                type: "TINYINT",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<short>(
                name: "Strength",
                table: "Characters",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "StatPoints",
                table: "Characters",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "Stamina",
                table: "Characters",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<sbyte>(
                name: "SkinSetId",
                table: "Characters",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<short>(
                name: "SkillPoints",
                table: "Characters",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "PlayTime",
                table: "Characters",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "BIGINT");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Characters",
                type: "NVARCHAR(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext CHARACTER SET utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "Characters",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<sbyte>(
                name: "JobId",
                table: "Characters",
                type: "TINYINT",
                nullable: false,
                defaultValue: (sbyte)1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsDeleted",
                table: "Characters",
                type: "BIT",
                nullable: false,
                defaultValue: 0ul,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<short>(
                name: "Intelligence",
                table: "Characters",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<sbyte>(
                name: "HairId",
                table: "Characters",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Gender",
                table: "Characters",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");

            migrationBuilder.AlterColumn<sbyte>(
                name: "FaceId",
                table: "Characters",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "Experience",
                table: "Characters",
                type: "BIGINT",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<short>(
                name: "Dexterity",
                table: "Characters",
                type: "SMALLINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "Angle",
                table: "Characters",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "float");

            migrationBuilder.AddColumn<sbyte>(
                name: "ClusterId",
                table: "Characters",
                type: "TINYINT",
                nullable: false,
                defaultValue: (sbyte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quests",
                table: "Quests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Characters",
                table: "Characters",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "VARCHAR(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.Sql(@"
                DELETE FROM Items WHERE ItemSlot = -1;

                INSERT INTO InventoryItems (CharacterId, ItemId, Quantity, Slot) 
                SELECT CharacterId, Id, ItemCount, ItemSlot FROM Items WHERE IsDeleted = 0;
            ");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ItemCount",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ItemSlot",
                table: "items");

            migrationBuilder.CreateTable(
                name: "ItemAttributes",
                columns: table => new
                {
                    ItemId = table.Column<int>(nullable: false),
                    AttributeId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAttributes", x => new { x.ItemId, x.AttributeId });
                    table.ForeignKey(
                        name: "FK_ItemAttributes_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemAttributes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Attributes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "STR" },
                    { 82, "HEAL" },
                    { 81, "MELEE_STEALHP" },
                    { 80, "PVP_DMG" },
                    { 79, "MONSTER_DMG" },
                    { 78, "SKILL_LEVEL" },
                    { 77, "CRITICAL_BONUS" },
                    { 76, "CAST_CRITICAL_RATE" },
                    { 75, "SPELL_RATE" },
                    { 74, "FP_DEC_RATE" },
                    { 73, "MP_DEC_RATE" },
                    { 72, "ADJDEF_RATE" },
                    { 71, "RECOVERY_EXP" },
                    { 70, "CHR_CHANCEBLEEDING" },
                    { 69, "CHR_CHANCESTEALHP" },
                    { 68, "JUMPING" },
                    { 67, "EXPERIENCE" },
                    { 66, "ATKPOWER_RATE" },
                    { 65, "PARRY" },
                    { 64, "CHRSTATE" },
                    { 63, "CHR_DMG" },
                    { 62, "ADDMAGIC" },
                    { 61, "IMMUNITY" },
                    { 60, "CHR_CHANCEPOISON" },
                    { 83, "ATKPOWER" },
                    { 85, "ONEHANDMASTER_DMG" },
                    { 86, "TWOHANDMASTER_DMG" },
                    { 87, "YOYOMASTER_DMG" },
                    { 10017, "KILL_FP_RATE" },
                    { 10016, "KILL_MP_RATE" },
                    { 10015, "KILL_HP_RATE" },
                    { 10014, "KILL_ALL" },
                    { 10013, "ALL_RECOVERY_RATE" },
                    { 10012, "ALL_RECOVERY" },
                    { 10011, "MASTRY_ALL" },
                    { 10010, "LOCOMOTION" },
                    { 10009, "FP_RECOVERY_RATE" },
                    { 10008, "MP_RECOVERY_RATE" },
                    { 10007, "HP_RECOVERY_RATE" },
                    { 59, "CHR_CHANCEDARK" },
                    { 10006, "CURECHR" },
                    { 10004, "HPDMG_UP" },
                    { 10003, "STAT_ALLUP" },
                    { 10002, "RESIST_ALL" },
                    { 10001, "PXP" },
                    { 10000, "GOLD" },
                    { 93, "MAX_ADJPARAMARY" },
                    { 92, "GIFTBOX" },
                    { 91, "RESIST_MAGIC_RATE" },
                    { 90, "HAWKEYE_RATE" },
                    { 89, "KNUCKLEMASTER_DMG" },
                    { 88, "BOWMASTER_DMG" },
                    { 10005, "DEFHITRATE_DOWN" },
                    { 58, "AUTOHP" },
                    { 57, "CHR_CHANCESTUN" },
                    { 56, "CHR_STEALHP" },
                    { 26, "ADJDEF" },
                    { 25, "SWD_DMG" },
                    { 24, "ATTACKSPEED" },
                    { 22, "PVP_DMG_RATE" },
                    { 21, "KNUCKLE_DMG" },
                    { 20, "MASTRY_WIND" },
                    { 19, "MASTRY_ELECTRICITY" },
                    { 18, "MASTRY_WATER" },
                    { 17, "MASTRY_FIRE" },
                    { 16, "STOP_MOVEMENT" },
                    { 15, "MASTRY_EARTH" },
                    { 27, "RESIST_MAGIC" },
                    { 14, "BLOCK_MELEE" },
                    { 12, "ABILITY_MIN" },
                    { 11, "SPEED" },
                    { 10, "CHR_BLEEDING" },
                    { 9, "CHR_CHANCECRITICAL" },
                    { 8, "BLOCK_RANGE" },
                    { 7, "CHR_RANGE" },
                    { 6, "BOW_DMG" },
                    { 5, "YOY_DMG" },
                    { 4, "STA" },
                    { 3, "INT" },
                    { 2, "DEX" },
                    { 13, "ABILITY_MAX" },
                    { 10018, "KILL_ALL_RATE" },
                    { 28, "RESIST_ELECTRICITY" },
                    { 30, "RESIST_FIRE" },
                    { 55, "CHR_WEAEATKCHANGE" },
                    { 54, "FP_MAX_RATE" },
                    { 53, "MP_MAX_RATE" },
                    { 52, "HP_MAX_RATE" },
                    { 51, "ATTACKSPEED_RATE" },
                    { 50, "CHR_STEALHP_IMM" },
                    { 49, "CLEARBUFF" },
                    { 47, "ADJ_HITRATE" },
                    { 46, "KILL_FP" },
                    { 45, "KILL_MP" },
                    { 44, "KILL_HP" },
                    { 29, "REFLECT_DAMAGE" },
                    { 43, "FP_RECOVERY" },
                    { 41, "HP_RECOVERY" },
                    { 40, "FP" },
                    { 39, "MP" },
                    { 38, "HP" },
                    { 37, "FP_MAX" },
                    { 36, "MP_MAX" },
                    { 35, "HP_MAX" },
                    { 34, "AXE_DMG" },
                    { 33, "RESIST_EARTH" },
                    { 32, "RESIST_WATER" },
                    { 31, "RESIST_WIND" },
                    { 42, "MP_RECOVERY" },
                    { 10019, "ALL_DEC_RATE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemId",
                table: "InventoryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CharacterId_Slot",
                table: "InventoryItems",
                columns: new[] { "CharacterId", "Slot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemAttributes_AttributeId",
                table: "ItemAttributes",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAttributes_ItemId_AttributeId",
                table: "ItemAttributes",
                columns: new[] { "ItemId", "AttributeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Users_UserId",
                table: "Characters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_mails_Items_ItemId",
                table: "mails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mails_Characters_ReceiverId",
                table: "mails",
                column: "ReceiverId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mails_Characters_SenderId",
                table: "mails",
                column: "SenderId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Characters_CharacterId",
                table: "Quests",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_shortcuts_Characters_CharacterId",
                table: "shortcuts",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_Characters_CharacterId",
                table: "Skills",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Users_UserId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_mails_Items_ItemId",
                table: "mails");

            migrationBuilder.DropForeignKey(
                name: "FK_mails_Characters_ReceiverId",
                table: "mails");

            migrationBuilder.DropForeignKey(
                name: "FK_mails_Characters_SenderId",
                table: "mails");

            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Characters_CharacterId",
                table: "Quests");

            migrationBuilder.DropForeignKey(
                name: "FK_shortcuts_Characters_CharacterId",
                table: "shortcuts");

            migrationBuilder.DropForeignKey(
                name: "FK_Skills_Characters_CharacterId",
                table: "Skills");

            migrationBuilder.AddColumn<int>(
                name: "CharacterId",
                table: "items",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemCount",
                table: "items",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemSlot",
                table: "items",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE items i
                SET i.CharacterId = (SELECT CharacterId FROM inventoryitems WHERE ItemId = i.Id LIMIT 1),
	                i.ItemCount = (SELECT Quantity FROM inventoryitems WHERE ItemId = i.Id LIMIT 1),
	                i.ItemSlot = (SELECT Slot FROM inventoryitems WHERE ItemId = i.Id LIMIT 1)
                ;
            ");

            migrationBuilder.AlterColumn<int>(
                name: "ItemCount",
                table: "items",
                type: "int",
                nullable: false);
            migrationBuilder.AlterColumn<int>(
                name: "ItemSlot",
                table: "items",
                type: "int",
                nullable: false);
            migrationBuilder.AlterColumn<int>(
                name: "CharacterId",
                table: "items",
                type: "int",
                nullable: false);

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "ItemAttributes");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quests",
                table: "Quests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Characters",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "ClusterId",
                table: "Characters");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Quests",
                newName: "quests");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "items");

            migrationBuilder.RenameTable(
                name: "Characters",
                newName: "characters");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username_Email",
                table: "users",
                newName: "IX_users_Username_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Quests_QuestId_CharacterId",
                table: "quests",
                newName: "IX_quests_QuestId_CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_Quests_CharacterId",
                table: "quests",
                newName: "IX_quests_CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_UserId",
                table: "characters",
                newName: "IX_characters_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "users",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "users",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "users",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT");

            migrationBuilder.AlterColumn<ulong>(
                name: "EmailConfirmed",
                table: "users",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "users",
                type: "varchar(255) CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<int>(
                name: "Authority",
                table: "users",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<byte>(
                name: "Level",
                table: "Skills",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<ulong>(
                name: "IsPatrolDone",
                table: "quests",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "IsDeleted",
                table: "quests",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "IsChecked",
                table: "quests",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<ulong>(
                name: "Finished",
                table: "quests",
                type: "BIT",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<byte>(
                name: "Refine",
                table: "items",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "ElementRefine",
                table: "items",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "Element",
                table: "items",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Strength",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<int>(
                name: "StatPoints",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<int>(
                name: "Stamina",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<int>(
                name: "SkinSetId",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<int>(
                name: "SkillPoints",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<long>(
                name: "PlayTime",
                table: "characters",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT",
                oldDefaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "characters",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<int>(
                name: "Level",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT",
                oldDefaultValue: (sbyte)1);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "characters",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "BIT",
                oldDefaultValue: 0ul);

            migrationBuilder.AlterColumn<int>(
                name: "Intelligence",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<int>(
                name: "HairId",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<byte>(
                name: "Gender",
                table: "characters",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<int>(
                name: "FaceId",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<long>(
                name: "Experience",
                table: "characters",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT",
                oldDefaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "Dexterity",
                table: "characters",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "SMALLINT");

            migrationBuilder.AlterColumn<float>(
                name: "Angle",
                table: "characters",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldDefaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_quests",
                table: "quests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                table: "items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_characters",
                table: "characters",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_items_CharacterId",
                table: "items",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_characters_users_UserId",
                table: "characters",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_characters_CharacterId",
                table: "items",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mails_items_ItemId",
                table: "mails",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mails_characters_ReceiverId",
                table: "mails",
                column: "ReceiverId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mails_characters_SenderId",
                table: "mails",
                column: "SenderId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_quests_characters_CharacterId",
                table: "quests",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_shortcuts_characters_CharacterId",
                table: "shortcuts",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skills_characters_CharacterId",
                table: "Skills",
                column: "CharacterId",
                principalTable: "characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
