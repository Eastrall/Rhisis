using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rhisis.Database.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Authority = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Angle = table.Column<float>(nullable: false),
                    BankCode = table.Column<int>(nullable: false),
                    ClassId = table.Column<int>(nullable: false),
                    Dexterity = table.Column<int>(nullable: false),
                    Experience = table.Column<long>(nullable: false),
                    FaceId = table.Column<int>(nullable: false),
                    Fp = table.Column<int>(nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    Gold = table.Column<int>(nullable: false),
                    HairColor = table.Column<int>(nullable: false),
                    HairId = table.Column<int>(nullable: false),
                    Hp = table.Column<int>(nullable: false),
                    Intelligence = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    MapId = table.Column<int>(nullable: false),
                    MapLayerId = table.Column<int>(nullable: false),
                    Mp = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    PosX = table.Column<float>(nullable: false),
                    PosY = table.Column<float>(nullable: false),
                    PosZ = table.Column<float>(nullable: false),
                    SkillPoints = table.Column<int>(nullable: false),
                    SkinSetId = table.Column<int>(nullable: false),
                    Slot = table.Column<int>(nullable: false),
                    Stamina = table.Column<int>(nullable: false),
                    StatPoints = table.Column<int>(nullable: false),
                    Strength = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_characters_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CharacterId = table.Column<int>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    Element = table.Column<byte>(nullable: false),
                    ElementRefine = table.Column<byte>(nullable: false),
                    ItemCount = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    ItemSlot = table.Column<int>(nullable: false),
                    Refine = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_items_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sender = table.Column<int>(nullable: false),
                    Receiver = table.Column<int>(nullable: false),
                    Gold = table.Column<int>(nullable: false),
                    Item = table.Column<int>(nullable: false),
                    ItemQuantity = table.Column<short>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    HasBeenRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mails_characters_Sender",
                        column: x => x.Sender,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mails_characters_Receiver",
                        column: x => x.Receiver,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mails_items_Item",
                        column: x => x.Item,
                        principalTable: "items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_UserId",
                table: "characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_items_CharacterId",
                table: "items",
                column: "CharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "mails");
        }
    }
}
