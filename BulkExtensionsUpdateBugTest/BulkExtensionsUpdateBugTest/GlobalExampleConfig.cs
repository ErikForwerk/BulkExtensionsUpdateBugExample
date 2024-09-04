namespace BulkExtensionsUpdateBugTest;

internal static class GlobalExampleConfig
{
	public static string SERVER { get; }		= "localhost";
	public static uint PORT { get; }			= 3306;
	public static string DATABASE_NAME { get; }	= "BulkUpdateTests";
	public static string USER_NAME { get; }		= "<Your Username Here>";
	public static string PASSWORD { get; }		= "<Your Password Here>";
}
