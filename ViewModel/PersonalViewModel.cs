﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Module07Data_Access.Model;
using Module07Data_Access.Services;

namespace Module07Data_Access.ViewModel
{
    public class PersonalViewModel : INotifyPropertyChanged
    {
        private readonly PersonalService _personalService;
        public ObservableCollection<Personal> PersonalList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private Personal _selectedPersonal;
        public Personal SelectedPersonal
        {
            get => _selectedPersonal;
            set
            {
                _selectedPersonal = value;
                if (_selectedPersonal != null)
                {
                    NewPersonalName = _selectedPersonal.Name;
                    NewPersonalGender = _selectedPersonal.Gender;
                    NewPersonalContactNo = _selectedPersonal.ContactNo;
                    IsPersonSelected = true;
                }
                else
                {
                    IsPersonSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _isPersonSelected;
        public bool IsPersonSelected
        {
            get => _isPersonSelected;
            set
            {
                _isPersonSelected = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalName;
        public string NewPersonalName
        {
            get => _newPersonalName;
            set
            {
                _newPersonalName = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalGender;
        public string NewPersonalGender
        {
            get => _newPersonalGender;
            set
            {
                _newPersonalGender = value;
                OnPropertyChanged();
            }
        }

        private string _newPersonalContactNo;
        public string NewPersonalContactNo
        {
            get => _newPersonalContactNo;
            set
            {
                _newPersonalContactNo = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddPersonalCommand { get; }
        public ICommand DeletePersonCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ShowAllCommand { get; }

        public PersonalViewModel()
        {
            _personalService = new PersonalService();
            PersonalList = new ObservableCollection<Personal>();
            LoadDataCommand = new Command(async () => await LoadData());
            AddPersonalCommand = new Command(async () => await AddPerson());
            DeletePersonCommand = new Command(async () => await DeletePersonal(), () => SelectedPersonal != null);
            SearchCommand = new Command(async () => await SearchPersonal());
            UpdateCommand = new Command(async () => await UpdatePersonal(), () => SelectedPersonal != null);
            ShowAllCommand = new Command(async () => await LoadData());
            LoadData();
        }

        public async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading personal data...";
            try
            {
                var personals = await _personalService.GetAllPersonalsAsync();
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "Data loaded successfully!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to load data: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddPerson()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewPersonalName) || string.IsNullOrWhiteSpace(NewPersonalGender) || string.IsNullOrWhiteSpace(NewPersonalContactNo))
            {
                StatusMessage = "Please fill in all fields before adding.";
                return;
            }

            IsBusy = true;

            try
            {
                var newPerson = new Personal
                {
                    Name = NewPersonalName,
                    Gender = NewPersonalGender,
                    ContactNo = NewPersonalContactNo
                };

                var isSuccess = await _personalService.AddPersonalAsync(newPerson);
                if (isSuccess)
                {
                    NewPersonalName = string.Empty;
                    NewPersonalGender = string.Empty;
                    NewPersonalContactNo = string.Empty;
                    StatusMessage = "New person added successfully!";
                    await LoadData();
                }
                else
                {
                    StatusMessage = "Failed to add person.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error adding person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeletePersonal()
        {
            if (SelectedPersonal == null) return;

            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {SelectedPersonal.Name}?", "Yes", "No");
            if (!answer) return;

            IsBusy = true;

            try
            {
                var success = await _personalService.DeletePersonalAsync(SelectedPersonal.ID);
                if (success)
                {
                    PersonalList.Remove(SelectedPersonal);
                    SelectedPersonal = null;
                    StatusMessage = "Person deleted successfully!";
                }
                else
                {
                    StatusMessage = "Failed to delete person.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchPersonal()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(SearchText))
            {
                StatusMessage = "Please enter a search term.";
                return;
            }

            IsBusy = true;

            try
            {
                var personals = await _personalService.SearchPersonalsAsync(SearchText);
                PersonalList.Clear();
                foreach (var personal in personals)
                {
                    PersonalList.Add(personal);
                }
                StatusMessage = "Search completed.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error during search: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UpdatePersonal()
        {
            if (SelectedPersonal == null) return;

            IsBusy = true;

            try
            {
                SelectedPersonal.Name = NewPersonalName;
                SelectedPersonal.Gender = NewPersonalGender;
                SelectedPersonal.ContactNo = NewPersonalContactNo;

                var success = await _personalService.UpdatePersonalAsync(SelectedPersonal);
                if (success)
                {
                    StatusMessage = "Person updated successfully!";
                    await LoadData();
                }
                else
                {
                    StatusMessage = "Failed to update person.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error updating person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
