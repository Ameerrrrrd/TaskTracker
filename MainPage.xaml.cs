using System.Collections.ObjectModel;
using System;
using Microsoft.Maui.Controls;

namespace TaskTracker;

public partial class MainPage : ContentPage
{
    public ObservableCollection<TaskModel> Tasks { get; set; }

    public MainPage()
    {
        InitializeComponent();

        Tasks = new ObservableCollection<TaskModel>();
        TasksCollectionView.ItemsSource = Tasks;
        

        addTaskButton.Clicked += OnAddTaskButtonClicked;
        ConfirmTask.Clicked += OnCloseContextMenu;
        Cancel.Clicked += CancelAddingTask;

        LoadTasksOnStartup();

        MessagingCenter.Subscribe<AddonInfo, TaskModel>(this, "TaskUpdated", (sender, updatedTask) =>
        {
            // Найти задачу в коллекции по Id
            var existingTask = Tasks.FirstOrDefault(t => t.Id == updatedTask.Id);
            if (existingTask != null)
            {
                // Обновляем свойства задачи в коллекции
                existingTask.Name = updatedTask.Name;
                existingTask.Description = updatedTask.Description;
                existingTask.Difficulty = updatedTask.Difficulty;
                existingTask.Term = updatedTask.Term;

                // Если TaskModel не реализует INotifyPropertyChanged, можно пересоздать источник данных:
                // Обновляем ItemsSource для того, чтобы UI отобразил изменения
                TasksCollectionView.ItemsSource = null;
                TasksCollectionView.ItemsSource = Tasks;
            }
        });
    }

    private async void LoadTasksOnStartup()
    {
        var savedTasks = await App.DataBase.GetTasksAsync();
        foreach (var task in savedTasks)
        {
            Tasks.Add(task);
        }
    }

    public void OnAddTaskButtonClicked(object sender, EventArgs e)
    {
        addTaskButton.IsVisible = false;
        PopupMenu.IsVisible = true;
    }

    private async void OnCloseContextMenu(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(NamePlaceHolder.Text))
            await DisplayAlert("Error!", "Required fields are not filled in", "OK");
        else
        {
            var newTask = new TaskModel
            {
                Name = NamePlaceHolder.Text,
                Description = DescriptionPlaceholder.Text,
                Difficulty = difficultyPicker.SelectedItem?.ToString(),
                Term = termPicker.SelectedItem?.ToString()
            };

            await App.DataBase.SaveTaskAsync(newTask);

            TasksCollectionView.ItemsSource = await App.DataBase.GetTasksAsync();

            Tasks.Add(newTask);

            NamePlaceHolder.Text = string.Empty;
            DescriptionPlaceholder.Text = string.Empty;
            difficultyPicker.SelectedIndex = -1;
            termPicker.SelectedIndex = -1;

            addTaskButton.IsVisible = true;
            PopupMenu.IsVisible = false;

            await DisplayAlert("Success", "You added the Task", "OK");
        }
    }

    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItemView swipeItem)
        {
            var task = (TaskModel)swipeItem.BindingContext;

            bool confirmDelete = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this task?", "Yes", "No");
            if (confirmDelete)
            {
                await App.DataBase.DeleteTaskAsync(task);

                TasksCollectionView.ItemsSource = await App.DataBase.GetTasksAsync();
                Tasks.Remove(task);
            }
        }
    }

    private async void OnEditSwipeInvoked(object sender, EventArgs e)
    {
        if (BindingContext is TaskModel selectedTask)
            await Navigation.PushModalAsync(new AddonInfo(selectedTask));
    }

    void CancelAddingTask(object sender, EventArgs e)
    {
        addTaskButton.IsVisible = true;
        PopupMenu.IsVisible = false;
    }

    private async void OnTaskTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is TaskModel selectedTask)
            await Navigation.PushAsync(new AddonInfo(selectedTask));
    }

    protected override bool OnBackButtonPressed()
    {
        // Запускаем асинхронную операцию без блокировки основного потока
        HandleBackButtonPress();
        return true; // Предотвращает стандартное действие "Назад"
    }

    private async void HandleBackButtonPress()
    {
        if (PopupMenu.IsVisible == false)
        {

            if (this is MainPage)
            {
                bool shouldExit = await DisplayAlert("Подтверждение", "Вы уверены, что хотите выйти из приложения?", "Да", "Нет");

                if (shouldExit)
                    Application.Current.Quit();
            }
            else
                await Navigation.PopAsync();
        }
        else
        {
            PopupMenu.IsVisible = false;
            addTaskButton.IsVisible = true;
        }
    }
}
