using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace BookList
{
	[Activity(Label = "Edit Book")]
	public class EditBookActivity : Activity
	{
		private string title;
		private string initialTitle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.EditBook);

			title = Intent.GetStringExtra(AddBookUtility.titleOfItemClicked);
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
					AddBookUtility.EditTitleInPreferences(this, initialTitle, title);

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
				AddBookUtility.RemoveTitle(this, title);
				Finish();
			};
		}

		private void SaveTitle() => AddBookUtility.SaveTitleToPreferences(this, title);
	}
}
