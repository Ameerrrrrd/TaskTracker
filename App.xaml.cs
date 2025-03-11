namespace TaskTracker;

public partial class App : Application
{

	public static TaskDataBase DataBase { get; private set; }
	public App()
	{
		InitializeComponent();

		string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tasks.db");

		DataBase = new TaskDataBase(dbPath);

		MainPage = new AppShell();
	}
}
