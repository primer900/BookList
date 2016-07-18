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
		private int _initialAmountOfContent;
		private int _amountOfContent;
		private const int EDIT_BOOK_ACTIVITY_RESULT = 2;
		private const string CONTENT_TO_ADD_OR_REMOVE = "numberOfPagesToAddOrRemove";
		private const string BOOK_IS_AUDIO = "bookIsAudio";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.BookDetail);

			_title = Intent.GetStringExtra(BookUtility.titleOfItemClicked);
			_initialTitle = _title;
			_amountOfContent = BookUtility.GetPageNumberFromPreferences(this, _title, 0);
			_initialAmountOfContent = _amountOfContent;

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

			if (_amountOfContent != 0)
				numberOfPagesEditText.Text = _amountOfContent.ToString();

			numberOfPagesEditText.TextChanged +=
			(object sender, Android.Text.TextChangedEventArgs e) =>
			{
				_amountOfContent = ConvertStringToInt(e.Text.ToString());
				if(_amountOfContent != _initialAmountOfContent)
					BookUtility.PutContentOfBook(this, _title, _amountOfContent);
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
				if (!TitleChange() && _amountOfContent == _initialAmountOfContent)
				{
					intent.PutExtra(CONTENT_TO_ADD_OR_REMOVE, 0);
					BookUtility.PutBoolInPreferences(this, _initialTitle + "Bool", audioBookCheckBox.Checked);
				}

				if (!TitleChange() && _initialAmountOfContent != _amountOfContent)
				{
					intent.PutExtra(CONTENT_TO_ADD_OR_REMOVE, _amountOfContent - _initialAmountOfContent);
					BookUtility.PutBoolInPreferences(this, _initialTitle + "Bool", audioBookCheckBox.Checked);
				}

				if (TitleChange() && _amountOfContent == _initialAmountOfContent)
				{
					BookUtility.EditTitleInPreferences(this, _initialTitle, _title);
					BookUtility.PutContentOfBook(this, _title, _initialAmountOfContent);
					BookUtility.PutBoolInPreferences(this, _title + "Bool", audioBookCheckBox.Checked);
				}

				if (TitleChange() && _amountOfContent != _initialAmountOfContent)
				{
					BookUtility.EditTitleInPreferences(this, _initialTitle, _title);
					BookUtility.PutContentOfBook(this, _title, _amountOfContent);
					BookUtility.PutBoolInPreferences(this, _title + "Bool", audioBookCheckBox.Checked);
					intent.PutExtra(CONTENT_TO_ADD_OR_REMOVE, _amountOfContent - _initialAmountOfContent);
				}

				intent.PutExtra(BOOK_IS_AUDIO, audioBookCheckBox.Checked);
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
			var audioBookCheckBox = FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox);
			BookUtility.RemoveTitle(this, _title);

			//To set the pages back to 0 when the title is removed.
			BookUtility.PutContentOfBook(this, _title, 0);
			BookUtility.PutContentOfBook(this, _initialTitle, 0);

			var intent = new Intent();

			intent.PutExtra(CONTENT_TO_ADD_OR_REMOVE, 0 - _initialAmountOfContent);
			intent.PutExtra(BOOK_IS_AUDIO, audioBookCheckBox.Checked);

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
			BookUtility.PutContentOfBook(this, _title, _initialAmountOfContent);
		}

		private void SaveTitle() => BookUtility.SaveTitleToPreferences(this, _title);

		private bool TitleChange() => _initialTitle != _title;
	}
}
