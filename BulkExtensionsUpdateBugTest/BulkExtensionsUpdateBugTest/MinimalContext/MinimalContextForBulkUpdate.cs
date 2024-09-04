//---------------------------------------------------------------------------------------------------------------------
using System.Text;

using Microsoft.EntityFrameworkCore;

using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace BulkExtensionsUpdateBugTest.MinimalContext;

//---------------------------------------------------------------------------------------------------------------------
internal sealed class MinimalContextForBulkUpdate : DbContext
{
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		StringBuilder connectionString = new StringBuilder()
			.Append("Server=localhost;")
			.Append("Port=3306;")
			.Append("Database=BulkUpdateTests;")
			.Append($"Username={GlobalExampleConfig.USERNAME};")
			.Append($"Password={GlobalExampleConfig.PASSWORD};")
			.Append("Database=BulkUpdateTests;")
			.Append("AllowLoadLocalInfile=true")
			;

		_ = optionsBuilder.UseMySql(
			connectionString.ToString()
			, ServerVersion.Create(Version.Parse("8.0.25"), ServerType.MySql)
			);
	}

	public DbSet<EntityWithMultipleValues> SimpleEntities
	{ get; set; }
}
