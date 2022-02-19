using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    public partial class Edition_Shop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShippingAddress = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    OrderStatus = table.Column<int>(type: "integer", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaxCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Taxcode = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true, defaultValue: "txcd_00000000"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ads_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ads_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    ValidFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ValidUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToCompanyMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToCompanyMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToCompanyMappings_Companies_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToCompanyMappings_Users_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    TaxCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductGroups_TaxCategories_TaxCategoryId",
                        column: x => x.TaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaxCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Products_TaxCategories_TaxCategoryId",
                        column: x => x.TaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<string>(type: "text", nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stores_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyProductGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProductGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroups_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroups_ProductGroups_ProductGroupId",
                        column: x => x.ProductGroupId,
                        principalTable: "ProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroups_TaxCategories_TaxCategoryId",
                        column: x => x.TaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductGroupToCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGroupToCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductGroupToCategoryMappings_Categories_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductGroupToCategoryMappings_ProductGroups_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "ProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedProductGroupToProductGroupMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedProductGroupToProductGroupMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedProductGroupToProductGroupMappings_ProductGroups_En~1",
                        column: x => x.EntityRightId,
                        principalTable: "ProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedProductGroupToProductGroupMappings_ProductGroups_Ent~",
                        column: x => x.EntityLeftId,
                        principalTable: "ProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaxCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProducts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProducts_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProducts_TaxCategories_TaxCategoryId",
                        column: x => x.TaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductToCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductToCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductToCategoryMappings_Categories_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductToCategoryMappings_Products_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductToProductGroupMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductToProductGroupMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductToProductGroupMappings_ProductGroups_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "ProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductToProductGroupMappings_Products_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedProductGroupToProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedProductGroupToProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedProductGroupToProductMappings_ProductGroups_EntityLe~",
                        column: x => x.EntityLeftId,
                        principalTable: "ProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedProductGroupToProductMappings_Products_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperatingTimes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OpensAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ClosesAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OrdersFrom = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OrdersUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    IsWorkday = table.Column<bool>(type: "boolean", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatingTimes_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToStoreMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToStoreMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToStoreMappings_Stores_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserToStoreMappings_Users_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProductGroupToCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProductGroupToCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroupToCategoryMappings_Categories_EntityRigh~",
                        column: x => x.EntityRightId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroupToCategoryMappings_CompanyProductGroups_~",
                        column: x => x.EntityLeftId,
                        principalTable: "CompanyProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedCompanyProductGroupToCompanyProductGroupMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedCompanyProductGroupToCompanyProductGroupMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedCompanyProductGroupToCompanyProductGroupMappings_Co~1",
                        column: x => x.EntityRightId,
                        principalTable: "CompanyProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedCompanyProductGroupToCompanyProductGroupMappings_Com~",
                        column: x => x.EntityLeftId,
                        principalTable: "CompanyProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyProductGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductGroups_CompanyProductGroups_CompanyProductGroup~",
                        column: x => x.CompanyProductGroupId,
                        principalTable: "CompanyProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProductGroups_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProductGroups_TaxCategories_TaxCategoryId",
                        column: x => x.TaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyProductGroupToCompanyProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProductGroupToCompanyProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroupToCompanyProductMappings_CompanyProductG~",
                        column: x => x.EntityLeftId,
                        principalTable: "CompanyProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroupToCompanyProductMappings_CompanyProducts~",
                        column: x => x.EntityRightId,
                        principalTable: "CompanyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductGroupToCompanyProductMappings_Products_Produc~",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyProductToCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProductToCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProductToCategoryMappings_Categories_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductToCategoryMappings_CompanyProducts_EntityLeft~",
                        column: x => x.EntityLeftId,
                        principalTable: "CompanyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProductToDiscountMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProductToDiscountMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProductToDiscountMappings_CompanyProducts_EntityLeft~",
                        column: x => x.EntityLeftId,
                        principalTable: "CompanyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProductToDiscountMappings_Discounts_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelatedCompanyProductGroupToCompanyProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedCompanyProductGroupToCompanyProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedCompanyProductGroupToCompanyProductMappings_Company~1",
                        column: x => x.EntityRightId,
                        principalTable: "CompanyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedCompanyProductGroupToCompanyProductMappings_CompanyP~",
                        column: x => x.EntityLeftId,
                        principalTable: "CompanyProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<int>(type: "integer", nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    FileId = table.Column<Guid>(type: "uuid", nullable: true),
                    TaxCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CompanyProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProducts_CompanyProducts_CompanyProductId",
                        column: x => x.CompanyProductId,
                        principalTable: "CompanyProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProducts_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreProducts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProducts_TaxCategories_TaxCategoryId",
                        column: x => x.TaxCategoryId,
                        principalTable: "TaxCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelatedStoreProductGroupToStoreProductGroupMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedStoreProductGroupToStoreProductGroupMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedStoreProductGroupToStoreProductGroupMappings_StoreP~1",
                        column: x => x.EntityRightId,
                        principalTable: "StoreProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedStoreProductGroupToStoreProductGroupMappings_StorePr~",
                        column: x => x.EntityLeftId,
                        principalTable: "StoreProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductGroupToCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductGroupToCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductGroupToCategoryMappings_Categories_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProductGroupToCategoryMappings_StoreProductGroups_Enti~",
                        column: x => x.EntityLeftId,
                        principalTable: "StoreProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_StoreProducts_StoreProductId",
                        column: x => x.StoreProductId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalPricePerUnit = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualPricePerUnit = table.Column<decimal>(type: "numeric", nullable: false),
                    TaxPerUnit = table.Column<decimal>(type: "numeric", nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Tax = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Metadata = table.Column<Dictionary<string, string>>(type: "hstore", nullable: true),
                    CompanyProductOverrideId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    StoreProductOverrideId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoreProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_CompanyProducts_CompanyProductId",
                        column: x => x.CompanyProductId,
                        principalTable: "CompanyProducts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_StoreProducts_StoreProductId",
                        column: x => x.StoreProductId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelatedStoreProductGroupToStoreProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedStoreProductGroupToStoreProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatedStoreProductGroupToStoreProductMappings_StoreProduc~1",
                        column: x => x.EntityRightId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RelatedStoreProductGroupToStoreProductMappings_StoreProduct~",
                        column: x => x.EntityLeftId,
                        principalTable: "StoreProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductGroupToStoreProductMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductGroupToStoreProductMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductGroupToStoreProductMappings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoreProductGroupToStoreProductMappings_StoreProductGroups_~",
                        column: x => x.EntityLeftId,
                        principalTable: "StoreProductGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProductGroupToStoreProductMappings_StoreProducts_Entit~",
                        column: x => x.EntityRightId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductToCategoryMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductToCategoryMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductToCategoryMappings_Categories_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProductToCategoryMappings_StoreProducts_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductToDiscountMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EntityLeftId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityRightId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductToDiscountMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductToDiscountMappings_Discounts_EntityRightId",
                        column: x => x.EntityRightId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreProductToDiscountMappings_StoreProducts_EntityLeftId",
                        column: x => x.EntityLeftId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_CompanyId",
                table: "Ads",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_FileId",
                table: "Ads",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_StoreProductId",
                table: "CartItems",
                column: "StoreProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_FileId",
                table: "Companies",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroups_CompanyId",
                table: "CompanyProductGroups",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroups_ProductGroupId",
                table: "CompanyProductGroups",
                column: "ProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroups_TaxCategoryId",
                table: "CompanyProductGroups",
                column: "TaxCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroupToCategoryMappings_EntityLeftId_EntityRi~",
                table: "CompanyProductGroupToCategoryMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroupToCategoryMappings_EntityRightId",
                table: "CompanyProductGroupToCategoryMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroupToCompanyProductMappings_EntityLeftId_En~",
                table: "CompanyProductGroupToCompanyProductMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroupToCompanyProductMappings_EntityRightId",
                table: "CompanyProductGroupToCompanyProductMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductGroupToCompanyProductMappings_ProductId",
                table: "CompanyProductGroupToCompanyProductMappings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProducts_CompanyId",
                table: "CompanyProducts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProducts_FileId",
                table: "CompanyProducts",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProducts_ProductId",
                table: "CompanyProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProducts_TaxCategoryId",
                table: "CompanyProducts",
                column: "TaxCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductToCategoryMappings_EntityLeftId_EntityRightId",
                table: "CompanyProductToCategoryMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductToCategoryMappings_EntityRightId",
                table: "CompanyProductToCategoryMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductToDiscountMappings_EntityLeftId_EntityRightId",
                table: "CompanyProductToDiscountMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProductToDiscountMappings_EntityRightId",
                table: "CompanyProductToDiscountMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CompanyId",
                table: "Discounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingTimes_StoreId",
                table: "OperatingTimes",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CompanyProductId",
                table: "OrderItems",
                column: "CompanyProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_StoreProductId",
                table: "OrderItems",
                column: "StoreProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroups_TaxCategoryId",
                table: "ProductGroups",
                column: "TaxCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroupToCategoryMappings_EntityLeftId_EntityRightId",
                table: "ProductGroupToCategoryMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductGroupToCategoryMappings_EntityRightId",
                table: "ProductGroupToCategoryMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_FileId",
                table: "Products",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TaxCategoryId",
                table: "Products",
                column: "TaxCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToCategoryMappings_EntityLeftId_EntityRightId",
                table: "ProductToCategoryMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductToCategoryMappings_EntityRightId",
                table: "ProductToCategoryMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToProductGroupMappings_EntityLeftId_EntityRightId",
                table: "ProductToProductGroupMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductToProductGroupMappings_EntityRightId",
                table: "ProductToProductGroupMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedCompanyProductGroupToCompanyProductGroupMappings_En~1",
                table: "RelatedCompanyProductGroupToCompanyProductGroupMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedCompanyProductGroupToCompanyProductGroupMappings_Ent~",
                table: "RelatedCompanyProductGroupToCompanyProductGroupMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedCompanyProductGroupToCompanyProductMappings_EntityLe~",
                table: "RelatedCompanyProductGroupToCompanyProductMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedCompanyProductGroupToCompanyProductMappings_EntityRi~",
                table: "RelatedCompanyProductGroupToCompanyProductMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProductGroupToProductGroupMappings_EntityLeftId_Enti~",
                table: "RelatedProductGroupToProductGroupMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProductGroupToProductGroupMappings_EntityRightId",
                table: "RelatedProductGroupToProductGroupMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProductGroupToProductMappings_EntityLeftId_EntityRig~",
                table: "RelatedProductGroupToProductMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedProductGroupToProductMappings_EntityRightId",
                table: "RelatedProductGroupToProductMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedStoreProductGroupToStoreProductGroupMappings_EntityL~",
                table: "RelatedStoreProductGroupToStoreProductGroupMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedStoreProductGroupToStoreProductGroupMappings_EntityR~",
                table: "RelatedStoreProductGroupToStoreProductGroupMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatedStoreProductGroupToStoreProductMappings_EntityLeftId~",
                table: "RelatedStoreProductGroupToStoreProductMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RelatedStoreProductGroupToStoreProductMappings_EntityRightId",
                table: "RelatedStoreProductGroupToStoreProductMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroups_CompanyProductGroupId",
                table: "StoreProductGroups",
                column: "CompanyProductGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroups_StoreId",
                table: "StoreProductGroups",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroups_TaxCategoryId",
                table: "StoreProductGroups",
                column: "TaxCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroupToCategoryMappings_EntityLeftId_EntityRigh~",
                table: "StoreProductGroupToCategoryMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroupToCategoryMappings_EntityRightId",
                table: "StoreProductGroupToCategoryMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroupToStoreProductMappings_EntityLeftId_Entity~",
                table: "StoreProductGroupToStoreProductMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroupToStoreProductMappings_EntityRightId",
                table: "StoreProductGroupToStoreProductMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductGroupToStoreProductMappings_ProductId",
                table: "StoreProductGroupToStoreProductMappings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_CompanyProductId",
                table: "StoreProducts",
                column: "CompanyProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_FileId",
                table: "StoreProducts",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_StoreId",
                table: "StoreProducts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_TaxCategoryId",
                table: "StoreProducts",
                column: "TaxCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductToCategoryMappings_EntityLeftId_EntityRightId",
                table: "StoreProductToCategoryMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductToCategoryMappings_EntityRightId",
                table: "StoreProductToCategoryMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductToDiscountMappings_EntityLeftId_EntityRightId",
                table: "StoreProductToDiscountMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductToDiscountMappings_EntityRightId",
                table: "StoreProductToDiscountMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_FileId",
                table: "Stores",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_SubscriptionId",
                table: "Stores",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CompanyId",
                table: "Subscriptions",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToCompanyMappings_EntityLeftId_EntityRightId",
                table: "UserToCompanyMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserToCompanyMappings_EntityRightId",
                table: "UserToCompanyMappings",
                column: "EntityRightId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToStoreMappings_EntityLeftId_EntityRightId",
                table: "UserToStoreMappings",
                columns: new[] { "EntityLeftId", "EntityRightId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserToStoreMappings_EntityRightId",
                table: "UserToStoreMappings",
                column: "EntityRightId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ads");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CompanyProductGroupToCategoryMappings");

            migrationBuilder.DropTable(
                name: "CompanyProductGroupToCompanyProductMappings");

            migrationBuilder.DropTable(
                name: "CompanyProductToCategoryMappings");

            migrationBuilder.DropTable(
                name: "CompanyProductToDiscountMappings");

            migrationBuilder.DropTable(
                name: "OperatingTimes");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductGroupToCategoryMappings");

            migrationBuilder.DropTable(
                name: "ProductToCategoryMappings");

            migrationBuilder.DropTable(
                name: "ProductToProductGroupMappings");

            migrationBuilder.DropTable(
                name: "RelatedCompanyProductGroupToCompanyProductGroupMappings");

            migrationBuilder.DropTable(
                name: "RelatedCompanyProductGroupToCompanyProductMappings");

            migrationBuilder.DropTable(
                name: "RelatedProductGroupToProductGroupMappings");

            migrationBuilder.DropTable(
                name: "RelatedProductGroupToProductMappings");

            migrationBuilder.DropTable(
                name: "RelatedStoreProductGroupToStoreProductGroupMappings");

            migrationBuilder.DropTable(
                name: "RelatedStoreProductGroupToStoreProductMappings");

            migrationBuilder.DropTable(
                name: "StoreProductGroupToCategoryMappings");

            migrationBuilder.DropTable(
                name: "StoreProductGroupToStoreProductMappings");

            migrationBuilder.DropTable(
                name: "StoreProductToCategoryMappings");

            migrationBuilder.DropTable(
                name: "StoreProductToDiscountMappings");

            migrationBuilder.DropTable(
                name: "UserToCompanyMappings");

            migrationBuilder.DropTable(
                name: "UserToStoreMappings");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "StoreProductGroups");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "StoreProducts");

            migrationBuilder.DropTable(
                name: "CompanyProductGroups");

            migrationBuilder.DropTable(
                name: "CompanyProducts");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "ProductGroups");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "TaxCategories");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
