using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;

namespace BookList
{
	[Activity(Label = "Details")]
	public class BookDetailActivity : Activity
	{
		private string _title;
		private string _initialTitle;
		private int _initialNumberOfPages;
		private int _numberOfPages;
		private const int EDIT_BOOK_ACTIVITY_RESULT = 2;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.BookDetail);

			_title = Intent.GetStringExtra(BookUtility.titleOfItemClicked);
			_initialTitle = _title;
			_numberOfPages = BookUtility.GetPageNumberFromPreferences(this, _title, 0);
			_initialNumberOfPages = _numberOfPages;

			InitializeEditTitleEditText();
			InitializeNumberOfPagesEditText();
			InitializeDoneEditingButton();
			InitializeDeleteButton();
		}

		private void InitializeEditTitleEditText()
		{
			var editText = FindViewById<EditText>(Resource.Id.EditTitle);
			editText.Text = _title;
			editText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => _title = e.Text.ToString();
		}

		private void InitializeNumberOfPagesEditText()
		{
			var numberOfPagesEditText = FindViewById<EditText>(Resource.Id.NumberOfPages);

			if (_numberOfPages != 0)
				numberOfPagesEditText.Text = _numberOfPages.ToString();

			numberOfPagesEditText.TextChanged +=
			(object sender, Android.Text.TextChangedEventArgs e) =>
			{
				_numberOfPages = ConvertStringToInt(e.Text.ToString());
				if(_numberOfPages != _initialNumberOfPages)
					BookUtility.PutNumberOfPagesInPreferences(this, _title, _numberOfPages);
			};
		}

		private int ConvertStringToInt(string enteredPageCount)
		{
			try
			{
				int numberOfPages;
				Int32.TryParse(enteredPageCount, out numberOfPages);
				return numberOfPages;
			}
			catch (FormatException e)
			{
				throw new FormatException(e.Message);
			}
		}

		private void InitializeDoneEditingButton()
		{
			var doneEditingButton = FindViewById<Button>(Resource.Id.Finish);
			doneEditingButton.Click += delegate 
			{
				if (_title != null && _title != _initialTitle)
					BookUtility.EditTitleInPreferences(this, _initialTitle, _title);

				var intent = new Intent();
				if (_initialNumberOfPages != _numberOfPages)
				{
					intent.PutExtra("numberOfPagesToAdd", _numberOfPages);
					intent.PutExtra("numberOfPagesToRemove", _initialNumberOfPages);
				}
				else
					intent.PutExtra("numberOfPagesToAdd", 0);
				SetResult(Result.Ok, intent);
				Finish();
			};
		}

		private void InitializeDeleteButton()
		{
			var deleteButton = FindViewById<Button>(Resource.Id.Delete);
			deleteButton.Click += delegate 
			{
				var editText = FindViewById<EditText>(Resource.Id.EditTitle);
				editText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => _title = e.Text.ToString();
				BookUtility.RemoveTitle(this, _title);
				Finish();
			};
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			BookUtility.PutNumberOfPagesInPreferences(this, _title, _initialNumberOfPages);
		}

		private void SaveTitle() => BookUtility.SaveTitleToPreferences(this, _title);
	}
}
