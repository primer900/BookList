using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System.Collections.Generic;
using System.Linq;

namespace BookList
{
	[Activity(Label = "BookList", MainLauncher = true, Icon = "@mipmap/book_icon")]
	public class MainActivity : Activity
	{
		private const int TOP_OF_LIST = 0;
		private const int ADD_BOOK_ACTIVITY_RESULT = 1;
		private const int EDIT_BOOK_ACTIVITY_RESULT = 2;
		private const string TOTAL_PAGES_MAIN = "totalPages";
		private const string TOTAL_HOURS_MAIN = "totalHours";
		private int _contentToAddOrRemove;
		private bool _isAudioBook;

		private ListView listView;
		private ArrayAdapter<string> adapter;
		private IEnumerable<string> titlesToBeAdded;

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == EDIT_BOOK_ACTIVITY_RESULT && resultCode == Result.Ok)
			{
				_contentToAddOrRemove = data.GetIntExtra("numberOfPagesToAddOrRemove", 0);
				_isAudioBook = data.GetBooleanExtra("bookIsAudio", false);
			}

			UpdateData();
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Main);

			listView = FindViewById<ListView>(Resource.Id.bookList);
			adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1);
			listView.Adapter = adapter;
			listView.ItemClick += EditBook;

			UpdateData();

			InitializeAddBookButton();
		}

		private void EditBook(object sender, AdapterView.ItemClickEventArgs e)
		{
			var intent = new Intent(this, typeof(BookDetailActivity));
			var titleOfItemClicked = listView.GetItemAtPosition(e.Position).ToString();
			intent.PutExtra(BookUtility.titleOfItemClicked, titleOfItemClicked);

			StartActivityForResult(intent, EDIT_BOOK_ACTIVITY_RESULT);
		}

		private void InitializeAddBookButton()
		{
			var addBook = FindViewById<Button>(Resource.Id.addBook);

			addBook.Click += delegate
			{
				var intent = new Intent(this, typeof(AddBookActivity));
				StartActivityForResult(intent, ADD_BOOK_ACTIVITY_RESULT);
			};
		}

		private void UpdateData()
		{
			titlesToBeAdded = BookUtility.GetListOfTitles(this);

			adapter.Clear();
			if (titlesToBeAdded != null)
				titlesToBeAdded
					.ToList()
					.ForEach(title => adapter.Insert(title, TOP_OF_LIST));

			if(!_isAudioBook)
				UpdateTotalPagesRead();
			else
				UpdateTotalHoursListenedTo();
		}

		private void UpdateTotalPagesRead()
		{
			var totalPagesTextView = FindViewById<TextView>(Resource.Id.totalPages);
			var oldTotalPagesRead = BookUtility.GetPageNumberFromPreferences(this, TOTAL_PAGES_MAIN, 0);

			var newTotalPagesRead = oldTotalPagesRead + _contentToAddOrRemove;
			BookUtility.PutContentOfBook(this, TOTAL_PAGES_MAIN, newTotalPagesRead);

			totalPagesTextView.Text = "You have read " + BookUtility.GetPageNumberFromPreferences(this, TOTAL_PAGES_MAIN, 0) + " pages";
			_contentToAddOrRemove = 0;
		}

		private void UpdateTotalHoursListenedTo()
		{
			var totalHoursTextView = FindViewById<TextView>(Resource.Id.totalHours);
			var oldTotalHoursListened = BookUtility.GetPageNumberFromPreferences(this, TOTAL_HOURS_MAIN, 0);

			var newTotalHoursListened = oldTotalHoursListened + _contentToAddOrRemove;
			BookUtility.PutContentOfBook(this, TOTAL_HOURS_MAIN, newTotalHoursListened);

			totalHoursTextView.Text = "You have listened for " + BookUtility.GetPageNumberFromPreferences(this, TOTAL_HOURS_MAIN, 0) + " hours";
			_contentToAddOrRemove = 0;
		}
	}
}
