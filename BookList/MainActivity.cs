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
		private int _numberOfPagesToAddOrRemove;

		private ListView listView;
		private ArrayAdapter<string> adapter;
		private IEnumerable<string> titlesToBeAdded;

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == EDIT_BOOK_ACTIVITY_RESULT && resultCode == Result.Ok)
				_numberOfPagesToAddOrRemove = data.GetIntExtra("numberOfPagesToAddOrRemove", 0);

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

			UpdateTotalPagesRead();
		}

		private void UpdateTotalPagesRead()
		{
			var totalPagesTextView = FindViewById<TextView>(Resource.Id.totalPages);
			var oldTotalPagesRead = BookUtility.GetPageNumberFromPreferences(this, TOTAL_PAGES_MAIN, 0);

			var newTotalPagesRead = oldTotalPagesRead + _numberOfPagesToAddOrRemove;
			BookUtility.PutNumberOfPagesInPreferences(this, TOTAL_PAGES_MAIN, newTotalPagesRead);

			totalPagesTextView.Text = "You have read " + BookUtility.GetPageNumberFromPreferences(this, TOTAL_PAGES_MAIN, 0) + " pages";
			_numberOfPagesToAddOrRemove = 0;
		}
	}
}
