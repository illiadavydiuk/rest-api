using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tracks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    isrc = table.Column<string>(type: "varchar(12)", nullable: false),
                    title = table.Column<string>(type: "varchar(200)", nullable: false),
                    artist = table.Column<string>(type: "varchar(200)", nullable: false),
                    duration_seconds = table.Column<int>(type: "integer", nullable: false),
                    genre = table.Column<string>(type: "varchar(50)", nullable: false),
                    is_explicit = table.Column<bool>(type: "boolean", nullable: false),
                    release_date = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "timezone('utc', now())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tracks", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tracks_isrc",
                table: "tracks",
                column: "isrc",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracks");
        }
    }
}
