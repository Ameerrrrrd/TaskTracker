using System;
using Microsoft.Maui.Controls;

namespace TaskTracker;

public partial class AddonInfo : ContentPage
{
    public TaskModel task; // ������, ���������� � �����������

    public AddonInfo(TaskModel selectedTask)
    {
        InitializeComponent();
        task = selectedTask;
        // ���������� ������ ������
        AddonName.Text = task.Name;
        AddonDescription.Text = task.Description;
        AddonDifficulty.Text = task.Difficulty;
        AddonTerm.Text = task.Term;
    }

    // ���������� ������� �� ������ ��������������
    private void OnEditChangesClicked(object sender, EventArgs e)
    {
        // ����� ��������� popup ��������� ���� �������������� �������� �������
        NamePlaceHolder.Text = task.Name;
        DescriptionPlaceholder.Text = task.Description;
        // ���� Picker'� ��� �������� ��������, ��������� ������ ������
        difficultyPicker.SelectedIndex = GetPickerIndex(difficultyPicker, task.Difficulty);
        termPicker.SelectedIndex = GetPickerIndex(termPicker, task.Term);

        // ���������� popup
        PopupMenu2.IsVisible = true;
    }

    // ���������� ������� ������ Cancel � popup
    private void OnCancelClicked(object sender, EventArgs e)
    {
        // �������� popup ��� ���������� ���������
        PopupMenu2.IsVisible = false;
    }

    // ���������� ������� ������ ConfirmTask � popup
    private async void OnConfirmTaskClicked(object sender, EventArgs e)
    {
        // ��������� �������� ������ �� ����� popup
        task.Name = NamePlaceHolder.Text;
        task.Description = DescriptionPlaceholder.Text;
        task.Difficulty = difficultyPicker.SelectedItem?.ToString();
        task.Term = termPicker.SelectedItem?.ToString();

        // ��������� ��������� � ���� ������
        await App.DataBase.SaveTaskAsync(task);

        // ��������� ������������ ������ �� �������� �������������� (���� �����)
        AddonName.Text = task.Name;
        AddonDescription.Text = task.Description;
        AddonDifficulty.Text = task.Difficulty;
        AddonTerm.Text = task.Term;

        // ���������� ��������� �� ���������� ������
        MessagingCenter.Send(this, "TaskUpdated", task);

        await DisplayAlert("Success", "Successfully added the task", "Yes");
        // �������� popup � ����� ������� �������� ��������������, ���� ���������
        await Navigation.PopAsync();
        PopupMenu2.IsVisible = false;
    }

    // ��������������� ����� ��� ��������� ������� Picker �� ��������
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
