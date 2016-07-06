using Android.App;
using Android.OS;
using Android.Widget;

namespace BookList
{
	[Activity(Label = "Add Book")]
	public class AddBookActivity : Activity
	{
		private const int PRIVATE_MODE = 0;
		private string _title;
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.AddBook);

			InitializeAddTitleEditText();
			InitializeFinishButton();
		}

		private void InitializeAddTitleEditText()
		{
			var editText = FindViewById<EditText>(Resource.Id.AddTitle);
			editText.TextChanged += (object sender, Android.Text.TextChangedEventArgs e) => _title = e.Text.ToString();
		}

		private void InitializeFinishButton()
		{
			var finishButton = FindViewById<Button>(Resource.Id.Finish);

			finishButton.Click += delegate
			{
				if(_title != null)
					SaveTitle();

				Finish();
			};
		}

		private void SaveTitle() => BookUtility.SaveTitleToPreferences(this, _title);
	}
}
