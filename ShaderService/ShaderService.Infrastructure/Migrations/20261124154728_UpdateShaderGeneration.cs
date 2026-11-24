using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShaderService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShaderGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShaderCode",
                table: "ShaderGenerations",
                newName: "VertexShaderCode");

            migrationBuilder.AddColumn<string>(
                name: "FragmentShaderCode",
                table: "ShaderGenerations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeometryType",
                table: "ShaderGenerations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FragmentShaderCode",
                table: "ShaderGenerations");

            migrationBuilder.DropColumn(
                name: "GeometryType",
                table: "ShaderGenerations");

            migrationBuilder.RenameColumn(
                name: "VertexShaderCode",
                table: "ShaderGenerations",
                newName: "ShaderCode");
        }
    }
}
