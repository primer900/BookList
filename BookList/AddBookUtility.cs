using System;
using Android.App;
using Android.Content;
using System.Linq;
using System.Collections.Generic;

namespace BookList
{
	public static class AddBookUtility
	{

		private const string preferences_file = "StorageFile";
		private const string keyForTitles = "title";
		private const int PRIVATE_MODE = 0;
		private const char comma = ',';

		public const string titleOfItemClicked = "titleOfItemClicked";

		public static void SaveTitleToPreferences(Activity activity, string title)
		{
			var listOfTitles = GetStringFromPreferences(activity, keyForTitles, null);

			if (listOfTitles != null)
				listOfTitles = listOfTitles + comma + title;
			else
				listOfTitles = title;

			PutStringInPreferences(activity, keyForTitles, listOfTitles);
		}

		public static void RemoveTitle(Activity activity, string titleToRemove)
		{
			var listOfTitles = new List<string>();
			foreach (var title in GetListOfTitles(activity))
				if (title != titleToRemove)
					listOfTitles.Add(title);

			PutStringInPreferences(activity, keyForTitles, string.Join(",", listOfTitles));
		}

		public static string[] GetListOfTitles(Activity activity)
		{
			var listOfTitles = GetStringFromPreferences(activity, keyForTitles, null);
			if (listOfTitles != null)
				return ConvertStringToArray(listOfTitles);

			return null;
		}

		private static void PutStringInPreferences(Activity activity, string key, string listOfTitles)
		{
			ISharedPreferences preferences = activity.GetSharedPreferences(preferences_file, PRIVATE_MODE);
			ISharedPreferencesEditor editor = preferences.Edit();
			editor.PutString(key, listOfTitles);
			editor.Commit();
		}

		private static string GetStringFromPreferences(Activity activity, string key, string defaultValue)
		{
			ISharedPreferences preferences = activity.GetSharedPreferences(preferences_file, PRIVATE_MODE);
			var listOfTitles = preferences.GetString(key, defaultValue);
			return listOfTitles;
		}

		private static string[] ConvertStringToArray(string listOfTitles) => listOfTitles.Split(comma);
	}
}
