using System.Xml;

namespace TaskTracker;

public partial class AddonInfo : ContentPage
{
	public TaskModel task;
	public AddonInfo(TaskModel selectedTask)
	{
		InitializeComponent();
        AddonName.Text = selectedTask.Name;
		AddonDescription.Text = selectedTask.Description;
		AddonDifficulty.Text = selectedTask.Difficulty;
		AddonTerm.Text = selectedTask.Term;
    }

}