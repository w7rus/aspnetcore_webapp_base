using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("create extension if not exists hstore");
            
            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("051412c8-c191-4b53-be7c-3090818b47c1"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0815f34f-97d7-4862-83d4-a4351f265a5d"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("090c2124-eef4-4eb1-8ef7-4dc45a06ee3c"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0940be2f-173c-476d-82cd-510d07f6f002"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0fd8aa6c-8b13-43b7-8d34-55c28f5c74c9"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0fe1fc45-7b09-403c-96c5-531059f806db"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("10f5cadb-b5d9-4c90-b09a-5f80737b520c"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("11570cbd-2545-436f-bfec-826bcc5941bb"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("18ca59a8-115d-47d5-9029-fe9059eed27a"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("1988cf6a-7e3a-4ecd-97e0-12ef75161f8b"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("1ac10d52-db5f-4577-809d-738773ddb334"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("218584d5-fd7b-46a9-b90c-c6ac22f66eb7"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("21986af2-bb75-4ce4-8799-12a2621b55d7"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("219fd9f6-0424-4004-af44-8583fbd8364c"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("21a7e09e-4b25-427a-a178-ac7f984e1e74"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("238f17b9-bbb1-4a53-a1fd-6a0ddce2718a"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("27862712-cbe2-4078-9b91-578aa53e7c89"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("289b7951-e119-4f05-a733-0b7fb167810b"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("2a0965b8-111c-47a6-982b-38421b148fe8"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("2c75b400-a7e0-46c7-ad65-411d4419a627"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("307e5fa9-0860-4280-8351-0dd2fd50d750"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("30807702-e7c6-498a-bb84-60ebaab60536"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("35647d94-5113-4514-8601-eab79001d35e"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("36caa2de-cf8f-4256-9eed-43ccd95e9917"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3a401e2f-c092-41ea-a105-d71149fc43a8"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3ab1a512-dc95-4f02-be1b-8d1811270624"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3fce2734-7f11-43f0-8aa0-42727e18517b"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("4df8bab9-a8d1-4484-aafc-80cce374d007"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("4ec7f7d6-9f43-4d48-855d-647477b0c5cc"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("4f847479-5ac8-4a77-bb1b-14f731ffef9b"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("52d2b4df-1361-4598-af65-efc5b3ebf984"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("542e6f70-ba52-449c-9c2d-8b8f3b91a302"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("566e5381-52bc-4897-b5eb-c234becb7525"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5777bc3f-38dd-4aff-8884-be843cb5cf63"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("59c33d82-a184-4457-8e4d-6f8b7adba2b4"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5be97199-4ac7-4478-afde-ae5a60927b1b"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5f349562-19ac-471e-b086-81196b03c6a2"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5f8698b5-50ef-4348-99a4-e64b2149f1fb"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("61786026-802e-4bdf-9864-cd7fa2be2901"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("66bfbe83-6dc1-4be3-8387-f24be04ff2a5"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("69bf2bb1-20dc-4aba-aeeb-896337f19543"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("6f9c61cf-afd6-4695-b404-25e12cc7bc5f"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("6ff670c3-30a5-4186-b785-bacddd4e2cba"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7083d8f1-e83c-4d90-8057-108dfc323ef7"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("71f90ed1-660c-4731-ad9a-d587a6986e4a"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("73e512cc-0939-49a3-ad9a-31954d493eb4"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("75537c46-c27d-4651-a66b-207eb4a5d8c7"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("76c93ef1-ac7f-45bd-a389-0c5402b0dd2a"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("77084c95-3460-472d-922f-97ed1cdd2641"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7bb5c75e-87e9-493c-9f00-562ff410ba29"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7fd067b8-dce3-48b3-9c03-0e490e7e8146"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7ffdcc78-db99-4928-afd4-ab4d924eda9c"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("801bf2d7-ca98-4d29-bc7b-6105e66ce628"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("81511bbd-1ca1-4102-8754-d8f0d766922e"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("83bbc265-3324-4baa-9d3d-63a6313cd695"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("84959e6b-6b84-445c-bda4-f68867ce179f"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("8f95bed0-163e-4965-9ec9-17849e780227"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9302d241-fd5d-484e-a06f-cc8c809fce89"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("93ec425a-7ade-4fa0-b51a-c9707fc6424d"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9447fefa-f9b6-4311-a638-146296aab793"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("95b71cbc-823e-4a76-b036-8b41b74f9142"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("979158ef-f4c6-4752-ad2c-9c935c4bdd49"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("97ddaaee-f3ee-4b51-9e2a-46d0ebdc6b98"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("98ecd932-362b-4edc-8083-3616db7339a6"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9a5741c3-3034-4f71-b1da-91a2f769a909"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9a733949-1f3d-4e77-b74d-22bebbc49d55"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9a9fbeb3-fc31-4089-bf04-fc1c81138ff8"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9d1bd7fb-f86d-461e-9ba8-01a4e3f92cb4"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9d265a5c-a75d-4d7d-8a50-1b702c474c7e"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9d569b58-9678-4586-90b5-ef6e36d5f0fe"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9f789166-eee5-412a-8d20-850806ef1b29"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a024fb6a-eeab-4b0d-adf7-f8fd412af2d3"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a048f842-f8f0-4e4f-81b6-b54e446162a4"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a1f6913e-7657-40a6-bc37-7423baeccaff"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a2e13b1b-bdbd-46ae-a34a-0511cb52ef49"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a4e111aa-8237-486c-b742-a1b989e89af2"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a6515d52-3a4d-4de5-98cc-a35413a0957e"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a6c5c26a-b06e-4a6a-89d0-133c29a9c981"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a8d3b92e-751d-4fc5-b435-2f7cc44ec813"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("ab3c97bf-e7a3-4118-8244-4ce7a4c4c4db"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b2d23951-c9f9-4997-bc4d-d96a42f606eb"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b7672bea-e3bf-4844-ba42-6fb8e8738fbb"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b9292452-29b7-4831-a6a5-5ce7b0c6aeaa"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b9e1efa0-df57-42e9-b603-b75e6b4b0b64"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("bb117641-de17-4489-a19f-fcf351eb08f9"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c09930e4-6234-45c4-87a4-395a4a90a6f1"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c7023877-47d1-4e15-aecb-1e25603d9671"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c7bd5411-2d0c-42d4-bd3c-edbbab6c5695"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c7f8d3f5-a5be-4df7-a8df-8be77dd312c4"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c9030be7-3755-4c25-b9ac-4774abb5c54f"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c9759755-5662-46e3-8615-81758b44cd04"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cb953d83-c9be-4427-96c1-ca9e566d9416"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cca6064e-c943-4347-9660-9bdaaf994256"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cd102ba6-a84c-44e8-94d9-ccc4d0ebd29b"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d0d2d84b-efd4-429a-914e-f5395ee41af8"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d181f458-4118-4ab8-8e71-1b9ffd7ff43c"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d1a35165-9dfc-4239-9cc2-15709f3bf8ec"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d37635c6-b013-493c-ad7e-5d20a50aec1c"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d73489c9-f72e-47c0-ac82-0e4f246a72ab"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("dae1a5de-ac03-4ba4-87bb-04fd6346400c"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e2b7d4f7-a33f-4f81-b9e2-c00c85afb4d8"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e3320704-c972-4d08-99b6-6e8473bb7b1a"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e6591c25-8257-4434-a2c4-acace713d2f9"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e7780ada-4489-44f4-9a38-4d2cd3045f40"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e9eb7fc1-51f5-4336-8311-c8531a570431"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb094c7a-4115-4e05-a4d8-3447efbc794a"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb43016d-ff51-476a-a217-97fc74259acc"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb7faaf0-0539-46e6-bffb-00a7deb845fa"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("ecbb8be2-df4f-4deb-84fe-3ac923ddf474"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("efdf0b13-a1a9-4ccc-b94b-83c313e84113"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f109982b-0aa2-4ca8-861f-ba839aa27e0a"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f4dbb4f6-6dfa-4578-a5de-de87f8e9ce8f"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f4f9f030-bf0f-4959-97bd-49f5b3c10d91"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f9562b2e-baa8-4829-887e-0d961100e61d"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fc09e800-e777-4765-9367-73ab10a3c0e0"),
                column: "EntityId",
                value: new Guid("b26a9112-211b-462f-bd41-8f38a3568106"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fcdef237-7b7d-4457-8da5-e5eb05f4f06f"),
                column: "EntityId",
                value: new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fe4ce055-285f-4c24-a55f-fe4c8d0bd3c2"),
                column: "EntityId",
                value: new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("196ddfa6-4791-48ef-afcd-9cb9183a840b"),
                column: "Type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("505502c4-4055-4267-b631-ff869f14885d"),
                column: "Type",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("64700a31-b2bc-4c6d-bd7e-25e2c62443dc"),
                column: "Type",
                value: 4);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Alias", "CompareMode", "CreatedAt", "Type", "UpdatedAt", "ValueType" },
                values: new object[,]
                {
                    { new Guid("28e18150-c23e-4552-be6e-67492f3d290b"), "g_any_a_read_o_permissionvalue", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9 },
                    { new Guid("2fbc4227-7992-4742-a200-8df76ded3cb5"), "g_any_a_create_o_permissionvalue", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 4, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9 },
                    { new Guid("55825e7b-c355-41b7-a473-90a091237bd7"), "g_any_a_delete_o_permissionvalue", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9 },
                    { new Guid("65269b59-0612-4348-a1e1-cbef6259d9e8"), "g_any_a_create_o_permissionvalue", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9 },
                    { new Guid("aff2f6f8-b2ec-4811-b5dd-15e1b85d7da6"), "g_any_a_update_o_permissionvalue", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 3, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9 },
                    { new Guid("b6c6ebb6-3ef3-4b40-8593-b53603c4097e"), "g_any_a_create_o_permissionvalue", 6, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 2, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), 9 }
                });

            migrationBuilder.InsertData(
                table: "EntityPermissionValueBase<UserGroup>",
                columns: new[] { "Id", "CreatedAt", "Discriminator", "EntityId", "PermissionId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("0acaa313-3705-4ddf-b1fa-3c0537a2527b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("aff2f6f8-b2ec-4811-b5dd-15e1b85d7da6"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("16ed4604-5541-4dc4-8a42-fa90037614a1"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("2fbc4227-7992-4742-a200-8df76ded3cb5"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("1abecdb2-17b6-4bb0-aac9-66b6e8b41d7b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("65269b59-0612-4348-a1e1-cbef6259d9e8"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("36fff9c0-9a3a-4944-9f1a-ac007e1441ba"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("55825e7b-c355-41b7-a473-90a091237bd7"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("3edf5d5b-bd8c-423f-a2e9-3e6eab06bf13"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("aff2f6f8-b2ec-4811-b5dd-15e1b85d7da6"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("47b10195-89c9-4fe9-8d73-123d7eadc8ba"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("65269b59-0612-4348-a1e1-cbef6259d9e8"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("585349da-33b1-4461-9d55-3fb4a21b4ef2"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("28e18150-c23e-4552-be6e-67492f3d290b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("5d9fa67b-2552-4c6f-98fc-509165a659d3"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("aff2f6f8-b2ec-4811-b5dd-15e1b85d7da6"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("5f382a3c-5213-4ef1-a8d1-7ec5342c49e2"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("b6c6ebb6-3ef3-4b40-8593-b53603c4097e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("732ff8ab-f8e6-4b19-963b-7fb74f979b37"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("2fbc4227-7992-4742-a200-8df76ded3cb5"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("77c08d64-9ec1-4f8a-9bc5-ae18804e40bf"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("aff2f6f8-b2ec-4811-b5dd-15e1b85d7da6"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("79b8d4ea-c818-4cbf-8d05-a05c8bb1234f"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("65269b59-0612-4348-a1e1-cbef6259d9e8"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("b33846bc-4927-40ff-8d5a-24c6708f26c8"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("b6c6ebb6-3ef3-4b40-8593-b53603c4097e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("b6ba5d3a-7e62-4f02-a111-24b6b4fb8cb6"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("3a95ab80-ac54-4e23-a35b-aaa6ca726523"), new Guid("28e18150-c23e-4552-be6e-67492f3d290b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("bc02f584-7d55-4956-981e-31c1bef5fb6e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("55825e7b-c355-41b7-a473-90a091237bd7"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("bc782278-9c17-42f0-bb53-1c1d6ab7776c"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("28e18150-c23e-4552-be6e-67492f3d290b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 50, 0, 0, 0, 0, 0, 0, 0 } },
                    { new Guid("cec541bf-c277-4d8a-b479-8b2744453c93"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("65269b59-0612-4348-a1e1-cbef6259d9e8"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("d2c17a7f-4180-4fd6-b3be-5308eb8acf33"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("b26a9112-211b-462f-bd41-8f38a3568106"), new Guid("2fbc4227-7992-4742-a200-8df76ded3cb5"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("d87e9eb3-eaf1-456a-b69e-52b9d264840e"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("28e18150-c23e-4552-be6e-67492f3d290b"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("eb16ed52-48c2-40af-a6dc-f30d94233eed"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("55825e7b-c355-41b7-a473-90a091237bd7"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("ec8f0eea-90bb-401c-bce7-867e9fc8611a"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("93998585-5a67-4a4e-ad2d-f29a4d080e98"), new Guid("55825e7b-c355-41b7-a473-90a091237bd7"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } },
                    { new Guid("fa160e38-dce3-4a25-94d9-b3fba7cdd268"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "UserGroupPermissionValue", new Guid("55119e40-f094-4560-877f-42d18ff197db"), new Guid("2fbc4227-7992-4742-a200-8df76ded3cb5"), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 } }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0acaa313-3705-4ddf-b1fa-3c0537a2527b"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("16ed4604-5541-4dc4-8a42-fa90037614a1"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("1abecdb2-17b6-4bb0-aac9-66b6e8b41d7b"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("36fff9c0-9a3a-4944-9f1a-ac007e1441ba"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3edf5d5b-bd8c-423f-a2e9-3e6eab06bf13"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("47b10195-89c9-4fe9-8d73-123d7eadc8ba"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("585349da-33b1-4461-9d55-3fb4a21b4ef2"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5d9fa67b-2552-4c6f-98fc-509165a659d3"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5f382a3c-5213-4ef1-a8d1-7ec5342c49e2"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("732ff8ab-f8e6-4b19-963b-7fb74f979b37"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("77c08d64-9ec1-4f8a-9bc5-ae18804e40bf"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("79b8d4ea-c818-4cbf-8d05-a05c8bb1234f"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b33846bc-4927-40ff-8d5a-24c6708f26c8"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b6ba5d3a-7e62-4f02-a111-24b6b4fb8cb6"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("bc02f584-7d55-4956-981e-31c1bef5fb6e"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("bc782278-9c17-42f0-bb53-1c1d6ab7776c"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cec541bf-c277-4d8a-b479-8b2744453c93"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d2c17a7f-4180-4fd6-b3be-5308eb8acf33"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d87e9eb3-eaf1-456a-b69e-52b9d264840e"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb16ed52-48c2-40af-a6dc-f30d94233eed"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("ec8f0eea-90bb-401c-bce7-867e9fc8611a"));

            migrationBuilder.DeleteData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fa160e38-dce3-4a25-94d9-b3fba7cdd268"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("28e18150-c23e-4552-be6e-67492f3d290b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2fbc4227-7992-4742-a200-8df76ded3cb5"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("55825e7b-c355-41b7-a473-90a091237bd7"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("65269b59-0612-4348-a1e1-cbef6259d9e8"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("aff2f6f8-b2ec-4811-b5dd-15e1b85d7da6"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("b6c6ebb6-3ef3-4b40-8593-b53603c4097e"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("051412c8-c191-4b53-be7c-3090818b47c1"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0815f34f-97d7-4862-83d4-a4351f265a5d"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("090c2124-eef4-4eb1-8ef7-4dc45a06ee3c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0940be2f-173c-476d-82cd-510d07f6f002"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0fd8aa6c-8b13-43b7-8d34-55c28f5c74c9"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("0fe1fc45-7b09-403c-96c5-531059f806db"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("10f5cadb-b5d9-4c90-b09a-5f80737b520c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("11570cbd-2545-436f-bfec-826bcc5941bb"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("18ca59a8-115d-47d5-9029-fe9059eed27a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("1988cf6a-7e3a-4ecd-97e0-12ef75161f8b"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("1ac10d52-db5f-4577-809d-738773ddb334"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("218584d5-fd7b-46a9-b90c-c6ac22f66eb7"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("21986af2-bb75-4ce4-8799-12a2621b55d7"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("219fd9f6-0424-4004-af44-8583fbd8364c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("21a7e09e-4b25-427a-a178-ac7f984e1e74"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("238f17b9-bbb1-4a53-a1fd-6a0ddce2718a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("27862712-cbe2-4078-9b91-578aa53e7c89"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("289b7951-e119-4f05-a733-0b7fb167810b"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("2a0965b8-111c-47a6-982b-38421b148fe8"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("2c75b400-a7e0-46c7-ad65-411d4419a627"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("307e5fa9-0860-4280-8351-0dd2fd50d750"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("30807702-e7c6-498a-bb84-60ebaab60536"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("35647d94-5113-4514-8601-eab79001d35e"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("36caa2de-cf8f-4256-9eed-43ccd95e9917"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3a401e2f-c092-41ea-a105-d71149fc43a8"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3ab1a512-dc95-4f02-be1b-8d1811270624"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("3fce2734-7f11-43f0-8aa0-42727e18517b"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("4df8bab9-a8d1-4484-aafc-80cce374d007"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("4ec7f7d6-9f43-4d48-855d-647477b0c5cc"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("4f847479-5ac8-4a77-bb1b-14f731ffef9b"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("52d2b4df-1361-4598-af65-efc5b3ebf984"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("542e6f70-ba52-449c-9c2d-8b8f3b91a302"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("566e5381-52bc-4897-b5eb-c234becb7525"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5777bc3f-38dd-4aff-8884-be843cb5cf63"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("59c33d82-a184-4457-8e4d-6f8b7adba2b4"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5be97199-4ac7-4478-afde-ae5a60927b1b"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5f349562-19ac-471e-b086-81196b03c6a2"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("5f8698b5-50ef-4348-99a4-e64b2149f1fb"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("61786026-802e-4bdf-9864-cd7fa2be2901"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("66bfbe83-6dc1-4be3-8387-f24be04ff2a5"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("69bf2bb1-20dc-4aba-aeeb-896337f19543"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("6f9c61cf-afd6-4695-b404-25e12cc7bc5f"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("6ff670c3-30a5-4186-b785-bacddd4e2cba"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7083d8f1-e83c-4d90-8057-108dfc323ef7"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("71f90ed1-660c-4731-ad9a-d587a6986e4a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("73e512cc-0939-49a3-ad9a-31954d493eb4"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("75537c46-c27d-4651-a66b-207eb4a5d8c7"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("76c93ef1-ac7f-45bd-a389-0c5402b0dd2a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("77084c95-3460-472d-922f-97ed1cdd2641"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7bb5c75e-87e9-493c-9f00-562ff410ba29"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7fd067b8-dce3-48b3-9c03-0e490e7e8146"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("7ffdcc78-db99-4928-afd4-ab4d924eda9c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("801bf2d7-ca98-4d29-bc7b-6105e66ce628"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("81511bbd-1ca1-4102-8754-d8f0d766922e"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("83bbc265-3324-4baa-9d3d-63a6313cd695"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("84959e6b-6b84-445c-bda4-f68867ce179f"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("8f95bed0-163e-4965-9ec9-17849e780227"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9302d241-fd5d-484e-a06f-cc8c809fce89"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("93ec425a-7ade-4fa0-b51a-c9707fc6424d"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9447fefa-f9b6-4311-a638-146296aab793"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("95b71cbc-823e-4a76-b036-8b41b74f9142"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("979158ef-f4c6-4752-ad2c-9c935c4bdd49"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("97ddaaee-f3ee-4b51-9e2a-46d0ebdc6b98"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("98ecd932-362b-4edc-8083-3616db7339a6"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9a5741c3-3034-4f71-b1da-91a2f769a909"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9a733949-1f3d-4e77-b74d-22bebbc49d55"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9a9fbeb3-fc31-4089-bf04-fc1c81138ff8"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9d1bd7fb-f86d-461e-9ba8-01a4e3f92cb4"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9d265a5c-a75d-4d7d-8a50-1b702c474c7e"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9d569b58-9678-4586-90b5-ef6e36d5f0fe"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("9f789166-eee5-412a-8d20-850806ef1b29"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a024fb6a-eeab-4b0d-adf7-f8fd412af2d3"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a048f842-f8f0-4e4f-81b6-b54e446162a4"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a1f6913e-7657-40a6-bc37-7423baeccaff"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a2e13b1b-bdbd-46ae-a34a-0511cb52ef49"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a4e111aa-8237-486c-b742-a1b989e89af2"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a6515d52-3a4d-4de5-98cc-a35413a0957e"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a6c5c26a-b06e-4a6a-89d0-133c29a9c981"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("a8d3b92e-751d-4fc5-b435-2f7cc44ec813"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("ab3c97bf-e7a3-4118-8244-4ce7a4c4c4db"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b2d23951-c9f9-4997-bc4d-d96a42f606eb"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b7672bea-e3bf-4844-ba42-6fb8e8738fbb"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b9292452-29b7-4831-a6a5-5ce7b0c6aeaa"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("b9e1efa0-df57-42e9-b603-b75e6b4b0b64"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("bb117641-de17-4489-a19f-fcf351eb08f9"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c09930e4-6234-45c4-87a4-395a4a90a6f1"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c7023877-47d1-4e15-aecb-1e25603d9671"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c7bd5411-2d0c-42d4-bd3c-edbbab6c5695"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c7f8d3f5-a5be-4df7-a8df-8be77dd312c4"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c9030be7-3755-4c25-b9ac-4774abb5c54f"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("c9759755-5662-46e3-8615-81758b44cd04"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cb953d83-c9be-4427-96c1-ca9e566d9416"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cca6064e-c943-4347-9660-9bdaaf994256"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("cd102ba6-a84c-44e8-94d9-ccc4d0ebd29b"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d0d2d84b-efd4-429a-914e-f5395ee41af8"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d181f458-4118-4ab8-8e71-1b9ffd7ff43c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d1a35165-9dfc-4239-9cc2-15709f3bf8ec"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d37635c6-b013-493c-ad7e-5d20a50aec1c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("d73489c9-f72e-47c0-ac82-0e4f246a72ab"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("dae1a5de-ac03-4ba4-87bb-04fd6346400c"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e2b7d4f7-a33f-4f81-b9e2-c00c85afb4d8"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e3320704-c972-4d08-99b6-6e8473bb7b1a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e6591c25-8257-4434-a2c4-acace713d2f9"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e7780ada-4489-44f4-9a38-4d2cd3045f40"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("e9eb7fc1-51f5-4336-8311-c8531a570431"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb094c7a-4115-4e05-a4d8-3447efbc794a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb43016d-ff51-476a-a217-97fc74259acc"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("eb7faaf0-0539-46e6-bffb-00a7deb845fa"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("ecbb8be2-df4f-4deb-84fe-3ac923ddf474"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("efdf0b13-a1a9-4ccc-b94b-83c313e84113"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f109982b-0aa2-4ca8-861f-ba839aa27e0a"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f4dbb4f6-6dfa-4578-a5de-de87f8e9ce8f"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f4f9f030-bf0f-4959-97bd-49f5b3c10d91"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("f9562b2e-baa8-4829-887e-0d961100e61d"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fc09e800-e777-4765-9367-73ab10a3c0e0"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fcdef237-7b7d-4457-8da5-e5eb05f4f06f"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "EntityPermissionValueBase<UserGroup>",
                keyColumn: "Id",
                keyValue: new Guid("fe4ce055-285f-4c24-a55f-fe4c8d0bd3c2"),
                column: "EntityId",
                value: new Guid("55119e40-f094-4560-877f-42d18ff197db"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("196ddfa6-4791-48ef-afcd-9cb9183a840b"),
                column: "Type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("505502c4-4055-4267-b631-ff869f14885d"),
                column: "Type",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("64700a31-b2bc-4c6d-bd7e-25e2c62443dc"),
                column: "Type",
                value: 5);
        }
    }
}
