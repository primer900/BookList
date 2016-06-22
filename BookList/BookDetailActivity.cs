using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace BookList
{
	[Activity(Label = "Details")]
	public class BookDetailActivity : Activity
	{
		private string title;
		private string initialTitle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EditBook);

			title = Intent.GetStringExtra(BookUtility.titleOfItemClicked);
			initialTitle = title;

			InitliazeEditTitleEditText();
			InitializeDoneEditingButton();
			InitliazeDeleteButton();
		}

		private void InitliazeEditTitleEditText()
		{
			var editText = FindViewById<EditText>(Resource.Id.EditTitle);
			editText.Text = title;
			editText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => title = e.Text.ToString();
		}

		private void InitializeDoneEditingButton()
		{
			var finishButton = FindViewById<Button>(Resource.Id.Finish);
			finishButton.Click += delegate 
			{
				if (title != null && title != initialTitle)
					BookUtility.EditTitleInPreferences(this, initialTitle, title);

				Finish();
			};
		}

		private void InitliazeDeleteButton()
		{
			var deleteButton = FindViewById<Button>(Resource.Id.Delete);
			deleteButton.Click += delegate 
			{
				var editText = FindViewById<EditText>(Resource.Id.EditTitle);
				editText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => title = e.Text.ToString();
				BookUtility.RemoveTitle(this, title);
				Finish();
			};
		}

		private void SaveTitle() => BookUtility.SaveTitleToPreferences(this, title);
	}
}
