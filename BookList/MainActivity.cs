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
		private const int _topOfList = 0;
		private const int _addBookActivityResult = 1;
		private const int _editBookActivityResult = 2;

		private ListView listView;
		private ArrayAdapter<string> adapter;
		private IEnumerable<string> titlesToBeAdded;

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

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

			StartActivityForResult(intent, _editBookActivityResult);
		}

		private void InitializeAddBookButton()
		{
			var addBook = FindViewById<Button>(Resource.Id.addBook);

			addBook.Click += delegate
			{
				var intent = new Intent(this, typeof(AddBookActivity));
				StartActivityForResult(intent, _addBookActivityResult);
			};
		}

		private void UpdateData()
		{
			titlesToBeAdded = BookUtility.GetListOfTitles(this);

			adapter.Clear();
			if (titlesToBeAdded != null)
				titlesToBeAdded
					.ToList()
					.ForEach(title => adapter.Insert(title, _topOfList));
		}
	}
}
