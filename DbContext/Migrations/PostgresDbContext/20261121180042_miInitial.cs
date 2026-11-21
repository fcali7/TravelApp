using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbContext.Migrations.PostgresDbContext
{
    /// <inheritdoc />
    public partial class miInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "supusr");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "supusr",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreetAddress = table.Column<string>(type: "varchar(200)", nullable: false),
                    ZipCode = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "varchar(200)", nullable: false),
                    Country = table.Column<string>(type: "varchar(200)", nullable: false),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "supusr",
                columns: table => new
                {
                    CityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false),
                    Country = table.Column<string>(type: "varchar(200)", nullable: false),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                schema: "supusr",
                columns: table => new
                {
                    QuoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuoteText = table.Column<string>(type: "varchar(200)", nullable: true),
                    Author = table.Column<string>(type: "varchar(200)", nullable: true),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.QuoteId);
                });

            migrationBuilder.CreateTable(
                name: "Travelers",
                schema: "supusr",
                columns: table => new
                {
                    TravelerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(200)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(200)", nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", nullable: true),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travelers", x => x.TravelerId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "varchar(200)", nullable: false),
                    Password = table.Column<string>(type: "varchar(200)", nullable: false),
                    UserRole = table.Column<string>(type: "varchar(200)", nullable: false),
                    Email = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                schema: "supusr",
                columns: table => new
                {
                    FriendId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(200)", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastName = table.Column<string>(type: "varchar(200)", nullable: true),
                    Email = table.Column<string>(type: "varchar(200)", nullable: true),
                    Birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.FriendId);
                    table.ForeignKey(
                        name: "FK_Friends_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "supusr",
                        principalTable: "Addresses",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Attractions",
                schema: "supusr",
                columns: table => new
                {
                    AttractionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "varchar(200)", nullable: false),
                    StreetAddress = table.Column<string>(type: "varchar(200)", nullable: false),
                    CityId = table.Column<Guid>(type: "uuid", nullable: false),
                    strCategory = table.Column<string>(type: "varchar(200)", nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attractions", x => x.AttractionId);
                    table.ForeignKey(
                        name: "FK_Attractions_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "supusr",
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FriendDbMQuoteDbM",
                schema: "supusr",
                columns: table => new
                {
                    FriendsDbMFriendId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuotesDbMQuoteId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendDbMQuoteDbM", x => new { x.FriendsDbMFriendId, x.QuotesDbMQuoteId });
                    table.ForeignKey(
                        name: "FK_FriendDbMQuoteDbM_Friends_FriendsDbMFriendId",
                        column: x => x.FriendsDbMFriendId,
                        principalSchema: "supusr",
                        principalTable: "Friends",
                        principalColumn: "FriendId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendDbMQuoteDbM_Quotes_QuotesDbMQuoteId",
                        column: x => x.QuotesDbMQuoteId,
                        principalSchema: "supusr",
                        principalTable: "Quotes",
                        principalColumn: "QuoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                schema: "supusr",
                columns: table => new
                {
                    PetId = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "varchar(200)", nullable: false),
                    strKind = table.Column<string>(type: "varchar(200)", nullable: true),
                    strMood = table.Column<string>(type: "varchar(200)", nullable: true),
                    Kind = table.Column<int>(type: "integer", nullable: false),
                    Mood = table.Column<int>(type: "integer", nullable: false),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.PetId);
                    table.ForeignKey(
                        name: "FK_Pets_Friends_FriendId",
                        column: x => x.FriendId,
                        principalSchema: "supusr",
                        principalTable: "Friends",
                        principalColumn: "FriendId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "supusr",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "varchar(1000)", nullable: false),
                    AttractionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TravelerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Seeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Attractions_AttractionId",
                        column: x => x.AttractionId,
                        principalSchema: "supusr",
                        principalTable: "Attractions",
                        principalColumn: "AttractionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalSchema: "supusr",
                        principalTable: "Travelers",
                        principalColumn: "TravelerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StreetAddress_ZipCode_City_Country",
                schema: "supusr",
                table: "Addresses",
                columns: new[] { "StreetAddress", "ZipCode", "City", "Country" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_CityId",
                schema: "supusr",
                table: "Attractions",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_strCategory",
                schema: "supusr",
                table: "Attractions",
                column: "strCategory");

            migrationBuilder.CreateIndex(
                name: "IX_Attractions_Title",
                schema: "supusr",
                table: "Attractions",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Country",
                schema: "supusr",
                table: "Cities",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_Country",
                schema: "supusr",
                table: "Cities",
                columns: new[] { "Name", "Country" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AttractionId",
                schema: "supusr",
                table: "Comments",
                column: "AttractionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TravelerId",
                schema: "supusr",
                table: "Comments",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendDbMQuoteDbM_QuotesDbMQuoteId",
                schema: "supusr",
                table: "FriendDbMQuoteDbM",
                column: "QuotesDbMQuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_AddressId",
                schema: "supusr",
                table: "Friends",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_FirstName_LastName",
                schema: "supusr",
                table: "Friends",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_LastName_FirstName",
                schema: "supusr",
                table: "Friends",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Pets_FriendId",
                schema: "supusr",
                table: "Pets",
                column: "FriendId");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_LastName_FirstName",
                schema: "supusr",
                table: "Travelers",
                columns: new[] { "LastName", "FirstName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "FriendDbMQuoteDbM",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Pets",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Attractions",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Travelers",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Quotes",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Friends",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "supusr");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "supusr");
        }
    }
}
