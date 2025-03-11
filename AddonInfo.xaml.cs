using System;
using Microsoft.Maui.Controls;

namespace TaskTracker;

public partial class AddonInfo : ContentPage
{
    public TaskModel task; // Задача, переданная в конструктор

    public AddonInfo(TaskModel selectedTask)
    {
        InitializeComponent();
        task = selectedTask;
        // Отображаем данные задачи
        AddonName.Text = task.Name;
        AddonDescription.Text = task.Description;
        AddonDifficulty.Text = task.Difficulty;
        AddonTerm.Text = task.Term;
    }

    // Обработчик нажатия на кнопку редактирования
    private void OnEditChangesClicked(object sender, EventArgs e)
    {
        // Перед открытием popup заполняем поля редактирования текущими данными
        NamePlaceHolder.Text = task.Name;
        DescriptionPlaceholder.Text = task.Description;
        // Если Picker'ы уже содержат элементы, подбираем нужный индекс
        difficultyPicker.SelectedIndex = GetPickerIndex(difficultyPicker, task.Difficulty);
        termPicker.SelectedIndex = GetPickerIndex(termPicker, task.Term);

        // Отображаем popup
        PopupMenu2.IsVisible = true;
    }

    // Обработчик нажатия кнопки Cancel в popup
    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Скрываем popup без сохранения изменений
        PopupMenu2.IsVisible = false;
    }

    // Обработчик нажатия кнопки ConfirmTask в popup
    private async void OnConfirmTaskClicked(object sender, EventArgs e)
    {
        // Обновляем значения задачи из полей popup
        task.Name = NamePlaceHolder.Text;
        task.Description = DescriptionPlaceholder.Text;
        task.Difficulty = difficultyPicker.SelectedItem?.ToString();
        task.Term = termPicker.SelectedItem?.ToString();

        // Сохраняем изменения в базе данных
        await App.DataBase.SaveTaskAsync(task);

        // Обновляем отображаемые данные на странице редактирования (если нужно)
        AddonName.Text = task.Name;
        AddonDescription.Text = task.Description;
        AddonDifficulty.Text = task.Difficulty;
        AddonTerm.Text = task.Term;

        // Отправляем сообщение об обновлении задачи
        MessagingCenter.Send(this, "TaskUpdated", task);

        await DisplayAlert("Success", "Successfully added the task", "Yes");
        // Скрываем popup и можно закрыть страницу редактирования, если требуется
        await Navigation.PopAsync();
        PopupMenu2.IsVisible = false;
    }

    // Вспомогательный метод для получения индекса Picker по значению
    private int GetPickerIndex(Picker picker, string value)
    {
        for (int i = 0; i < picker.Items.Count; i++)
        {
            if (picker.Items[i] == value)
                return i;
        }
        return -1;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MessagingCenter.Unsubscribe<AddonInfo, TaskModel>(this, "TaskUpdated");
    }
}
