//-----------------------------------------------------------------------------------------------------------------------------------------
namespace BulkExtensionsUpdateBugTest.MinimalContext;

//-----------------------------------------------------------------------------------------------------------------------------------------
public sealed class EntityWithMultipleValues
{
	public EntityWithMultipleValues()
	{ }

	public EntityWithMultipleValues(int intData1, int intData2)
	{
		IntData1 = intData1;
		IntData2 = intData2;
	}

	public int ID { get; set; }

	/// <summary>
	/// Dieses Property soll später aktualisiert werden.
	/// </summary>
	public int IntData1 { get; set; }

	/// <summary>
	/// Dieses Property soll später bei Aktualisierung nicht verändert werden.
	/// </summary>
	public int IntData2 { get; set; }
}
