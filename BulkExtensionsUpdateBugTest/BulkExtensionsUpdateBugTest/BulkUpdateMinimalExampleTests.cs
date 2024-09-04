//-----------------------------------------------------------------------------------------------------------------------------------------
using BulkExtensionsUpdateBugTest.MinimalContext;

using EFCore.BulkExtensions;

namespace BulkExtensionsUpdateBugTest;

//-----------------------------------------------------------------------------------------------------------------------------------------
public class BulkUpdateMinimalExampleTests
{
	//-------------------------------------------------------------------------------------------------------------
	#region Fields

	private const int NUM_ENTITIES						= 1000;
	private const int EXPECTED_INT_DATA1				= -8;
	private const int NOT_EXPECTED_INT_DATA2			= -15;
	private static readonly BulkConfig BULK_UPDATE_CFG	= new ()
	{
		PropertiesToIncludeOnUpdate = [nameof(EntityWithMultipleValues.IntData1)],
	};

	#endregion Fields

	//-------------------------------------------------------------------------------------------------------------
	#region Static Helper

	private static void InsertSimpleEntities(MinimalContextForBulkUpdate ctx, int numEntities)
	{
		_ = ctx.Database.EnsureDeleted();
		_ = ctx.Database.EnsureCreated();

		//--- IDs will range from 1 to 1000 since they're one based ---
		for (int i = 0; i < numEntities; i++)
			_ = ctx.SimpleEntities.Add(new EntityWithMultipleValues(i, i));

		_ = ctx.SaveChanges();
	}

	private static void BulkUpdateEntities(MinimalContextForBulkUpdate ctx, Func<int, EntityWithMultipleValues> singleUpdateEntity)
	{
		//--- create N entities to update ---
		List<EntityWithMultipleValues> updateEntities = [];

		for (int i = 0; i < NUM_ENTITIES; i++)
		{
			EntityWithMultipleValues tmp = singleUpdateEntity(i);
			tmp.ID = i + 1; //--- index in DB is 1-based ---

			updateEntities.Add(tmp);
		}

		//-- invoke bulk-update extension ---
		ctx.BulkUpdate(updateEntities, options =>
		{
			options.PropertiesToIncludeOnUpdate = BULK_UPDATE_CFG.PropertiesToIncludeOnUpdate;
		});
	}

	private static void AssertSimpleEntities(MinimalContextForBulkUpdate ctx, int numEntities)
	{
		//--- All entities should now have [IntData1 == EXPECTED_INT_DATA1] ---
		//--- All entities should now have [0 <= IntData2 < NUM_ENTITIES] unchanged ---
		//--- No entity should have [IntData2 == NOT_EXPECTED_INT_DATA2] ---

		EntityWithMultipleValues[] entities = [.. ctx.SimpleEntities.OrderBy(it => it.ID)];
		Assert.Equal(numEntities, entities.Length);

		for (int i = 0; i < numEntities; i++)
		{
			EntityWithMultipleValues entity = entities[i];

			//--- Check the changed value ---
			Assert.Equal(EXPECTED_INT_DATA1, entity.IntData1);

			//--- check the value that is NOT expected here ---
			Assert.NotEqual(NOT_EXPECTED_INT_DATA2, entity.IntData2);

			//--- Check the expected value ---
			Assert.Equal(i, entity.IntData2);
		}
	}

	#endregion Static Helper

	//-----------------------------------------------------------------------------------------------------------------
	#region Test Methods

	/// <summary>
	/// Generally tests that writing and reading back the
	/// entities works and that there is no other error.
	/// </summary>
	[Fact]
	public void NoUpdate_JustWriteAndAssert()
	{
		//--- ARRANGE ---------------------------------------------------------
		//--- ACT -------------------------------------------------------------
		//--- initial commit ---
		using (MinimalContextForBulkUpdate ctxOem = new())
			InsertSimpleEntities(ctxOem, NUM_ENTITIES);

		//--- ASSERT ----------------------------------------------------------
		using (MinimalContextForBulkUpdate ctxAssert = new())
		{
			EntityWithMultipleValues[] entities = [.. ctxAssert.SimpleEntities.OrderBy(it => it.ID)];
			Assert.Equal(NUM_ENTITIES, entities.Length);

			for (int i = 0; i < NUM_ENTITIES; i++)
			{
				EntityWithMultipleValues entity = entities[i];

				Assert.Equal(i, entity.IntData1);
				Assert.Equal(i, entity.IntData2);
			}
		}
	}

	/// <summary>
	/// In this test, both properties are explicitly changed, but the BulkUpdate is only given
	/// <see cref='EntityWithMultipleValues.IntData1'/> for updating.
	/// The other changed property- value should therefore remain unchanged in the DB.	/// </summary>
	/// <param name="databaseType"></param>
	[Fact]
	public void UpdateEntities_ChangedProperty_ExcludeFromUpdate()
	{
		//--- ARRANGE ---------------------------------------------------------
		//--- initial commit ---
		using (MinimalContextForBulkUpdate ctxOem = new())
			InsertSimpleEntities(ctxOem, NUM_ENTITIES);

		//--- ACT -------------------------------------------------------------
		//--- Create "Update entities" and explicitly change [IntData1] and [IntData2] ---
		//--- However, the bulk update is set to only be carried out for [IntData1] ---
		using (MinimalContextForBulkUpdate ctxUpdate = new())
			BulkUpdateEntities(ctxUpdate, i => new EntityWithMultipleValues(EXPECTED_INT_DATA1, NOT_EXPECTED_INT_DATA2));

		//--- ASSERT ----------------------------------------------------------
		using (MinimalContextForBulkUpdate ctxAssert = new())
			AssertSimpleEntities(ctxAssert, NUM_ENTITIES);
	}

	#endregion Test Methods
}
