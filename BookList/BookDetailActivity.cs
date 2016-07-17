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
		private const string NUMBER_OF_PAGES_TO_ADD = "numberOfPagesToAdd";
		private const string NUMBER_OF_PAGES_TO_REMOVE = "numberOfPagesToRemove";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.BookDetail);

			_title = Intent.GetStringExtra(BookUtility.titleOfItemClicked);
			_initialTitle = _title;
			_numberOfPages = BookUtility.GetPageNumberFromPreferences(this, _title, 0);
			_initialNumberOfPages = _numberOfPages;

			var isAudioBook = BookUtility.GetBoolInPreferences(this, _initialTitle + "Bool", false);
			FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox).Checked = isAudioBook;

			InitializeEditTitleEditText();
			InitializeNumberOfPagesEditText();
			InitializeDoneEditingButton();
			InitializeDeleteButton();
			InitializeAudioBookCheckBox();
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

			if (FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox).Checked)
				numberOfPagesEditText.Hint = "How many hours did you listen?";

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
			doneEditingButton.Click += (sender, e) => DoneEditingButtonClick();
		}
		private void DoneEditingButtonClick()
		{
			var intent = new Intent();
			var audioBookCheckBox = FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox);

			if (_title != null)
			{
				if (!TitleChange() && _numberOfPages == _initialNumberOfPages)
					intent.PutExtra(NUMBER_OF_PAGES_TO_ADD, 0);

				if (!TitleChange() && _initialNumberOfPages != _numberOfPages)
				{
					intent.PutExtra(NUMBER_OF_PAGES_TO_ADD, _numberOfPages);
					intent.PutExtra(NUMBER_OF_PAGES_TO_REMOVE, _initialNumberOfPages);
				}

				if (TitleChange() && _numberOfPages == _initialNumberOfPages)
				{
					BookUtility.EditTitleInPreferences(this, _initialTitle, _title);
					BookUtility.PutNumberOfPagesInPreferences(this, _title, _initialNumberOfPages);
					BookUtility.PutBoolInPreferences(this, _title + "Bool", audioBookCheckBox.Checked);
				}

				if (TitleChange() && _numberOfPages != _initialNumberOfPages)
				{
					BookUtility.EditTitleInPreferences(this, _initialTitle, _title);
					BookUtility.PutNumberOfPagesInPreferences(this, _title, _numberOfPages);
					BookUtility.PutBoolInPreferences(this, _title + "Bool", audioBookCheckBox.Checked);
					intent.PutExtra(NUMBER_OF_PAGES_TO_ADD, _numberOfPages);
					intent.PutExtra(NUMBER_OF_PAGES_TO_REMOVE, _initialNumberOfPages);
				}

				if (audioBookCheckBox.Checked)
				{
					intent.PutExtra(NUMBER_OF_PAGES_TO_ADD, 0);
					intent.PutExtra(NUMBER_OF_PAGES_TO_REMOVE, 0);
				}
			}

			SetResult(Result.Ok, intent);
			Finish();
		}

		private void InitializeDeleteButton()
		{
			var deleteButton = FindViewById<Button>(Resource.Id.Delete);
			deleteButton.Click += (sender, e) => DeleteButtonClick();
		}

		private void DeleteButtonClick()
		{
			var editText = FindViewById<EditText>(Resource.Id.EditTitle);
			editText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => _title = e.Text.ToString();
			BookUtility.RemoveTitle(this, _title);

			//To set the pages back to 0 when the title is removed.
			BookUtility.PutNumberOfPagesInPreferences(this, _title, 0);
			BookUtility.PutNumberOfPagesInPreferences(this, _initialTitle, 0);

			var intent = new Intent();
			intent.PutExtra(NUMBER_OF_PAGES_TO_ADD, 0);

			var audioBookCheckBox = FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox);

			if (!audioBookCheckBox.Checked)
				intent.PutExtra(NUMBER_OF_PAGES_TO_REMOVE, _initialNumberOfPages);
			else
				intent.PutExtra(NUMBER_OF_PAGES_TO_REMOVE, 0);

			SetResult(Result.Ok, intent);
			Finish();
		}

		private void InitializeAudioBookCheckBox()
		{
			var audioBookCheckBox = FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox);
			audioBookCheckBox.Click += delegate 
			{
				var audioBookKey = TitleChange() ? _title + "Bool" : _initialTitle + "Bool";
				if (audioBookCheckBox.Checked)
				{
					FindViewById<EditText>(Resource.Id.NumberOfPages).Hint = "How many hours did you listen?";
					BookUtility.PutBoolInPreferences(this, audioBookKey , true);
				}

				else
				{
					FindViewById<EditText>(Resource.Id.NumberOfPages).Hint = "How many pages did you read?";
					BookUtility.PutBoolInPreferences(this, audioBookKey, false);
				}
			};
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			BookUtility.PutNumberOfPagesInPreferences(this, _title, _initialNumberOfPages);
		}

		private void SaveTitle() => BookUtility.SaveTitleToPreferences(this, _title);

		private bool TitleChange() => _initialTitle != _title;
	}
}
