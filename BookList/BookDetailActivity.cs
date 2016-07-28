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
		private const string AUDIO_BOOK_CHECK_ADD_ON = "Bool";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.BookDetail);

			_title = Intent.GetStringExtra(BookUtility.titleOfItemClicked);
			_initialTitle = _title;

			_amountOfContent = BookUtility.GetPageNumberFromPreferences(this, _title, 0);
			_initialAmountOfContent = _amountOfContent;

			var isAudioBook = BookUtility.GetBoolInPreferences(this, _initialTitle + AUDIO_BOOK_CHECK_ADD_ON, false);
			FindViewById<CheckBox>(Resource.Id.AudioBookCheckBox).Checked = isAudioBook;

			InitializeEditTitleEditText();
			InitializeNumberOfPagesEditText();
			InitializeDoneEditingButton();
			InitializeDeleteButton();
			InitializeAudioBookCheckBox();
			InitializeRatingBar();
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
			var rating = FindViewById<RatingBar>(Resource.Id.ratingbar);

			if (_title != null)
			{
				PutValuesInPreferences(audioBookCheckBox, rating);

				intent.PutExtra(CONTENT_TO_ADD_OR_REMOVE, _amountOfContent - _initialAmountOfContent);
				intent.PutExtra(BOOK_IS_AUDIO, audioBookCheckBox.Checked);
			}

			SetResult(Result.Ok, intent);
			Finish();
		}

		private void PutValuesInPreferences(CheckBox audioBookCheckBox, RatingBar rating)
		{
			if (!TitleChange())
			{
				BookUtility.PutBoolInPreferences(this, _initialTitle + AUDIO_BOOK_CHECK_ADD_ON, audioBookCheckBox.Checked);
				BookUtility.PutRatingInPreferences(this, $"RatingFor{_initialTitle}", (int)rating.Rating);
			}

			if (TitleChange())
			{
				BookUtility.EditTitleInPreferences(this, _initialTitle, _title);
				BookUtility.PutBoolInPreferences(this, _title + AUDIO_BOOK_CHECK_ADD_ON, audioBookCheckBox.Checked);
				BookUtility.PutRatingInPreferences(this, $"RatingFor{_title}", (int)rating.Rating);

				if (_amountOfContent == _initialAmountOfContent)
					BookUtility.PutContentOfBook(this, _title, _initialAmountOfContent);

				else if (_amountOfContent != _initialAmountOfContent)
					BookUtility.PutContentOfBook(this, _title, _amountOfContent);
			}
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

			//To set the pages back to 0 when the title is removed.
			BookUtility.PutContentOfBook(this, _initialTitle, 0);
			BookUtility.PutBoolInPreferences(this, _initialTitle + AUDIO_BOOK_CHECK_ADD_ON, false);
			BookUtility.PutRatingInPreferences(this, $"RatingFor{_initialTitle}", 0);
			BookUtility.RemoveTitle(this, _initialTitle);

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
				var audioBookKey = TitleChange() ? _title + AUDIO_BOOK_CHECK_ADD_ON : _initialTitle + AUDIO_BOOK_CHECK_ADD_ON;

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

		private void InitializeRatingBar()
		{
			var ratingBar = FindViewById<RatingBar>(Resource.Id.ratingbar);
			ratingBar.Rating = BookUtility.GetRatingInPreferences(this, $"RatingFor{_initialTitle}", 0);

			ratingBar.RatingBarChange += (o, e) =>
			{
				Toast.MakeText(this, $"New Rating: {ratingBar.Rating}", ToastLength.Short).Show();
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
