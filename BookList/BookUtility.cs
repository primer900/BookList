using Android.App;
using Android.Content;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BookList
{
	public static class BookUtility
	{

		private const string preferences_file = "StorageFile";
		private const string keyForTitles = "title";
		private const string keyForTotalPages = "TotalPages";
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
			var listOfTitles = GetListOfTitles(activity)
								.Where(title => title != titleToRemove)
								.ToList();

			PutStringInPreferences(activity, keyForTitles, string.Join(comma.ToString(), listOfTitles));
		}

		public static string[] GetListOfTitles(Activity activity)
		{
			var listOfTitles = GetStringFromPreferences(activity, keyForTitles, null);
			if (listOfTitles != null)
				return ConvertStringToArray(listOfTitles);

			return null;
		}

		public static void EditTitleInPreferences(Activity activity, string originalTitle, string replacementTitle)
		{
			var listOfTitles = GetStringFromPreferences(activity, keyForTitles, null);

			var listOfTitlesArray = ConvertStringToArray(listOfTitles)
				.Where(title => title != originalTitle)
				.ToList();
			
			listOfTitlesArray.Add(replacementTitle);

			PutStringInPreferences(activity, keyForTitles, string.Join(comma.ToString(), listOfTitlesArray));
		}

		public static int GetPageNumberFromPreferences(Activity activity, string key, int defaultValue)
		{
			var preferences = activity.GetSharedPreferences(preferences_file, PRIVATE_MODE);
			var numberOfPages = preferences.GetInt(key, defaultValue);
			return numberOfPages;
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

		public static void PutNumberOfPagesInPreferences(Activity activity, string key, int number)
		{
			var preferences = activity.GetSharedPreferences(preferences_file, PRIVATE_MODE);
			var editor = preferences.Edit();
			editor.PutInt(key, number);
			editor.Commit();
		}
	}
}
