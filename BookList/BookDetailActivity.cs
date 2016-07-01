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
		private int _numberOfPages;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.BookDetail);

			_title = Intent.GetStringExtra(BookUtility.titleOfItemClicked);
			_initialTitle = _title;

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
			numberOfPagesEditText.TextChanged +=
				(object sender, Android.Text.TextChangedEventArgs e) => 
			{
				var numberOfPagesString = e.Text.ToString();
				_numberOfPages = GetNumberOfPages(numberOfPagesString);
			};
		}

		private int GetNumberOfPages(string enteredPageCount)
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
			var finishButton = FindViewById<Button>(Resource.Id.Finish);
			finishButton.Click += delegate 
			{
				if (_title != null && _title != _initialTitle)
					BookUtility.EditTitleInPreferences(this, _initialTitle, _title);
				

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

		private void SaveTitle() => BookUtility.SaveTitleToPreferences(this, _title);
	}
}
