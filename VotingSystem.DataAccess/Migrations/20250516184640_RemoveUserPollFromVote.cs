using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VotingSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserPollFromVote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_UserPolls_UserPollId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserPollId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserPollId",
                table: "Votes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserPollId",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserPollId",
                table: "Votes",
                column: "UserPollId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_UserPolls_UserPollId",
                table: "Votes",
                column: "UserPollId",
                principalTable: "UserPolls",
                principalColumn: "Id");
        }
    }
}
