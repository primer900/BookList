using Android.App;
using Android.Content;
using System.Linq;


namespace BookList
{
	public static class BookUtility
	{

		private const string PREFERENCES_FILE = "StorageFile";
		private const string KEY_FOR_TITLES = "title";
		private const string KEY_FOR_TOTAL_PAGES = "TotalPages";
		private const int PRIVATE_MODE = 0;
		private const char COMMA = ',';

		public const string titleOfItemClicked = "titleOfItemClicked";

		public static void SaveTitleToPreferences(Activity activity, string title)
		{
			var listOfTitles = GetStringFromPreferences(activity, KEY_FOR_TITLES, null);

			if (listOfTitles != null)
				listOfTitles = listOfTitles + COMMA + title;
			else
				listOfTitles = title;

			PutStringInPreferences(activity, KEY_FOR_TITLES, listOfTitles);
		}

		public static void RemoveTitle(Activity activity, string titleToRemove)
		{
			var listOfTitles = GetListOfTitles(activity)
								.Where(title => title != titleToRemove)
								.ToList();

			PutStringInPreferences(activity, KEY_FOR_TITLES, string.Join(COMMA.ToString(), listOfTitles));
		}

		public static string[] GetListOfTitles(Activity activity)
		{
			var listOfTitles = GetStringFromPreferences(activity, KEY_FOR_TITLES, null);
			if (listOfTitles != null)
				return ConvertStringToArray(listOfTitles);

			return null;
		}

		public static void EditTitleInPreferences(Activity activity, string originalTitle, string replacementTitle)
		{
			var listOfTitles = GetStringFromPreferences(activity, KEY_FOR_TITLES, null);

			var listOfTitlesArray = ConvertStringToArray(listOfTitles)
				.Where(title => title != originalTitle)
				.ToList();
			
			listOfTitlesArray.Add(replacementTitle);

			PutStringInPreferences(activity, KEY_FOR_TITLES, string.Join(COMMA.ToString(), listOfTitlesArray));
		}

		public static int GetPageNumberFromPreferences(Activity activity, string key, int defaultValue)
		{
			var preferences = activity.GetSharedPreferences(PREFERENCES_FILE, PRIVATE_MODE);
			var numberOfPages = preferences.GetInt(key, defaultValue);
			return numberOfPages;
		}

		private static void PutStringInPreferences(Activity activity, string key, string listOfTitles)
		{
			ISharedPreferences preferences = activity.GetSharedPreferences(PREFERENCES_FILE, PRIVATE_MODE);
			ISharedPreferencesEditor editor = preferences.Edit();
			editor.PutString(key, listOfTitles);
			editor.Commit();
		}

		private static string GetStringFromPreferences(Activity activity, string key, string defaultValue)
		{
			ISharedPreferences preferences = activity.GetSharedPreferences(PREFERENCES_FILE, PRIVATE_MODE);
			var listOfTitles = preferences.GetString(key, defaultValue);
			return listOfTitles;
		}

		private static string[] ConvertStringToArray(string listOfTitles) => listOfTitles.Split(COMMA);

		public static void PutContentOfBook(Activity activity, string key, int number)
		{
			var preferences = activity.GetSharedPreferences(PREFERENCES_FILE, PRIVATE_MODE);
			var editor = preferences.Edit();
			editor.PutInt(key, number);
			editor.Commit();
		}

		public static void PutBoolInPreferences(Activity activity, string key, bool value)
		{
			var preferences = activity.GetSharedPreferences(PREFERENCES_FILE, PRIVATE_MODE);
			var editor = preferences.Edit();
			editor.PutBoolean(key, value);
			editor.Commit();
		}

		public static bool GetBoolInPreferences(Activity activity, string key, bool defaultValue)
		{
			var preferences = activity.GetSharedPreferences(PREFERENCES_FILE, PRIVATE_MODE);
			var isAudioBook = preferences.GetBoolean(key, defaultValue);
			return isAudioBook;
		}
	}
}
